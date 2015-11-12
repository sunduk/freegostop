using System;
using System.Collections;
using System.Collections.Generic;
using FreeNet;

public class CGameRoom
{
	CLocalServer server;
    CGostopEngine engine;
    List<CPlayer> players;

    Dictionary<byte, PROTOCOL> received_protocol;
	CPlayerOrderManager order_manager;

    public CGameRoom(CLocalServer server)
    {
		this.server = server;
        this.engine = new CGostopEngine();
        this.players = new List<CPlayer>();
        this.received_protocol = new Dictionary<byte, PROTOCOL>();
		this.order_manager = new CPlayerOrderManager();
    }


    public void add_player(CPlayer newbie)
    {
        this.players.Add(newbie);
    }


    void reset()
    {
        this.engine.reset();
		for (int i = 0; i < this.players.Count; ++i)
		{
			this.players[i].reset();
		}
		this.order_manager.reset(this.engine);
    }


    bool is_received(byte player_index, PROTOCOL protocol)
    {
        if (!this.received_protocol.ContainsKey(player_index))
        {
            return false;
        }

        return this.received_protocol[player_index] == protocol;
    }


    void checked_protocol(byte player_index, PROTOCOL protocol)
    {
        //UnityEngine.Debug.Log(player_index + ",  sent : " + protocol);
		if (this.received_protocol.ContainsKey(player_index))
		{
			UnityEngine.Debug.LogError(string.Format("Already contains player. Please call 'clear_received_protocol()' before send to client."));
			return;
		}

        this.received_protocol.Add(player_index, protocol);
    }


    bool all_received(PROTOCOL protocol)
    {
        if (this.received_protocol.Count < this.players.Count)
        {
            return false;
        }

        foreach (KeyValuePair<byte, PROTOCOL> kvp in this.received_protocol)
        {
            if (kvp.Value != protocol)
            {
                return false;
            }
        }

		clear_received_protocol();
        return true;
    }


	void clear_received_protocol()
	{
		this.received_protocol.Clear();
	}


