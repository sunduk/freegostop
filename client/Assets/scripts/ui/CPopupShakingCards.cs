using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CPopupShakingCards : MonoBehaviour {

	List<Image> slots;

	void Awake()
	{
		this.slots = new List<Image>();
		for (int i = 0; i < 3; ++i)
		{
			Transform obj = transform.FindChild(string.Format("slot{0:D2}", (i + 1)));
			this.slots.Add(obj.GetComponent<Image>());
		}
	}


	public void refresh(List<Sprite> sprites)
	{
		if (sprites.Count != slots.Count)
		{
			Debug.LogError("Sprite count is different!! It must be 3 count.  sprites count : " + sprites.Count);
			return;
		}

		for (int i = 0; i < this.slots.Count; ++i)
		{
			this.slots[i].sprite = sprites[i];
		}
	}
}
