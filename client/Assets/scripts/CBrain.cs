using System;
using System.Collections;
using System.Collections.Generic;

public class CBrain
{
	public CBrain()
	{
	}


	/// <summary>
	/// 낼 카드 선택.
	/// 먹을 수 있는 카드 중에서 제일 높은 점수를 얻을 수 있는 경우를 선택한다.
	/// 점수가 같을 경우 피, 광, 띠, 열끗 순으로 선택 한다.
	/// </summary>
	/// <returns></returns>
	public byte choice_card_to_put(
		List<CCard> hand_cards,
		List<CCard> bottom_cards,
		CFloorCardManager floor_card_manager)
	{
		List<byte> same_card_indexes = new List<byte>();

		for (byte i = 0; i < hand_cards.Count; ++i)
		{
			byte same_number = floor_card_manager.get_same_number_card_count(hand_cards[i].number);
			if (same_number >= 1)
			{
				same_card_indexes.Add(i);
			}
		}

		if (same_card_indexes.Count <= 0)
		{
			return 0;
		}

		UnityEngine.Debug.Log("[AI] " + same_card_indexes[0]);
		return same_card_indexes[0];
	}
}
