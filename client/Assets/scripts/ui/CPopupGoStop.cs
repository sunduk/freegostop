using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using FreeNet;

public class CPopupGoStop : MonoBehaviour {

	void Awake()
	{
		transform.FindChild("button_go").GetComponent<Button>().onClick.AddListener(this.on_touch_01);
		transform.FindChild("button_stop").GetComponent<Button>().onClick.AddListener(this.on_touch_02);
	}


	void on_touch_01()
	{
		on_choice_go_or_stop(1);
	}


	void on_touch_02()
	{
		on_choice_go_or_stop(0);
	}


	void on_choice_go_or_stop(byte is_go)
	{
		gameObject.SetActive(false);

		CPacket choose_msg = CPacket.create((short)PROTOCOL.ANSWER_GO_OR_STOP);
		choose_msg.push(is_go);
		CNetworkManager.Instance.send(choose_msg);
	}
}
