using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using FreeNet;

public class CPopupShaking : MonoBehaviour {

	CCard selected_card_info;
	byte card_slot;

	void Awake()
	{
		transform.FindChild("button_yes").GetComponent<Button>().onClick.AddListener(this.on_touch_01);
		transform.FindChild("button_no").GetComponent<Button>().onClick.AddListener(this.on_touch_02);
	}


	public void refresh(CCard selected_card_info, byte card_slot)
	{
		this.selected_card_info = selected_card_info;
		this.card_slot = card_slot;
	}


	void on_touch_01()
	{
		on_choice_shaking(1);
	}


	void on_touch_02()
	{
		on_choice_shaking(0);
	}


	void on_choice_shaking(byte is_shaking)
	{
		gameObject.SetActive(false);

		CPlayRoomUI.send_select_card(this.selected_card_info, this.card_slot, is_shaking);
	}
}
