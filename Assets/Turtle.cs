using System.Collections.Generic;
using System.Threading;
using System;

public class Turtle
{
	int ID, x, y, xMax, yMax;
	Dir d;
	int[,] world;
	Random rand;
	int moveCount;

	//shuffled to exhaustively find a direction without giving preference to any
	//	particular direction
	Dir[] dirArr = { Dir.up, Dir.left, Dir.down, Dir.right };

	public Turtle(int ID, int x, int y, Dir d, ref int[,] world, int xMax, int yMax)
	{
		this.ID = ID;
		this.x = x;
		this.y = y;
		this.d = d;
		this.world = world;
		this.xMax = xMax;
		this.yMax = yMax;

		//use system random number generator because Unity's is not thread safe.
		rand = new Random(DateTime.Now.GetHashCode());

		moveCount = 0;
	}

	public void Run()
	{
		bool run = true;

		while(run)
		{
			//Randomize direction
			if(moveCount % 1000 == 0)
				d = (Dir)rand.Next(0, 5);

			//Check for obstacles in current direction.
			//If safe, move forward.
			//Otherwise, check each direction and choose the first one that
			//	doesn't result in collision.
			if(NextIsClear(d))
			{
				MoveForward();
				moveCount++;
			}
			else if(SetSafeDirection())
			{
				MoveForward();
				moveCount++;
			}
			else
				run = false; //If there are no safe moves remaining, terminate.

			//Sleep so we don't fill the array in an instant.
			Thread.Sleep(3);
		}
	}

	//Try each direction in random order until a safe direction is found, or
	//	all options are exhausted.
	//Return true on success and false on failure.
	private bool SetSafeDirection()
	{
		//randomize the array of directions
		for(int i = 0; i < 4; i++)
			Swap(ref dirArr[i], ref dirArr[rand.Next(0, 4)]);

		int count = 0;
		do
		{
			d = dirArr[count];
			count++;
		} while(count < 5 && !NextIsClear(d));

		return NextIsClear(d);
	}

	private void Swap(ref Dir a, ref Dir b)
	{
		Dir tmp = a;
		a = b;
		b = tmp;
	}

	//Based on current direction, return true if the next cell is open and false
	//	if the next cell is occupied.
	private bool NextIsClear(Dir d)
	{
		switch(d)
		{
			case Dir.up:
				if(y + 1 < yMax && world[x, y + 1] == -1)
					return true;
				else
					return false;
			case Dir.left:
				if(x - 1 >= 0 && world[x - 1, y] == -1)
					return true;
				else
					return false;
			case Dir.down:
				if(y - 1 >= 0 && world[x, y - 1] == -1)
					return true;
				else
					return false;
			case Dir.right:
				if(x + 1 < xMax && world[x + 1, y] == -1)
					return true;
				else
					return false;
		}

		return false;
	}

	//Moves into the next cell according to direction (the variable d).
	//Does not check array bounds or locks. It is assumed that has been done by
	//	the caller.
	private void MoveForward()
	{
		switch(d)
		{
			case Dir.up:
				y++;
				break;
			case Dir.left:
				x--;
				break;
			case Dir.down:
				y--;
				break;
			case Dir.right:
				x++;
				break;
		}

		world[x, y] = ID;
	}

	//Attempt to lock a cell(?) in the array.
	//Return true on success an false on failure.
	private bool GetCellLock(int x, int y)
	{
		return false;
	}
}

public enum Dir
{up, left, down, right};