    //--------------------------------------------------------
    // Handler.
    //--------------------------------------------------------
    public void on_receive(CPlayer owner, CPacket msg)
    {
		PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();
        if (is_received(owner.player_index, protocol))
        {
            //error!! already exist!!
            return;
        }

        checked_protocol(owner.player_index, protocol);

		//UnityEngine.Debug.Log("protocol " + protocol);
		switch (protocol)
		{
			case PROTOCOL.READY_TO_START:
				on_ready_req();
				break;

			case PROTOCOL.DISTRIBUTED_ALL_CARDS:
				{
					if (all_received(protocol))
					{
						CPacket turn_msg = CPacket.create((short)PROTOCOL.START_TURN);
						turn_msg.push((byte)0);
						this.players[this.engine.current_player_index].send(turn_msg);
					}
				}
				break;

			case PROTOCOL.SELECT_CARD_REQ:
				{
					byte number = msg.pop_byte();
					PAE_TYPE pae_type = (PAE_TYPE)msg.pop_byte();
					byte position = msg.pop_byte();
					byte slot_index = msg.pop_byte();
					byte is_shaking = msg.pop_byte();
					//UnityEngine.Debug.Log("server. " + slot_index);
					on_player_put_card(
						owner.player_index, 
						number, pae_type, position, slot_index,
						is_shaking);
				}
				break;

			case PROTOCOL.CHOOSE_CARD:
				{
					clear_received_protocol();

					PLAYER_SELECT_CARD_RESULT client_result = (PLAYER_SELECT_CARD_RESULT)msg.pop_byte();
					byte choice_index = msg.pop_byte();
					PLAYER_SELECT_CARD_RESULT server_result =
						this.engine.on_choose_card(owner.player_index, client_result, choice_index);
					if (server_result == PLAYER_SELECT_CARD_RESULT.CHOICE_ONE_CARD_FROM_PLAYER)
					{
						PLAYER_SELECT_CARD_RESULT choose_result = 
							this.engine.flip_process(owner.player_index, 
							TURN_RESULT_TYPE.RESULT_OF_NORMAL_CARD);
						send_flip_result(choose_result, this.engine.current_player_index);
					}
					else
					{
						send_turn_result(this.engine.current_player_index);
					}
					//send_turn_result(this.engine.current_player_index, this.engine.selected_slot_index);
				}
				break;

            case PROTOCOL.FLIP_BOMB_CARD_REQ:
                {
                    clear_received_protocol();

                    if (this.engine.current_player_index != owner.player_index)
                    {
                        break;
                    }

                    if (!owner.agent.decrease_bomb_count())
                    {
                        // error!
                        UnityEngine.Debug.Log(string.Format("Invalid bomb count!! player {0}", owner.player_index));
                        break;
                    }

                    PLAYER_SELECT_CARD_RESULT choose_result = this.engine.flip_process(owner.player_index, TURN_RESULT_TYPE.RESULT_OF_BOMB_CARD);
					send_flip_result(choose_result, this.engine.current_player_index);
                }
                break;

			case PROTOCOL.FLIP_DECK_CARD_REQ:
				{
					clear_received_protocol();

					PLAYER_SELECT_CARD_RESULT result =
						this.engine.flip_process(this.engine.current_player_index, 
						TURN_RESULT_TYPE.RESULT_OF_NORMAL_CARD);
					send_flip_result(result, this.engine.current_player_index);
				}
				break;

			case PROTOCOL.TURN_END:
				{
					if (!all_received(PROTOCOL.TURN_END))
					{
						break;
					}

					if (this.engine.has_kookjin(this.engine.current_player_index))
					{
						CPacket ask_msg = CPacket.create((short)PROTOCOL.ASK_KOOKJIN_TO_PEE);
						this.players[this.engine.current_player_index].send(ask_msg);
					}
					else
					{
						check_game_finish();
					}
				}
				break;

			case PROTOCOL.ANSWER_KOOKJIN_TO_PEE:
				{
					clear_received_protocol();
					owner.agent.kookjin_selected();
					byte answer = msg.pop_byte();
					if (answer == 1)
					{
						// 국진을 쌍피로 이동.
						owner.agent.move_kookjin_to_pee();
						send_player_statistics(owner.player_index);
						broadcast_move_kookjin(owner.player_index);
					}

					check_game_finish();
				}
				break;

			case PROTOCOL.ANSWER_GO_OR_STOP:
				{
					clear_received_protocol();
					// answer가 1이면 GO, 0이면 STOP.
					byte answer = msg.pop_byte();
					if (answer == 1)
					{
						owner.agent.plus_go_count();
						broadcast_go_count(owner);
						next_turn();
					}
					else
					{
						broadcast_game_result();
					}
				}
				break;
		}
    }


	void broadcast_go_count(CPlayer player)
	{
        byte delay = get_aiplayer_delay(player);

		for (int i = 0; i < this.players.Count; ++i)
		{
			CPacket msg = CPacket.create((short)PROTOCOL.NOTIFY_GO_COUNT);
            msg.push(delay);
			msg.push(player.agent.go_count);
			this.players[i].send(msg);
		}
	}


	void broadcast_game_result()
	{
		CPlayer winner = this.players[0];

		for (int i = 0; i < this.players.Count; ++i)
		{
			CPacket msg = CPacket.create((short)PROTOCOL.GAME_RESULT);
			msg.push((byte)1);
			msg.push((short)15000);
			msg.push(winner.agent.score);
			msg.push((short)2);
			msg.push((short)16);
			this.players[i].send(msg);
		}
	}


	void broadcast_move_kookjin(byte who)
	{
		for (int i = 0; i < this.players.Count; ++i)
		{
			CPacket msg = CPacket.create((short)PROTOCOL.MOVE_KOOKJIN_TO_PEE);
			msg.push(who);
			this.players[i].send(msg);
		}
	}


