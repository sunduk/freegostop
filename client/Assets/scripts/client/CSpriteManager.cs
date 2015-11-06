using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CSpriteManager : CSingletonMonobehaviour<CSpriteManager>
{
	Dictionary<string, Sprite> sprites_by_name;
	Sprite[] hwatoo;


	void Awake()
	{
		this.hwatoo = Resources.LoadAll<Sprite>("atlas/allcard");
		Sprite[] sprites = Resources.LoadAll<Sprite>("atlas/sp_ingame");

		this.sprites_by_name = new Dictionary<string, Sprite>();
		for (int i = 0; i < sprites.Length; ++i)
		{
			this.sprites_by_name.Add(sprites[i].name, sprites[i]);
		}
	}


	public Sprite get_sprite(string name)
	{
		if (!this.sprites_by_name.ContainsKey(name))
		{
			return null;
		}

		return this.sprites_by_name[name];
	}


	public Sprite get_card_sprite(int index)
	{
		return this.hwatoo[index];
	}
}
