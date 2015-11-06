using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using FreeNet;

public class CPopupPlayerOrder : MonoBehaviour {

	GameObject root;
	List<Transform> slots;

	void Awake()
	{
		this.slots = new List<Transform>();

		this.root = transform.FindChild("root").gameObject;
		Transform slot01 = root.transform.FindChild("slot01");
		this.slots.Add(slot01);

		Transform slot02 = root.transform.FindChild("slot02");
		this.slots.Add(slot02);
	}


	public void reset(Sprite sprite)
	{
		for (int i = 0; i < this.slots.Count; ++i)
		{
			this.slots[i].GetComponent<Image>().sprite = sprite;
		}
	}


	public void play()
	{
		this.root.GetComponent<Animator>().Play("player_order_01");
	}


	public void update_slot_info(byte slot_index, Sprite sprite)
	{
		this.slots[slot_index].GetComponent<Image>().sprite = sprite;
	}
}
