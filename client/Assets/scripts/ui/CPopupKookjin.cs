using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using FreeNet;

public class CPopupKookjin : MonoBehaviour {

	void Awake()
	{
		transform.FindChild("button_yes").GetComponent<Button>().onClick.AddListener(this.on_touch_01);
		transform.FindChild("button_no").GetComponent<Button>().onClick.AddListener(this.on_touch_02);
	}


	void on_touch_01()
	{
		on_choice_kookjin(1);
	}


	void on_touch_02()
	{
		on_choice_kookjin(0);
	}


	void on_choice_kookjin(byte use_kookjin)
	{
		gameObject.SetActive(false);

		CPacket msg = CPacket.create((short)PROTOCOL.ANSWER_KOOKJIN_TO_PEE);
		msg.push(use_kookjin);
		CNetworkManager.Instance.send(msg);
	}
}
