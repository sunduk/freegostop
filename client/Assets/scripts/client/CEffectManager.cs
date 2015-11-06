using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CEffectManager : CSingletonMonobehaviour<CEffectManager>
{
	// 이벤트별 이펙트 객체.
	Dictionary<CARD_EVENT_TYPE, GameObject> effects;

	GameObject dust;


	void Awake()
	{
		this.effects = new Dictionary<CARD_EVENT_TYPE, GameObject>();
	}


	public void load_effects()
	{
		load_effect(CARD_EVENT_TYPE.KISS, "ef_kiss");
		load_effect(CARD_EVENT_TYPE.BOMB, "ef_explosion");
		load_effect(CARD_EVENT_TYPE.PPUK, "ef_ppuk");
		load_effect(CARD_EVENT_TYPE.CLEAN, "ef_clean");
		load_effect(CARD_EVENT_TYPE.DDADAK, "ef_ddadak");

		this.dust = GameObject.Instantiate(Resources.Load("effects/ef_dust")) as GameObject;
		this.dust.SetActive(false);
	}


	void load_effect(CARD_EVENT_TYPE event_type, string effect_name)
	{
		if (this.effects.ContainsKey(event_type))
		{
			Debug.LogError(string.Format("Already added this effect.  event type {0}, effect name {1}",
				event_type, effect_name));
		}

		GameObject obj = GameObject.Instantiate(Resources.Load("effects/" + effect_name)) as GameObject;
		obj.SetActive(false);
		this.effects.Add(event_type, obj);
	}


	public void play(CARD_EVENT_TYPE event_type)
	{
		if (!this.effects.ContainsKey(event_type))
		{
			return;
		}

		this.effects[event_type].SetActive(true);
	}


	public void play_dust(Vector3 position, float delay, bool is_big)
	{
		StopAllCoroutines();
		GameObject target = this.dust;

		target.SetActive(false);
		target.transform.position = position;
		StartCoroutine(run_dust_effect(target, delay));
	}


	IEnumerator run_dust_effect(GameObject obj, float delay)
	{
		yield return new WaitForSeconds(delay);

		obj.SetActive(true);
	}
}
