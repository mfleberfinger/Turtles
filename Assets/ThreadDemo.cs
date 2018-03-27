using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ThreadDemo : MonoBehaviour
{
	Renderer rend;
	Texture2D tex2D;

	int[,] gameWorld; //Keeps track of the paths laid down by each thread/turtle. Read and written by all threads.

	int xMax, yMax; //dimensions of the array/playfield

	int xMaxTex, yMaxTex; //dimensions of the texture
	int texOffset; //space between the game world's borders and the edges of the texture (on each side)

	List<Color> colors; //keeps track of the color used to represent each thread

	void Start()
	{
		xMax = 500;
		yMax = 500;
		texOffset = 50;

		xMaxTex = xMax + texOffset * 2;
		yMaxTex = yMax + texOffset * 2;
		rend = GetComponent<Renderer>();
		tex2D = new Texture2D(xMaxTex, yMaxTex);
		colors = new List<Color>();

		rend.material.SetTexture("_MainTex", (Texture)tex2D);

		//initialize the array and represent empty cells with -1
		gameWorld = new int[xMax, yMax];
		for(int i = 0; i < xMax; i++)
			for(int j = 0; j < yMax; j++)
				gameWorld[i, j] = -1;

		//Make a solid white texture.
		for(int i = 0; i < xMaxTex; i++)
			for(int j = 0; j < yMaxTex; j++)
				tex2D.SetPixel(i, j, Color.white);

		//Draw the map's border on the texture
		for(int i = texOffset - 1; i < xMax + texOffset; i++)
		{
			tex2D.SetPixel(i, yMax + texOffset, Color.black);
			tex2D.SetPixel(i, texOffset - 1, Color.black);
		}
		for(int i = texOffset - 1; i < yMax + texOffset; i++)
		{
			tex2D.SetPixel(xMax + texOffset, i, Color.black);
			tex2D.SetPixel(texOffset - 1, i, Color.black);
		}

		tex2D.Apply();
	}
	

	void Update()
	{
		if(Input.GetKey(KeyCode.Space))
			AddTurtle();

		UpdateCanvas();
	}

	//Spawn a thread and place a "turtle" in the game world under the new
	// thread's control, if a cell is available in gameWorld.
	private void AddTurtle()
	{
		int x;
		int y;
		Turtle iLikeTortles = null;

		if(GetStartingCell(out x, out y))
		{
			Dir d = (Dir)Random.Range(0, 5);
			iLikeTortles = new Turtle(colors.Count, x, y, d, ref gameWorld, xMax, yMax);
			colors.Add(GetColor());
			new Thread(iLikeTortles.Run).Start();
		}
	}

	//return a random color, but not too light.
	private Color GetColor()
	{
		return Random.ColorHSV(0, 1, 0.5f, 1, 0, 0.5f, 1, 1);
	}

	//Make sure the turtle is placed in an unoccupied cell.
	//If we can't quickly find an unnocupied cell randomly, then search
	//	exhaustively and give up on spawning the new thread if the gameWorld
	//	is completely filled.
	//Return true on success and false on failure.
	private bool GetStartingCell(out int x, out int y)
	{
		int count = 0;

		x = Random.Range(0, xMax - 1);
		y = Random.Range(0, yMax - 1);

		//See if we can find a cell easily.
		while(gameWorld[x, y] != -1 && count < 20)
		{
			x = Random.Range(0, xMax - 1);
			y = Random.Range(0, yMax - 1);
			count++;
		}

		//If we haven't found a cell, search exhaustively.
		if(gameWorld[x, y] != -1)
		{
			x = y = 0;

			while(x < xMax && gameWorld[x, y] != -1)
			{
				while(y < yMax && gameWorld[x, y] != -1)
					y++;

				if(y >= yMax || gameWorld[x, y] != -1)
				{
					x++;
					y = 0;
				}
			}
		}

		//give up
		if(x >= xMax || y >= yMax || gameWorld[x, y] != -1)
			return false;

		return true;
	}

	//place some colored objects in the gameWorld to test the UpdateCanvas() function
	private void TestFunc()
	{
		float r, g, b;
		int x, y;

		//set some random colors and add some blobs to the game world
		for(int i = 0; i < 1000; i++)
		{
			r = Random.value;
			g = Random.value;
			b = Random.value;

			//We have a white background, so make sure the color isn't too light.
			while(r > 0.9 && g > 0.9 && b > 0.9)
			{
				r = Random.value;
				g = Random.value;
				b = Random.value;
			}

			colors.Add(new Color(r, g, b));

			//place some blobs... this should result in randomly placed 10x10 blocks of random color
			for(int j = 0; j < 10; j++)
			{
				x = Random.Range(0, xMax);
				y = Random.Range(0, yMax);

				for(int ji = (int)Mathf.Clamp((float)(x - 5), 0f, (float)xMax); ji < (int)Mathf.Clamp((float)(x + 5), 0f, (float)(xMax)); ji++)
					for(int jj = (int)Mathf.Clamp((float)(y - 5), 0f, (float)yMax); jj < (int)Mathf.Clamp((float)(y + 5), 0f, (float)yMax); jj++)
						gameWorld[ji, jj] = i;
			}
		}
	}

	//Draw the trails and any symbols that might be added.
	private void UpdateCanvas()
	{
		//draw symbols
		//[code to draw symbols]

		//draw trails over symbols
		for(int i = 0; i < xMax; i++)
			for(int j = 0; j < yMax; j++)
				if(gameWorld[i , j] > -1)
					tex2D.SetPixel(i + texOffset, j + texOffset, colors[gameWorld[i, j]]);

		tex2D.Apply();
	}
}