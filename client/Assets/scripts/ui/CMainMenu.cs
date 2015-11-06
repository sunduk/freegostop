using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CMainMenu : MonoBehaviour
{
	GameObject room;

	void Awake()
	{
		this.room = GameObject.Find("Main").transform.FindChild("playroom").gameObject;
		transform.FindChild("button_play").GetComponent<Button>().onClick.AddListener(this.on_play);
	}


	void on_play()
	{
		this.room.SetActive(true);
		CUIManager.Instance.hide(UI_PAGE.MAIN_MENU);
	}
}
