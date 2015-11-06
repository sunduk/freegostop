using System.Collections;

public class CSpriteLayerOrderManager : CSingleton<CSpriteLayerOrderManager>
{
	int order;
	public int Order
	{
		get
		{
			return ++order;
		}
	}


	public void reset()
	{
		this.order = 0;
	}
}
