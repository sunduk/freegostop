using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CPopupGo : MonoBehaviour {

	List<Sprite> go_images;
	Image go;

	void Awake()
	{
		this.go_images = new List<Sprite>();
		for (int i = 1; i <= 9; ++i)
		{
			Sprite spr = CSpriteManager.Instance.get_sprite(string.Format("go_{0:D2}", i));
			this.go_images.Add(spr);
		}

		this.go = transform.FindChild("image").GetComponent<Image>();
	}


	public void refresh(int howmany_go)
	{
		if (howmany_go <= 0 || howmany_go >= 10)
		{
			return;
		}

		this.go.sprite = this.go_images[howmany_go - 1];
	}
}
