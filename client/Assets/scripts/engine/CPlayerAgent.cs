using System;
using System.Collections;
using System.Collections.Generic;

public class CPlayerAgent
{
	byte player_index;

    Dictionary<PAE_TYPE, List<CCard>> floor_pae;
    List<CCard> hand_pae;

	public short score { get; private set; }
	public short prev_score { get; private set; }

	public byte go_count { get; private set; }
	public byte shaking_count { get; private set; }
	public byte ppuk_count { get; private set; }

    public byte remain_bomb_count { get; private set; }
	public bool is_used_kookjin { get; private set; }


    public CPlayerAgent(byte player_index)
    {
        this.player_index = player_index;
        this.hand_pae = new List<CCard>();
        this.floor_pae = new Dictionary<PAE_TYPE, List<CCard>>();
		this.score = 0;
		this.prev_score = 0;
        this.remain_bomb_count = 0;
    }


	/// <summary>
	/// 매판 시작 전 초기화 해야 할 변수들.
	/// </summary>
	public void reset()
	{
		this.score = 0;
		this.prev_score = 0;
		this.go_count = 0;
		this.shaking_count = 0;
		this.ppuk_count = 0;
		this.hand_pae.Clear();
		this.floor_pae.Clear();
        this.remain_bomb_count = 0;
		this.is_used_kookjin = false;
	}


    public void add_card_to_hand(CCard card)
    {
		//UnityEngine.Debug.Log(string.Format("add to hand. player {0},   {1}, {2}, {3}",
		//	this.player_index, card.number, card.pae_type, card.position));
		this.hand_pae.Add(card);
    }


	public CCard pop_card_from_hand(
		byte card_number,
		PAE_TYPE pae_type,
		byte position)
    {
		CCard card = this.hand_pae.Find(obj =>
		{
			return obj.number == card_number &&
				obj.pae_type == pae_type &&
				obj.position == position;
		});

		if (card == null)
		{
			return null;
		}

		this.hand_pae.Remove(card);
		return card;
    }


	public List<CCard> pop_all_cards_from_hand(byte card_number)
	{
		List<CCard> cards = this.hand_pae.FindAll(obj => obj.is_same_number(card_number));
		List<CCard> result = new List<CCard>();
		for (int i = 0; i < cards.Count; ++i)
		{
			//UnityEngine.Debug.Log(string.Format("pop bomb card {0}, {1}, {2}", cards[i].number,
			//	cards[i].pae_type, cards[i].position));

			result.Add(cards[i]);
			this.hand_pae.Remove(cards[i]);
		}

		return result;
	}


    public void add_card_to_floor(CCard card)
    {
		if (!this.floor_pae.ContainsKey(card.pae_type))
		{
			this.floor_pae.Add(card.pae_type, new List<CCard>());
		}
		this.floor_pae[card.pae_type].Add(card);
    }


	public List<CCard> pop_card_from_floor(byte pee_count_to_want)
	{
		// 피가 한장도 없다면 줄 수 있는게 없다.
		if (!this.floor_pae.ContainsKey(PAE_TYPE.PEE))
		{
			return null;
		}

		List<CCard> player_pees = this.floor_pae[PAE_TYPE.PEE];
		if (player_pees.Count <= 0)
		{
			return null;
		}

		List<CCard> result = new List<CCard>();
		if (player_pees.Count == 1)
		{
			// 갖고 있는 피가 한장밖에 없는 경우에는 그것밖에 줄게 없다.
			result.Add(player_pees[0]);
		}
		else
		{
			if (pee_count_to_want == 1)
			{
				CCard onepee_card = player_pees.Find(obj => obj.status != CARD_STATUS.TWO_PEE);
				if (onepee_card != null)
				{
					result.Add(onepee_card);
				}
				else
				{
					result.Add(player_pees[0]);
				}
			}
			else if (pee_count_to_want == 2)
			{
				// 쌍피짜리 한장이 있다면 쌍피를 내어준다.
				CCard twopee_card = player_pees.Find(obj => obj.status == CARD_STATUS.TWO_PEE);
				if (twopee_card != null)
				{
					result.Add(twopee_card);
				}
				else
				{
					for (int i = 0; i < pee_count_to_want; ++i)
					{
						result.Add(player_pees[i]);
					}
				}
			}
		}


		// 플레이어의 바닥패에서 제거.
		for (int i = 0; i < result.Count; ++i)
		{
			player_pees.Remove(result[i]);
		}
		if (player_pees.Count <= 0)
		{
			this.floor_pae.Remove(PAE_TYPE.PEE);
		}

		return result;
	}


