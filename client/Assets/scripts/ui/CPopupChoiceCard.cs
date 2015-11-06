using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using FreeNet;

public class CPopupChoiceCard : MonoBehaviour {

	List<Image> slots;
	PLAYER_SELECT_CARD_RESULT result_type_from_server;

	void Awake()
	{
		this.slots = new List<Image>();
		for (int i = 0; i < 2; ++i)
		{
			Transform obj = transform.FindChild(string.Format("slot{0:D2}", (i + 1)));
			this.slots.Add(obj.GetComponent<Image>());
		}

		this.slots[0].GetComponent<Button>().onClick.AddListener(this.on_touch_01);
		this.slots[1].GetComponent<Button>().onClick.AddListener(this.on_touch_02);
	}


	public void refresh(PLAYER_SELECT_CARD_RESULT result_type, Sprite card1, Sprite card2)
	{
		this.result_type_from_server = result_type;
		Debug.Log(this.slots.Count);
		this.slots[0].sprite = card1;
		this.slots[1].sprite = card2;
	}


	void on_touch_01()
	{
		on_choice_card(0);
	}


	void on_touch_02()
	{
		on_choice_card(1);
	}


	void on_choice_card(byte slot_index)
	{
		gameObject.SetActive(false);

		CPacket choose_msg = CPacket.create((short)PROTOCOL.CHOOSE_CARD);
		choose_msg.push((byte)this.result_type_from_server);
		choose_msg.push((byte)slot_index);
		CNetworkManager.Instance.send(choose_msg);
	}
}
