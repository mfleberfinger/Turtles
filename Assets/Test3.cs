using System.Collections.Generic;
using UnityEngine;

public class Test3 : MonoBehaviour
{
	Renderer rend;
	Texture2D tex2D;
	int x, y, a;
	int[,] arr;

	// Use this for initialization
	void Start()
	{
		arr = new int[800, 800];
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
		for(int j = 0; j < 800; j++)
		{
			x = Random.Range(50, 751);
			y = Random.Range(50, 751);
			a = Random.Range(0, 2);


			arr[x, y] = a;
		}

		for(int i = 0; i < 800; i++)
			for(int j = 0; j < 800; j++)
			{
				if(arr[i, j] > 0)
					tex2D.SetPixel(i, j, Color.black);
				else
					tex2D.SetPixel(i, j, Color.white);
			}

		tex2D.Apply();
	}
}