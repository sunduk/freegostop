using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CGameOption : MonoBehaviour {

	Toggle card_hint;


	void Awake()
	{
		this.card_hint = gameObject.GetComponent<Toggle>();
		this.card_hint.onValueChanged.AddListener(this.on_card_hint_changed);
	}


	void on_card_hint_changed(bool is_on)
	{
		CPlayRoomUI.Instance.refresh_hint_mark();
	}


	public bool is_hint_on()
	{
		return this.card_hint.isOn;
	}
}