	CCard pop_specific_card_from_floor(PAE_TYPE pae_type, CARD_STATUS status)
	{
		if (!this.floor_pae.ContainsKey(pae_type))
		{
			return null;
		}

		CCard card = this.floor_pae[pae_type].Find(obj => obj.status == status);
		this.floor_pae[pae_type].Remove(card);
		return card;
	}


	List<CCard> find_cards(PAE_TYPE pae_type)
	{
		if (this.floor_pae.ContainsKey(pae_type))
		{
			return this.floor_pae[pae_type];
		}

		return null;
	}


	public byte get_card_count(PAE_TYPE pae_type, CARD_STATUS status)
	{
		if (!this.floor_pae.ContainsKey(pae_type))
		{
			return 0;
		}

		List<CCard> targets = this.floor_pae[pae_type].FindAll(obj => obj.is_same_status(status));
		if (targets == null)
		{
			return 0;
		}

		return (byte)targets.Count;
	}


	public byte get_same_card_count_from_hand(byte number)
	{
		List<CCard> same_cards = find_same_cards_from_hand(number);
		if (same_cards == null)
		{
			return 0;
		}

		return (byte)same_cards.Count;
	}


	public List<CCard> find_same_cards_from_hand(byte number)
	{
		return this.hand_pae.FindAll(obj => obj.is_same_number(number));
	}


	public byte get_pee_count()
	{
		List<CCard> cards = find_cards(PAE_TYPE.PEE);
		if (cards == null)
		{
			return 0;
		}

		byte twopee_count = get_card_count(PAE_TYPE.PEE, CARD_STATUS.TWO_PEE);
		byte total_pee_count = (byte)(cards.Count + twopee_count);
		return total_pee_count;
	}


	short get_score_by_type(PAE_TYPE pae_type)
	{
		short pae_score = 0;

		List<CCard> cards = find_cards(pae_type);
		if (cards == null)
		{
			return 0;
		}

		switch (pae_type)
		{
			case PAE_TYPE.PEE:
				{
					byte twopee_count = get_card_count(PAE_TYPE.PEE, CARD_STATUS.TWO_PEE);
					byte total_pee_count = (byte)(cards.Count + twopee_count);
					//UnityEngine.Debug.Log(string.Format("[SCORE] Player {0}, total pee {1}, twopee {2}",
					//	this.player_index, total_pee_count, twopee_count));
					if (total_pee_count >= 10)
					{
						pae_score = (short)(total_pee_count - 9);
					}
				}
				break;

			case PAE_TYPE.TEE:
				if (cards.Count >= 5)
				{
					pae_score = (short)(cards.Count - 4);
				}
				break;

			case PAE_TYPE.YEOL:
				if (cards.Count >= 5)
				{
					pae_score = (short)(cards.Count - 4);
				}
				break;

			case PAE_TYPE.KWANG:
				if (cards.Count == 5)
				{
					pae_score = 15;
				}
				else if (cards.Count == 4)
				{
					pae_score = 4;
				}
				else if (cards.Count == 3)
				{
					// 비광이 포함되어 있으면 2점. 아니면 3점.
					bool is_exist_beekwang = cards.Exists(obj => obj.is_same_number(CCard.BEE_KWANG));
					if (is_exist_beekwang)
					{
						pae_score = 2;
					}
					else
					{
						pae_score = 3;
					}
				}
				break;
		}

		//UnityEngine.Debug.Log(string.Format("{0} {1} score", pae_type, pae_score));
		return pae_score;
	}


