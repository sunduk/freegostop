using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CStageSelect : MonoBehaviour {

	GameObject room;

	void Awake()
	{
		transform.FindChild("button_challenge").GetComponent<Button>().onClick.AddListener(this.on_challenge);

		this.room = GameObject.Find("Main").transform.FindChild("playroom").gameObject;
	}


	void on_challenge()
	{
		this.room.SetActive(true);

		CUIManager.Instance.hide(UI_PAGE.STAGE_SELECT);
		CUIManager.Instance.hide(UI_PAGE.CREDIT_BAR);
		CUIManager.Instance.hide(UI_PAGE.MAIN_MENU);
	}
}
