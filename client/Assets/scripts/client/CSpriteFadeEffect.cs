using UnityEngine;
using System.Collections;

public class CSpriteFadeEffect : MonoBehaviour {

	[SerializeField]
	float duration;

	SpriteRenderer sprite_renderer;


	void Awake()
	{
		this.sprite_renderer = gameObject.GetComponent<SpriteRenderer>();
	}


	void OnEnable()
	{
		StopAllCoroutines();
		StartCoroutine(blink());
	}


	IEnumerator blink()
	{
		float begin_time = Time.time;
		Color color = this.sprite_renderer.color;

		float from = 0.0f;
		float to = 1.0f;

		while (true)
		{
			float val = EasingUtil.linear(from, to, (Time.time - begin_time) / this.duration);
			color.a = val;
			this.sprite_renderer.color = color;

			if (val >= 1.0f)
			{
				from = 1.0f;
				to = 0.0f;
				begin_time = Time.time;
			}
			else if (val <= 0.0f)
			{
				from = 0.0f;
				to = 1.0f;
				begin_time = Time.time;
			}

			yield return 0;
		}
	}
}
