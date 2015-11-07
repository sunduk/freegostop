using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CMainMenu : MonoBehaviour
{
	GameObject room;
	GameObject ef_intro;

	void Awake()
	{
		this.room = GameObject.Find("Main").transform.FindChild("playroom").gameObject;
		transform.FindChild("button_play").GetComponent<Button>().onClick.AddListener(this.on_play);

		this.ef_intro = GameObject.Find("ef_falling");
	}


	void on_play()
	{
		this.room.SetActive(true);
		this.ef_intro.SetActive(false);
		CUIManager.Instance.hide(UI_PAGE.CREDIT_BAR);
		CUIManager.Instance.hide(UI_PAGE.MAIN_MENU);
	}
}