	void check_game_finish()
	{
		if (this.engine.is_time_to_ask_gostop())
		{
			if (this.engine.is_last_turn())
			{
				// 막턴에 났으면 자동 스톱 처리.
				broadcast_game_result();
			}
			else
			{
				send_player_statistics(this.engine.current_player_index);
				send_go_or_stop();
			}
		}
		else
		{
			if (this.engine.is_finished())
			{
				//todo:게임이 끝났는데 나지 못했다면 나가리 처리.
				UnityEngine.Debug.Log("Nagari!!!!!!");
			}
			else
			{
				next_turn();
			}
		}
	}


	void send_go_or_stop()
	{
		CPacket msg = CPacket.create((short)PROTOCOL.ASK_GO_OR_STOP);
		this.players[this.engine.current_player_index].send(msg);
	}


	void next_turn()
	{
		send_player_statistics(this.engine.current_player_index);

		this.engine.clear_turn_data();
		this.engine.move_to_next_player();

		CPacket turn_msg = CPacket.create((short)PROTOCOL.START_TURN);
		turn_msg.push(this.players[this.engine.current_player_index].agent.remain_bomb_count);

		// 바닥 카드 갱신을 위한 데이터.
		List<CFloorSlot> slots = this.engine.floor_manager.slots;
		turn_msg.push((byte)slots.Count);
		for (int i = 0; i < slots.Count; ++i)
		{
			turn_msg.push((byte)slots[i].cards.Count);
			for (int card_index = 0; card_index < slots[i].cards.Count; ++card_index)
			{
				CCard card = slots[i].cards[card_index];
				turn_msg.push(card.number);
				turn_msg.push((byte)card.pae_type);
				turn_msg.push(card.position);
			}
		}

		this.players[this.engine.current_player_index].send(turn_msg);
	}


	public void on_ready_req()
	{
		if (!all_received(PROTOCOL.READY_TO_START))
		{
			return;
		}

		reset();
		for (int i = 0; i < this.players.Count; ++i)
		{
			CPacket msg = CPacket.create((short)PROTOCOL.PLAYER_ORDER_RESULT);
			msg.push((byte)this.order_manager.random_cards.Count);
			for (byte slot_index = 0; slot_index < this.order_manager.random_cards.Count; ++slot_index)
			{
				CCard card = this.order_manager.random_cards[slot_index];
				msg.push(slot_index);
				msg.push(card.number);
				msg.push((byte)card.pae_type);
				msg.push(card.position);
			}

			this.players[i].send(msg);
		}


		this.engine.start(this.players);
		for (int i = 0; i < this.players.Count; ++i)
		{
			send_cardinfo_to_player(this.players[i]);
		}
	}


    void send_cardinfo_to_player(CPlayer player)
    {
        byte count = (byte)this.engine.distributed_floor_cards.Count;

        CPacket msg = CPacket.create((short)PROTOCOL.BEGIN_CARD_INFO);
		msg.push(player.player_index);
        msg.push(count);
        for (int i = 0; i < count; ++i)
        {
            msg.push(this.engine.distributed_floor_cards[i].number);
            msg.push((byte)this.engine.distributed_floor_cards[i].pae_type);
            msg.push((byte)this.engine.distributed_floor_cards[i].position);
        }

		msg.push((byte)this.players.Count);
		for (int i = 0; i < this.players.Count; ++i)
		{
			byte player_index = this.players[i].player_index;
			byte players_card_count = (byte)this.engine.distributed_players_cards[player_index].Count;
			msg.push(player_index);
			msg.push(players_card_count);

			// 플레이어 본인의 카드정보만 실제 카드로 보내주고,
			// 다른 플레이어의 카드는 null카드로 보내줘서 클라이언트딴에서는 알지 못하게 한다.
			if (player.player_index == player_index)
			{
				for (int card_index = 0; card_index < players_card_count; ++card_index)
				{
					msg.push(this.engine.distributed_players_cards[player_index][card_index].number);
					msg.push((byte)this.engine.distributed_players_cards[player_index][card_index].pae_type);
					msg.push((byte)this.engine.distributed_players_cards[player_index][card_index].position);
				}
			}
			else
			{
				for (int card_index = 0; card_index < players_card_count; ++card_index)
				{
					// 다른 플레이어의 카드는 null카드로 보내준다.
					msg.push(byte.MaxValue);
				}
			}
		}

        player.send(msg);
    }


