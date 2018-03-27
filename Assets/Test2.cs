using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
	Renderer rend;
	Texture2D tex2D;
	int x, y;


	// Use this for initialization
	void Start()
	{
		x = y = 0;

		rend = GetComponent<Renderer>();

		tex2D = new Texture2D(800, 800);

		for(int i = 0; i < 800; i++)
			for(int j = 0; j < 800; j++)
				tex2D.SetPixel(i, j, Color.white);



		tex2D.Apply();

		rend.material.SetTexture("_MainTex", (Texture)tex2D);
	}



	// Update is called once per frame
	void Update()
	{
		tex2D.SetPixel(x, y, Color.black);

		tex2D.Apply();

		rend.material.SetTexture("_MainTex", (Texture)tex2D);

		x++;
		if(x > 800)
		{
			x = 0;
			y++;
		}
	}
}
