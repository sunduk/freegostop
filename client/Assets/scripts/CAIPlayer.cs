using System;
using System.Collections;
using System.Collections.Generic;
using FreeNet;

public class CAIPlayer
{
    delegate void PacketFn(CPacket msg);
    Dictionary<PROTOCOL, PacketFn> packet_handler;

	byte player_index;
	List<CCard> hand_cards;

	CBrain brain;
	CFloorCardManager floor_card_manager;

    CLocalServer local_server;

    public CAIPlayer(CLocalServer local_server)
    {
        this.local_server = local_server;
		this.brain = new CBrain();
		this.hand_cards = new List<CCard>();
		this.floor_card_manager = new CFloorCardManager();
		reset();

        this.packet_handler = new Dictionary<PROTOCOL, PacketFn>();
        this.packet_handler.Add(PROTOCOL.LOCAL_SERVER_STARTED, ON_LOCAL_SERVER_STARTED);
        this.packet_handler.Add(PROTOCOL.BEGIN_CARD_INFO, ON_BEGIN_CARD_INFO);
		this.packet_handler.Add(PROTOCOL.START_TURN, ON_START_TURN);
		this.packet_handler.Add(PROTOCOL.SELECT_CARD_ACK, ON_SELECT_CARD_ACK);
		this.packet_handler.Add(PROTOCOL.FLIP_DECK_CARD_ACK, ON_FLIP_DECK_CARD_ACK);
		this.packet_handler.Add(PROTOCOL.TURN_RESULT, ON_TURN_RESULT);
		this.packet_handler.Add(PROTOCOL.ASK_GO_OR_STOP, ON_ASK_GO_OR_STOP);
		this.packet_handler.Add(PROTOCOL.ASK_KOOKJIN_TO_PEE, ON_ASK_KOOKJIN_TO_PEE);
		this.packet_handler.Add(PROTOCOL.GAME_RESULT, ON_GAME_RESULT);
    }


	public void reset()
	{
		this.hand_cards.Clear();
		this.floor_card_manager.reset();
	}


	bool is_me(byte player_index)
	{
		return this.player_index == player_index;
	}


    public void send(CPacket msg)
    {
        CPacket clone = CLocalServer.pre_send(msg);
        CPacket.destroy(msg);
        on_receive(clone);
    }


    void on_receive(CPacket msg)
    {
        PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();

        //UnityEngine.Debug.Log("AIPlayer received : " + protocol);

        if (!this.packet_handler.ContainsKey(protocol))
        {
            return;
        }

        this.packet_handler[protocol](msg);
    }


    // handler.
    void ON_LOCAL_SERVER_STARTED(CPacket msg)
    {
		send_ready_to_start();
    }


    void ON_BEGIN_CARD_INFO(CPacket msg)
    {
		Queue<CCard> floor_cards = new Queue<CCard>();
		this.player_index = msg.pop_byte();
		// floor cards.
		byte floor_count = msg.pop_byte();
		for (byte i = 0; i < floor_count; ++i)
		{
			byte number = msg.pop_byte();
			PAE_TYPE pae_type = (PAE_TYPE)msg.pop_byte();
			byte position = msg.pop_byte();

			CCard card = new CCard(number, pae_type, position);
			floor_cards.Enqueue(card);
			this.floor_card_manager.puton_card(card);
		}


		byte player_count = msg.pop_byte();
		for (byte player = 0; player < player_count; ++player)
		{
			Queue<CCard> cards = new Queue<CCard>();
			byte player_index = msg.pop_byte();
			byte card_count = msg.pop_byte();
			for (byte i = 0; i < card_count; ++i)
			{
				byte number = msg.pop_byte();
				PAE_TYPE pae_type = PAE_TYPE.KWANG;
				byte position = byte.MaxValue;
				if (number != byte.MaxValue)
				{
					pae_type = (PAE_TYPE)msg.pop_byte();
					position = msg.pop_byte();

					// AI플레이어 본인 것만 담는다.
					CCard card = new CCard(number, pae_type, position);
					this.hand_cards.Add(card);
				}
			}
		}


		CPacket finished_msg = CPacket.create((short)PROTOCOL.DISTRIBUTED_ALL_CARDS);
		this.local_server.on_receive_from_ai(finished_msg);
    }


	void update_floor_cards(CPacket msg)
	{
		this.floor_card_manager.reset();
		byte slot_count = msg.pop_byte();
		for (byte i = 0; i < slot_count; ++i)
		{
			byte card_count = msg.pop_byte();
			for (byte card_index = 0; card_index < card_count; ++card_index)
			{
				byte number = msg.pop_byte();
				PAE_TYPE pae_type = (PAE_TYPE)msg.pop_byte();
				byte position = msg.pop_byte();

				this.floor_card_manager.puton_card(new CCard(number, pae_type, position));
			}
		}
	}


	void ON_START_TURN(CPacket msg)
	{
		byte remain_bomb_card_count = msg.pop_byte();
		update_floor_cards(msg);

		if (remain_bomb_card_count > 0)
		{
			CPacket card_msg = CPacket.create((short)PROTOCOL.FLIP_BOMB_CARD_REQ);
			this.local_server.on_receive_from_ai(card_msg);
		}
		else
		{
			byte slot_index = this.brain.choice_card_to_put(this.hand_cards, null, this.floor_card_manager);
			CPacket card_msg = CPacket.create((short)PROTOCOL.SELECT_CARD_REQ);
			CCard card = this.hand_cards[slot_index];

			card_msg.push(card.number);
			card_msg.push((byte)card.pae_type);
			card_msg.push(card.position);
			card_msg.push(slot_index);

			byte is_shaking = 1;
			card_msg.push(is_shaking);

			//UnityEngine.Debug.Log(string.Format("sent {0}, {1}, {2}", card.number, card.pae_type, card.position));
			this.local_server.on_receive_from_ai(card_msg);
		}
	}