    public void on_player_put_card(byte player_index, 
		byte card_number,
		PAE_TYPE pae_type, 
		byte position,
		byte slot_index,
		byte is_shaking)
    {
		PLAYER_SELECT_CARD_RESULT result = this.engine.player_put_card(
			player_index, card_number, pae_type, position, slot_index, is_shaking);

		if (result == PLAYER_SELECT_CARD_RESULT.ERROR_INVALID_CARD)
		{
			return;
		}

		clear_received_protocol();
		send_select_card_ack(result, player_index, slot_index);
    }


	/// <summary>
	/// 카드 선택에 대한 결과를 모든 플레이어에게 전송한다.
	/// </summary>
	/// <param name="current_turn_player_index"></param>
	/// <param name="slot_index"></param>
	void send_select_card_ack(PLAYER_SELECT_CARD_RESULT result,
		byte current_turn_player_index, byte slot_index)
	{
        byte delay = get_aiplayer_delay(this.players[current_turn_player_index]);

		for (int i = 0; i < this.players.Count; ++i)
		{
			CPacket msg = CPacket.create((short)PROTOCOL.SELECT_CARD_ACK);

            msg.push(delay);

			// 플레이어 정보.
			msg.push(current_turn_player_index);

			// 낸 카드 정보.
			add_player_select_result_to(msg, slot_index);

			// 둘중 하나를 선택하는 경우 대상이 되는 카드 정보를 담는다.
			msg.push((byte)result);
			if (result == PLAYER_SELECT_CARD_RESULT.CHOICE_ONE_CARD_FROM_PLAYER)
			{
				add_choice_card_info_to(msg);
			}

			this.players[i].send(msg);
		}
	}


	void send_turn_result(byte current_turn_player_index)
	{
		for (int i = 0; i < this.players.Count; ++i)
		{
			CPacket msg = CPacket.create((short)PROTOCOL.TURN_RESULT);
			// 플레이어 정보.
			msg.push(current_turn_player_index);

			add_player_get_cards_info_to(msg);
			add_others_card_result_to(msg);
			add_turn_result_to(msg, current_turn_player_index);

			this.players[i].send(msg);
		}
	}


	void send_flip_result(PLAYER_SELECT_CARD_RESULT result, byte current_turn_player_index)
	{
		for (int i = 0; i < this.players.Count; ++i)
		{
			CPacket msg = CPacket.create((short)PROTOCOL.FLIP_DECK_CARD_ACK);
			// 플레이어 정보.
			msg.push(current_turn_player_index);

			add_flip_result_to(msg);

			msg.push((byte)result);
			if (result == PLAYER_SELECT_CARD_RESULT.CHOICE_ONE_CARD_FROM_DECK)
			{
				add_choice_card_info_to(msg);
			}
			else
			{
				add_player_get_cards_info_to(msg);
				add_others_card_result_to(msg);
				add_turn_result_to(msg, current_turn_player_index);
			}

			this.players[i].send(msg);
		}
	}


    void add_player_select_result_to(CPacket msg, byte slot_index)
    {
        // 플레이어가 낸 카드 정보.
        msg.push(this.engine.card_from_player.number);
        msg.push((byte)this.engine.card_from_player.pae_type);
        msg.push(this.engine.card_from_player.position);
        msg.push(this.engine.same_card_count_with_player);
        msg.push(slot_index);

        // 카드 이벤트.
        msg.push((byte)this.engine.card_event_type);

        // 폭탄 카드 정보.
		switch (this.engine.card_event_type)
		{
			case CARD_EVENT_TYPE.BOMB:
				{
					byte bomb_cards_count = (byte)this.engine.bomb_cards_from_player.Count;
					msg.push((byte)bomb_cards_count);
					for (byte card_index = 0; card_index < bomb_cards_count; ++card_index)
					{
						msg.push(this.engine.bomb_cards_from_player[card_index].number);
						msg.push((byte)this.engine.bomb_cards_from_player[card_index].pae_type);
						msg.push(this.engine.bomb_cards_from_player[card_index].position);
					}
				}
				break;

			case CARD_EVENT_TYPE.SHAKING:
				{
					byte shaking_cards_count = (byte)this.engine.shaking_cards.Count;
					msg.push((byte)shaking_cards_count);
					for (byte card_index = 0; card_index < shaking_cards_count; ++card_index)
					{
						msg.push(this.engine.shaking_cards[card_index].number);
						msg.push((byte)this.engine.shaking_cards[card_index].pae_type);
						msg.push(this.engine.shaking_cards[card_index].position);
					}
				}
				break;
		}
    }


