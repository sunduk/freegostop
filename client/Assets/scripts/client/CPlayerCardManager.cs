using System;
using System.Collections;
using System.Collections.Generic;

public class CPlayerCardManager
{
	Dictionary<PAE_TYPE, List<CCardPicture>> floor_slots;


	public CPlayerCardManager()
	{
		this.floor_slots = new Dictionary<PAE_TYPE, List<CCardPicture>>();
		this.floor_slots.Add(PAE_TYPE.KWANG, new List<CCardPicture>());
		this.floor_slots.Add(PAE_TYPE.TEE, new List<CCardPicture>());
		this.floor_slots.Add(PAE_TYPE.YEOL, new List<CCardPicture>());
		this.floor_slots.Add(PAE_TYPE.PEE, new List<CCardPicture>());
	}


	public void reset()
	{
		foreach (KeyValuePair<PAE_TYPE, List<CCardPicture>> kvp in this.floor_slots)
		{
			kvp.Value.Clear();
		}
	}


	public void add(CCardPicture card_pic)
	{
		PAE_TYPE pae_type = card_pic.card.pae_type;
		this.floor_slots[pae_type].Add(card_pic);
	}


	public void remove(CCardPicture card_pic)
	{
		PAE_TYPE pae_type = card_pic.card.pae_type;
		this.floor_slots[pae_type].Remove(card_pic);
	}


	public void remove(byte number, PAE_TYPE pae_type, byte position)
	{
		CCardPicture card_pic =
			this.floor_slots[pae_type].Find(obj => obj.is_same(number, pae_type, position));
		remove(card_pic);
	}


	public int get_card_count(PAE_TYPE pae_type)
	{
		return this.floor_slots[pae_type].Count;
	}


	public CCardPicture get_card(byte number, PAE_TYPE pae_type, byte position)
	{
		CCardPicture card_pic =
			this.floor_slots[pae_type].Find(obj => obj.is_same(number, pae_type, position));
		return card_pic;
	}


	public CCardPicture get_card_at(PAE_TYPE pae_type, int index)
	{
		return this.floor_slots[pae_type][index];
	}
}
