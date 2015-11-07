using UnityEngine;
using System.Collections;

public class CScrollingEffect : MonoBehaviour {

	[SerializeField]
	Vector3 begin;

	[SerializeField]
	Vector3 to;

	[SerializeField]
	float duration;


	void OnEnable()
	{
		StopAllCoroutines();
		StartCoroutine(scroll());
	}


	IEnumerator scroll()
	{
		float begin_time = Time.time;

		float from = this.begin.x;
		float to = this.to.x;

		Transform tr = gameObject.transform;

		while (true)
		{
			float val = EasingUtil.linear(from, to, (Time.time - begin_time) / this.duration);
			Vector3 pos = transform.position;
			pos.x = val;
			transform.position = pos;

			if (Time.time - begin_time >= this.duration)
			{
				from = this.begin.x;
				to = this.to.x;
				begin_time = Time.time;
			}

			yield return 0;
		}
	}
}
