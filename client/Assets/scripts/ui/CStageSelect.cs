using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CStageSelect : MonoBehaviour {

	GameObject room;

	Text has_money;
	Text npc_name;
	Text grade;
	Text point;

	int current_level_index;

	void Awake()
	{
		transform.FindChild("button_challenge").GetComponent<Button>().onClick.AddListener(this.on_challenge);
		transform.FindChild("button_prev").GetComponent<Button>().onClick.AddListener(this.on_prev);
		transform.FindChild("button_next").GetComponent<Button>().onClick.AddListener(this.on_next);

		this.room = GameObject.Find("Main").transform.FindChild("playroom").gameObject;

		this.has_money = transform.FindChild("money").GetComponent<Text>();
		this.npc_name = transform.FindChild("npc_name").GetComponent<Text>();
		this.grade = transform.FindChild("grade").GetComponent<Text>();
		this.point = transform.FindChild("point").GetComponent<Text>();
	}


	public void show()
	{
		this.current_level_index = 0;
		refresh_current();
	}


	void on_challenge()
	{
		this.room.SetActive(true);

		CUIManager.Instance.hide(UI_PAGE.STAGE_SELECT);
		CUIManager.Instance.hide(UI_PAGE.CREDIT_BAR);
		CUIManager.Instance.hide(UI_PAGE.MAIN_MENU);
	}


	void on_prev()
	{
		if (this.current_level_index > 0)
		{
			--this.current_level_index;
			refresh_current();
		}
	}


	void on_next()
	{
		if (this.current_level_index < CTableDataManager.Instance.levels.Count - 1)
		{
			++this.current_level_index;
			refresh_current();
		}
	}


	void refresh_current()
	{
		refresh(CTableDataManager.Instance.levels[this.current_level_index]);
	}


	void refresh(CTableLevels level)
	{
		this.npc_name.text = string.Format("Lv.{0} {1}", level.level, level.name);
		this.has_money.text = string.Format("{0:n0}", level.has_money);
		this.grade.text = level.grade;
		this.point.text = string.Format("점 {0:n0}", level.point);
	}
}
