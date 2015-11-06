using System;
using System.Collections;
using System.Collections.Generic;

public class CPlayerOrderManager
{
	public List<CCard> random_cards { get; private set; }

	public CPlayerOrderManager()
	{
	}


	public void reset(CGostopEngine engine)
	{
		this.random_cards = engine.get_random_cards(2);
	}
}
