using System;
using System.Collections;
using System.Collections.Generic;

public class CTableLevels
{
	public short level { get; private set; }
	public string name { get; private set; }
	public Int32 point { get; private set; }
	public Int64 has_money { get; private set; }
	public string grade { get; private set; }

	public CTableLevels(Hashtable table)
	{
		this.level = short.Parse(table["level"].ToString());
		this.name = table["name"].ToString();
		this.point = Int32.Parse(table["point"].ToString());
		this.has_money = Int64.Parse(table["money"].ToString());
		this.grade = table["grade"].ToString();
	}
}
