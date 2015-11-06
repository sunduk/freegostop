using UnityEngine;
using System.Collections;

public class CMovingObject : MonoBehaviour {

	public Vector3 begin;
	public Vector3 to;
	public float duration = 0.1f;

	SpriteRenderer sprite_renderer;

	void Awake()
	{
		this.sprite_renderer = gameObject.GetComponentInChildren<SpriteRenderer>();
	}


	public void run()
	{
		StopAllCoroutines();
		StartCoroutine(run_moving());
	}


	IEnumerator run_moving()
	{
		this.sprite_renderer.sortingOrder = CSpriteLayerOrderManager.Instance.Order;

		float begin_time = Time.time;
		while (Time.time - begin_time <= duration)
		{
			float t = (Time.time - begin_time) / duration;

			float x = EasingUtil.easeInExpo(begin.x, to.x, t);
			float y = EasingUtil.easeInExpo(begin.y, to.y, t);
			transform.position = new Vector3(x, y, begin.z);

			yield return 0;
		}

		transform.position = to;
	}
}
