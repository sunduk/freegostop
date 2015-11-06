using System;
using System.Collections;
using System.Collections.Generic;

public class CFloorSlot
{
	public byte slot_position { get; private set; }
	public List<CCard> cards { get; private set; }
	
	public CFloorSlot(byte position)
	{
		this.cards = new List<CCard>();
		this.slot_position = position;

        reset();
	}


    public void reset()
    {
        this.cards.Clear();
    }


	public bool is_same(byte number)
	{
		if (this.cards.Count <= 0)
		{
			return false;
		}

		return this.cards[0].number == number;
	}


	public void add_card(CCard card)
	{
		this.cards.Add(card);
	}


	public void remove_card(CCard card)
	{
		this.cards.Remove(card);
	}

	
	public bool is_empty()
	{
		return this.cards.Count <= 0;
	}
}
