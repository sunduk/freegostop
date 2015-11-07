using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum UI_PAGE
{
	PLAY_ROOM,
	POPUP_PLAYER_ORDER,
	POPUP_CHOICE_CARD,
	POPUP_GO_STOP,
	POPUP_ASK_SHAKING,
	POPUP_SHAKING_CARDS,
	POPUP_ASK_KOOKJIN,
	POPUP_GAME_RESULT,
	POPUP_GO_COUNT,
	POPUP_STOP,
	POPUP_FIRST_PLAYER,
	GAME_OPTION,
	MAIN_MENU,
	CREDIT_BAR,
	STAGE_SELECT,
}

public class CUIManager : CSingletonMonobehaviour<CUIManager>
{
	Dictionary<UI_PAGE, GameObject> ui_objects;

	void Awake()
	{
		this.ui_objects = new Dictionary<UI_PAGE, GameObject>();
		this.ui_objects.Add(UI_PAGE.MAIN_MENU, transform.FindChild("main_menu").gameObject);
		this.ui_objects.Add(UI_PAGE.CREDIT_BAR, transform.FindChild("credit").gameObject);
		this.ui_objects.Add(UI_PAGE.POPUP_PLAYER_ORDER, transform.FindChild("popup_player_order").gameObject);
		this.ui_objects.Add(UI_PAGE.POPUP_CHOICE_CARD, transform.FindChild("popup_choice_card").gameObject);
		this.ui_objects.Add(UI_PAGE.POPUP_GO_STOP, transform.FindChild("popup_gostop").gameObject);
		this.ui_objects.Add(UI_PAGE.POPUP_ASK_SHAKING, transform.FindChild("popup_shaking").gameObject);
		this.ui_objects.Add(UI_PAGE.POPUP_SHAKING_CARDS, transform.FindChild("popup_shaking_cards").gameObject);
		this.ui_objects.Add(UI_PAGE.POPUP_ASK_KOOKJIN, transform.FindChild("popup_kookjin").gameObject);
		this.ui_objects.Add(UI_PAGE.POPUP_GAME_RESULT, transform.FindChild("popup_result").gameObject);
		this.ui_objects.Add(UI_PAGE.POPUP_GO_COUNT, transform.FindChild("popup_go").gameObject);
		this.ui_objects.Add(UI_PAGE.POPUP_STOP, transform.FindChild("popup_stop").gameObject);
		this.ui_objects.Add(UI_PAGE.POPUP_FIRST_PLAYER, transform.FindChild("popup_first_player").gameObject);
		this.ui_objects.Add(UI_PAGE.GAME_OPTION, transform.FindChild("option_cardhint").gameObject);
		this.ui_objects.Add(UI_PAGE.STAGE_SELECT, transform.FindChild("stage").gameObject);
	}


	public GameObject get_uipage(UI_PAGE page)
	{
		return this.ui_objects[page];
	}


	public void show(UI_PAGE page)
	{
		this.ui_objects[page].SetActive(true);
	}


	public void hide(UI_PAGE page)
	{
		this.ui_objects[page].SetActive(false);
	}
}
