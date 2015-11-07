using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class CUIImageFadeEffect : MonoBehaviour {

	[SerializeField]
	float delay;

	[SerializeField]
	float duration;

	Image image;


	void Awake()
	{
		this.image = gameObject.GetComponent<Image>();
	}


	void OnEnable()
	{
		StopAllCoroutines();
		StartCoroutine(fade_in());
	}


	IEnumerator fade_in()
	{
		float from = 1.0f;
		float to = 0.0f;

		Color color = this.image.color;
		color.a = from;
		this.image.color = color;

		yield return new WaitForSeconds(this.delay);
		float begin_time = Time.time;

		while (true)
		{
			float val = EasingUtil.linear(from, to, (Time.time - begin_time) / this.duration);
			color.a = val;
			this.image.color = color;

			if (val <= 0.0f)
			{
				break;
			}

			yield return 0;
		}
	}
}
