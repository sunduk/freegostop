using UnityEngine;
using System.Collections;
using FreeNet;

public class CCardPicture : MonoBehaviour {

	public CCard card { get; private set; }
	public SpriteRenderer sprite_renderer { get; private set; }

	public byte slot { get; private set; }
	BoxCollider box_collider;


	void Awake()
	{
		this.sprite_renderer = gameObject.GetComponentInChildren<SpriteRenderer>();
		this.box_collider = gameObject.GetComponent<BoxCollider>();
	}


	public void set_slot_index(byte slot)
	{
		this.slot = slot;
	}


	public void update_card(CCard card, Sprite image)
	{
		this.card = card;
		this.sprite_renderer.sprite = image;
	}


	public void update_backcard(Sprite back_image)
	{
		this.card = null;
		update_image(back_image);
	}


	public void update_image(Sprite image)
	{
		this.sprite_renderer.sprite = image;
	}


	public bool is_same(byte number)
	{
		return this.card.number == number;
	}


	public bool is_same(byte number, PAE_TYPE pae_type, byte position)
	{
		return this.card.is_same(number, pae_type, position);
	}


	public void on_touch()
	{
		//if (this.card != null)
		//{
		//	Debug.Log("on touch " + this.slot);
		//	//todo:[3장 같은 카드 AND 바닥에 같은게 없을 때] 흔들기 팝업 출력.
		//	CPlayRoomUI.send_select_card(this.card, this.slot, 0);
		//}
		//else
		//{
		//	Debug.Log("on back image touch " + this.slot);
		//	CPacket msg = CPacket.create((short)PROTOCOL.FLIP_BOMB_CARD_REQ);
		//	CNetworkManager.Instance.send(msg);
		//}
	}


	public void enable_collider(bool flag)
	{
		this.box_collider.enabled = flag;
	}


	public bool is_back_card()
	{
		return this.card == null;
	}
}
