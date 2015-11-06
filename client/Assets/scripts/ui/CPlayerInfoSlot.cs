using UnityEngine;
using System.Collections;

public class CPlayerInfoSlot : MonoBehaviour {

	TextMesh score_text;
	TextMesh go_text;
	TextMesh shake_text;
	TextMesh ppuk_text;
	TextMesh pee_count_text;


	void Awake()
	{
		this.score_text = gameObject.transform.FindChild("score").GetComponent<TextMesh>();
		this.go_text = gameObject.transform.FindChild("go").GetComponent<TextMesh>();
		this.shake_text = gameObject.transform.FindChild("shake").GetComponent<TextMesh>();
		this.ppuk_text = gameObject.transform.FindChild("ppuk").GetComponent<TextMesh>();
		this.pee_count_text = gameObject.transform.FindChild("pee").GetComponent<TextMesh>();
	}


	public void update_score(short score)
	{
		this.score_text.text = score.ToString();
	}


	public void update_go(short go)
	{
		this.go_text.text = go.ToString();
	}


	public void update_shake(short shake)
	{
		this.shake_text.text = shake.ToString();
	}


	public void update_ppuk(short ppuk)
	{
		this.ppuk_text.text = ppuk.ToString();
	}


	public void update_peecount(byte count)
	{
		this.pee_count_text.text = string.Format("({0})", count);
	}
}