    void add_flip_result_to(CPacket msg)
    {
        // 덱에서 뒤집은 카드 정보.
        msg.push(this.engine.card_from_deck.number);
        msg.push((byte)this.engine.card_from_deck.pae_type);
        msg.push(this.engine.card_from_deck.position);
        msg.push(this.engine.same_card_count_with_deck);
    }


	void add_player_get_cards_info_to(CPacket msg)
	{
		// 플레이어가 가져갈 카드 정보.
		byte count_to_player = (byte)this.engine.cards_to_give_player.Count;
		msg.push(count_to_player);
		for (byte card_index = 0; card_index < count_to_player; ++card_index)
		{
			CCard card = this.engine.cards_to_give_player[card_index];
			msg.push(card.number);
			msg.push((byte)card.pae_type);
			msg.push(card.position);
		}
	}


	void add_others_card_result_to(CPacket msg)
	{
		msg.push((byte)this.engine.other_cards_to_player.Count);

		foreach (KeyValuePair<byte, List<CCard>> kvp in this.engine.other_cards_to_player)
		{
			msg.push(kvp.Key);

			byte count = (byte)this.engine.other_cards_to_player[kvp.Key].Count;
			msg.push(count);
			for (byte card_index = 0; card_index < count; ++card_index)
			{
				CCard card = this.engine.other_cards_to_player[kvp.Key][card_index];
				msg.push(card.number);
				msg.push((byte)card.pae_type);
				msg.push(card.position);
			}
		}
	}


	void add_turn_result_to(CPacket msg, byte current_turn_player_index)
    {
        // 점수 정보.
        msg.push(this.players[current_turn_player_index].agent.score);

        // 기타 정보.
        msg.push(this.players[current_turn_player_index].agent.remain_bomb_count);

		// 카드 이벤트 정보.
		byte count = (byte)this.engine.flipped_card_event_type.Count;
		msg.push(count);
		for (byte i = 0; i < count; ++i)
		{
			msg.push((byte)this.engine.flipped_card_event_type[i]);
		}
    }


	void add_choice_card_info_to(CPacket msg)
	{
		List<CCard> target_cards = this.engine.target_cards_to_choice;
		byte count = (byte)target_cards.Count;
		msg.push(count);

		for (int i = 0; i < count; ++i)
		{
			CCard card = target_cards[i];
			msg.push(card.number);
			msg.push((byte)card.pae_type);
			msg.push(card.position);
		}
	}


	void send_player_statistics(byte player_index)
	{
		CPlayerAgent target_player = this.players[player_index].agent;
		for (int i = 0; i < this.players.Count; ++i)
		{
			CPacket msg = CPacket.create((short)PROTOCOL.UPDATE_PLAYER_STATISTICS);
			msg.push(player_index);
			msg.push(target_player.score);
			msg.push(target_player.go_count);
			msg.push(target_player.shaking_count);
			msg.push(target_player.ppuk_count);
			msg.push(target_player.get_pee_count());
			this.players[i].send(msg);
		}
	}


    /// <summary>
    /// ai플레이일 경우 딜레이 값을 넣어줘서 너무 빨리 진행되지 않도록 한다.
    /// </summary>
    /// <param name="current_player"></param>
    /// <returns></returns>
    byte get_aiplayer_delay(CPlayer current_player)
    {
        byte delay = 0;
        if (current_player.is_autoplayer())
        {
            delay = 1;
        }

        return delay;
    }
}