	public void calculate_score()
	{
		this.score = 0;
		this.score += get_score_by_type(PAE_TYPE.PEE);
		this.score += get_score_by_type(PAE_TYPE.TEE);
		this.score += get_score_by_type(PAE_TYPE.YEOL);
		this.score += get_score_by_type(PAE_TYPE.KWANG);

		// 고도리
		byte godori_count = get_card_count(PAE_TYPE.YEOL, CARD_STATUS.GODORI);
		if (godori_count == 3)
		{
			this.score += 5;
			//UnityEngine.Debug.Log("Godori 5 score");
		}

		// 홍단, 초단, 청단
		byte cheongdan_count = get_card_count(PAE_TYPE.TEE, CARD_STATUS.CHEONG_DAN);
		byte hongdan_count = get_card_count(PAE_TYPE.TEE, CARD_STATUS.HONG_DAN);
		byte chodan_count = get_card_count(PAE_TYPE.TEE, CARD_STATUS.CHO_DAN);
		if (cheongdan_count == 3)
		{
			this.score += 3;
			//UnityEngine.Debug.Log("Cheongdan 3 score");
		}

		if (hongdan_count == 3)
		{
			this.score += 3;
			//UnityEngine.Debug.Log("Hongdan 3 score");
		}

		if (chodan_count == 3)
		{
			this.score += 3;
			//UnityEngine.Debug.Log("Chodan 3 score");
		}

		//UnityEngine.Debug.Log(string.Format("[SCORE] player {0},  score {1}", this.player_index, this.score));
	}


	/// <summary>
	/// 플레이어의 패를 번호 순서에 따라 오름차순 정렬 한다.
	/// </summary>
	/// <param name="player_index"></param>
	public void sort_player_hand_slots()
	{
		this.hand_pae.Sort((CCard lhs, CCard rhs) =>
		{
			if (lhs.number < rhs.number)
			{
				return -1;
			}
			else if (lhs.number > rhs.number)
			{
				return 1;
			}

			return 0;
		});

		string debug = string.Format("player [{0}] ", this.player_index);
		for (int i = 0; i < this.hand_pae.Count; ++i)
		{
			debug += string.Format("{0}, ",
				this.hand_pae[i].number);
		}
		//UnityEngine.Debug.Log(debug);
	}


    public void add_bomb_count(byte count)
    {
        this.remain_bomb_count += count;
    }


    public bool decrease_bomb_count()
    {
        if (this.remain_bomb_count > 0)
        {
            --this.remain_bomb_count;
            return true;
        }

        return false;
    }


	public bool can_finish()
	{
		if (this.score < 7)
		{
			return false;
		}

		if (this.prev_score >= this.score)
		{
			return false;
		}

		this.prev_score = this.score;
		return true;
	}


	public void plus_go_count()
	{
		++this.go_count;
	}


	public void plus_shaking_count()
	{
		++this.shaking_count;
	}


	public void plus_ppuk_count()
	{
		++this.ppuk_count;
	}


	public void kookjin_selected()
	{
		this.is_used_kookjin = true;
	}


	public void move_kookjin_to_pee()
	{
		CCard card = pop_specific_card_from_floor(PAE_TYPE.YEOL, CARD_STATUS.KOOKJIN);
		if (card == null)
		{
			// 국진이 없다!??
			UnityEngine.Debug.LogError("Cannot find kookjin!!  player : " + this.player_index);
			return;
		}

		card.change_pae_type(PAE_TYPE.PEE);
		card.set_card_status(CARD_STATUS.TWO_PEE);

		add_card_to_floor(card);
		calculate_score();
	}


	public bool is_empty_on_hand()
	{
		return this.hand_pae.Count <= 0;
	}
}
