using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CTableDataManager : CSingleton<CTableDataManager>
{
	public List<CTableLevels> levels { get; private set; }


	public CTableDataManager()
	{
		this.levels = new List<CTableLevels>();
	}


	public void load_all()
	{
		load_levels();
	}


	void load_levels()
	{
		TextAsset data = Resources.Load<TextAsset>("table/levels");
		ArrayList tables = XUtil.MiniJSON.jsonDecode(data.text) as ArrayList;

		for (int i = 0; i < tables.Count; ++i)
		{
			this.levels.Add(new CTableLevels((Hashtable)tables[i]));
		}
	}
}
