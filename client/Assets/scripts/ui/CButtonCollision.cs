using UnityEngine;
using System.Collections;

public class CButtonCollision : MonoBehaviour {

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit))
			{
				GameObject obj = hit.transform.gameObject;
				CUIButton button = obj.GetComponent<CUIButton>();
				if (button != null)
				{
					button.on_touch();
				}
			}
		}
	}
}