	void ON_SELECT_CARD_ACK(CPacket msg)
	{
        byte delay = msg.pop_byte();
		byte current_player_index = msg.pop_byte();
		//short score = msg.pop_int16();
		//byte remain_bomb_card_count = msg.pop_byte();

		if (is_me(current_player_index))
		{
			// 카드 내는 연출을 위해 필요한 변수들.
			CARD_EVENT_TYPE card_event = CARD_EVENT_TYPE.NONE;
			byte slot_index = byte.MaxValue;
			byte player_card_number = byte.MaxValue;
			PAE_TYPE player_card_pae_type = PAE_TYPE.PEE;
			byte player_card_position = byte.MaxValue;

			// 플레이어가 낸 카드 정보.
			player_card_number = msg.pop_byte();
			player_card_pae_type = (PAE_TYPE)msg.pop_byte();
			player_card_position = msg.pop_byte();
			byte same_count_with_player = msg.pop_byte();
			slot_index = msg.pop_byte();

			card_event = (CARD_EVENT_TYPE)msg.pop_byte();
			switch (card_event)
			{
				case CARD_EVENT_TYPE.BOMB:
					{
						byte bomb_card_count = (byte)msg.pop_byte();
						for (byte i = 0; i < bomb_card_count; ++i)
						{
							byte number = msg.pop_byte();
							PAE_TYPE pae_type = (PAE_TYPE)msg.pop_byte();
							byte position = msg.pop_byte();

							CCard card = this.hand_cards.Find(obj => obj.is_same(number, pae_type, position));
							this.hand_cards.Remove(card);
						}
					}
					break;

				case CARD_EVENT_TYPE.SHAKING:
					{
						byte shaking_card_count = (byte)msg.pop_byte();
						for (byte i = 0; i < shaking_card_count; ++i)
						{
							byte number = msg.pop_byte();
							PAE_TYPE pae_type = (PAE_TYPE)msg.pop_byte();
							byte position = msg.pop_byte();
						}

						this.hand_cards.RemoveAt(slot_index);
					}
					break;

				default:
					this.hand_cards.RemoveAt(slot_index);
					break;
			}


			PLAYER_SELECT_CARD_RESULT select_result = (PLAYER_SELECT_CARD_RESULT)msg.pop_byte();
			if (select_result == PLAYER_SELECT_CARD_RESULT.CHOICE_ONE_CARD_FROM_PLAYER)
			{
				byte count = msg.pop_byte();
				for (byte i = 0; i < count; ++i)
				{
					byte number = msg.pop_byte();
					PAE_TYPE pae_type = (PAE_TYPE)msg.pop_byte();
					byte position = msg.pop_byte();
				}

				CPacket choose_msg = CPacket.create((short)PROTOCOL.CHOOSE_CARD);
				choose_msg.push((byte)select_result);
				choose_msg.push((byte)0);
				this.local_server.on_receive_from_ai(choose_msg);
				return;
			}

			CPacket ret = CPacket.create((short)PROTOCOL.FLIP_DECK_CARD_REQ);
			this.local_server.on_receive_from_ai(ret);
		}
	}


	void ON_FLIP_DECK_CARD_ACK(CPacket msg)
	{
		byte player_index = msg.pop_byte();

		// 덱에서 뒤집은 카드 정보.
		byte deck_card_number = msg.pop_byte();
		PAE_TYPE deck_card_pae_type = (PAE_TYPE)msg.pop_byte();
		byte deck_card_position = msg.pop_byte();
		byte same_count_with_deck = msg.pop_byte();

		PLAYER_SELECT_CARD_RESULT result = (PLAYER_SELECT_CARD_RESULT)msg.pop_byte();
		if (result == PLAYER_SELECT_CARD_RESULT.CHOICE_ONE_CARD_FROM_DECK)
		{
			if (is_me(player_index))
			{
				CPacket choose_msg = CPacket.create((short)PROTOCOL.CHOOSE_CARD);
				choose_msg.push((byte)result);
				choose_msg.push((byte)0);
				this.local_server.on_receive_from_ai(choose_msg);
			}
		}
		else
		{
			CPacket ret = CPacket.create((short)PROTOCOL.TURN_END);
			this.local_server.on_receive_from_ai(ret);
		}
	}


	void ON_TURN_RESULT(CPacket msg)
	{
		CPacket finish = CPacket.create((short)PROTOCOL.TURN_END);
		this.local_server.on_receive_from_ai(finish);
	}


	void ON_ASK_GO_OR_STOP(CPacket msg)
	{
		CPacket answer_msg = CPacket.create((short)PROTOCOL.ANSWER_GO_OR_STOP);
		answer_msg.push((byte)1);		// 0:스톱, 1:고.
		this.local_server.on_receive_from_ai(answer_msg);
	}


	void ON_ASK_KOOKJIN_TO_PEE(CPacket msg)
	{
		CPacket answer_msg = CPacket.create((short)PROTOCOL.ANSWER_KOOKJIN_TO_PEE);
		answer_msg.push((byte)1);		// 0:사용하지 않음, 1:쌍피로 사용.
		this.local_server.on_receive_from_ai(answer_msg);
	}


	void ON_GAME_RESULT(CPacket msg)
	{
		send_ready_to_start();
	}


	void send_ready_to_start()
	{
		CPacket send = CPacket.create((short)PROTOCOL.READY_TO_START);
		this.local_server.on_receive_from_ai(send);
	}
}
