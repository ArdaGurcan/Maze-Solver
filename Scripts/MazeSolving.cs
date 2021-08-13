//using UnityEngine;

// public class MazeSolving : MonoBehaviour
// {
    /// <summary>
    /// First explore the whole maze using trémaux algorithm.
    /// When the goal is found find shortest path to start using floodfill.
    /// At every step, look at the surrounding obstacles, write them to the grid and find path again using floodfill.
    /// When reached start, do the same to reach the goal.
    /// -Arda
    /// Yorumlara bakarak daha iyi anlayabilirsin ne oldugunu
    /// </summary>

    [SerializeField]//for debugging
    int currentIndex = 1;
    Vector2 robotPosition = new Vector2(22, 0);
    int[,] grid = new int[23, 23]; // stores each wall,available slot,and wall corners
    [SerializeField]
    int state; // which step is currently being executed
    bool backtracking;
    [SerializeField]
    Vector2 goalPosition;
    int[,] solveGrid = new int[23, 23];
    int[,] returnGrid = new int[23, 23];
    public LayerMask mask;
    public GameObject box;
    public Transform canvas;
    int smallest = 145;
    [SerializeField]
    public Vector2 destination = new Vector2(22, 0);

	private void Start()
	{
        // initialize grid with wall corners as -1's
        for (int i = 0; i < 23; i++)
        {
            for (int j = 0; j < 23; j++)
            {
                if(i * j % 2 != 0)
                {
                    grid[i, j] = -1;
                }
            }
        }
        InvokeRepeating("Solve", 0, 0.1f);
	}

	private void Update()
	{
        // show the rays cast
        Debug.DrawRay(transform.position, transform.forward);
        Debug.DrawRay(transform.position, transform.right);
        Debug.DrawRay(transform.position, -transform.forward);
        Debug.DrawRay(transform.position, -transform.right);
	}

	void Solve() // switch between each step according to state variable
    {
        switch(state)
        {
            case 0:
                Step0();
                break;
            case 1:
                Step1();
                break;
            case 2:
                Step2();
                break;
            case 3:
                Step3();
                break;
        }
    }

    public void Step0()// explore the whole maze until goal is found
    {
        //write current index to the robot position in grid
        grid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y)] = currentIndex;

        //write any seen walls to grid
        if (!IsEmpty(0) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) - new Vector2(0, 1)))
        {
            grid[Mathf.RoundToInt(robotPosition.x) - 1, Mathf.RoundToInt(robotPosition.y)] = -1;
        }
        if (!IsEmpty(1) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) + new Vector2(1, 0)))
        {
            print("right sensor");
            grid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 1] = -1;
        }
        if (!IsEmpty(2) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) + new Vector2(0, 1)))
        {
            grid[Mathf.RoundToInt(robotPosition.x) + 1, Mathf.RoundToInt(robotPosition.y)] = -1;
        }
        if (!IsEmpty(3) && IsInGrid(new Vector2(robotPosition.y,robotPosition.x) + new Vector2(-1, 0)))
        {
            print("left sensor");
            grid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 1] = -1;
        }

        //move robot
        if(!backtracking)
        {
            //if there isn't a wall and it hasn't been numbered 
            if (IsEmpty(0) && !IsNumbered(0))
            {
                print("moving forward");
                Move(0);
                currentIndex++;
            }
            else if (IsEmpty(1) && !IsNumbered(1))
            {
                print("moving right");
                Move(1);
                currentIndex++;
            }
            else if (IsEmpty(3) && !IsNumbered(3))
            {
                print("moving left");
                Move(3);
                currentIndex++;
            }
            else if (IsEmpty(2) && !IsNumbered(2))
            {
                print("moving back");
                Move(2);
                currentIndex++;
            }
            else
            {
                print("stuck");
                backtracking = true;
            }

        }
        else//if stuck
        {
            //backtrack

            if (IsEmpty(0) && !IsNumbered(0)) // if the slot is empty
            {
                print("moving forward1");
                Move(0);
                backtracking = false;
            }
            else if (IsEmpty(1) && !IsNumbered(1))
            {
                print("moving right1");
                Move(1);
                backtracking = false;
                currentIndex++;
            }
            else if (IsEmpty(3) && !IsNumbered(3))
            {
                print("moving left1");
                Move(3);
                backtracking = false;
                currentIndex++;
            }
            else if (IsEmpty(2) && !IsNumbered(2))
            {
                print("moving back1");
                Move(2);
                backtracking = false;
                currentIndex++;
            }
            else if (IsEmpty(0) && Number(0) == currentIndex - 1) // if still stuck but backtracking check if 1 lower than current index
            {
                print("moving forward2");
                Move(0);
                currentIndex--;
            }
            else if (IsEmpty(1) && Number(1) == currentIndex - 1)
            {
                print("moving right2");
                Move(1);
                currentIndex--;
            }
            else if (IsEmpty(3) && Number(3) == currentIndex - 1)
            {
                print("moving left2");
                Move(3);
                currentIndex--;
            }
            else if (IsEmpty(2) && Number(2) == currentIndex - 1)
            {
                print("moving back2");
                Move(2);
                currentIndex--;
            }
            else
            {
                
            }
        }

        //print grid with robot position
        ////PrintGrid(grid);

    }

    public void Step1()
    {
        //go back to start while discovering shortest path
        if (!IsEmpty(0) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) - new Vector2(0, 1)))//
        {
            print("up sensor");
            grid[Mathf.RoundToInt(robotPosition.x) - 1, Mathf.RoundToInt(robotPosition.y)] = -1;
        }
        if (!IsEmpty(1) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) + new Vector2(1, 0)))
        {
            print("right sensor");
            grid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 1] = -1;
        }
        if (!IsEmpty(2) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) + new Vector2(0, 1)))
        {
            print("down sensor");
            grid[Mathf.RoundToInt(robotPosition.x) + 1, Mathf.RoundToInt(robotPosition.y)] = -1;
        }
        if (!IsEmpty(3) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) + new Vector2(-1, 0)))
        {
            print("left sensor");
            grid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 1] = -1;
        }
        FillSolveGrid();
        smallest = 145;
        if (IsEmpty(0) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y))) && solveGrid[Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y)] < smallest)
        {
            print("Up available");
            smallest = solveGrid[Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y)];
        }
        if (IsEmpty(1) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2)) && solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2] < smallest)
        {
            print("Right available");
            smallest = solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2];
        }
        if (IsEmpty(3) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2)) && solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2] < smallest)
        {
            print("Left available");
            smallest = solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2];
        }
        if (IsEmpty(2) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y))) && solveGrid[Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y)] < smallest)
        {
            print("Down available");
            smallest = solveGrid[Mathf.RoundToInt(robotPosition.x + 2), Mathf.RoundToInt(robotPosition.y)];
        }

        if (IsEmpty(0) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y))) && solveGrid[Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y)] == smallest)
        {
            print("Moving up");
            Move(0);
        }
        else if (IsEmpty(1) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2)) && solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2] == smallest)
        {
            print("Moving right");
            Move(1);
        }
        else if (IsEmpty(3) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2)) && solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2] == smallest)
        {
            print("Moving left");
            Move(3);
        }
        else if (IsEmpty(2) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y))) && solveGrid[Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y)] == smallest)
        {
            print("Moving down");
            Move(2);
        }



        if (robotPosition == new Vector2(22, 0))
        {
            destination = goalPosition;
            FillSolveGrid();
            state = 2;
        }
        //state = 2; //temporary
    }

    public void Step2()
    {
        //go back to goal while discovering shortest path
        if (!IsEmpty(0) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) - new Vector2(0, 1)))
        {
            print("up sensor");
            grid[Mathf.RoundToInt(robotPosition.x) - 1, Mathf.RoundToInt(robotPosition.y)] = -1;
        }
        if (!IsEmpty(1) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) + new Vector2(1, 0)))
        {
            print("right sensor");
            grid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 1] = -1;
        }
        if (!IsEmpty(2) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) + new Vector2(0, 1)))
        {
            print("down sensor");
            grid[Mathf.RoundToInt(robotPosition.x) + 1, Mathf.RoundToInt(robotPosition.y)] = -1;
        }
        if (!IsEmpty(3) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) + new Vector2(-1, 0)))
        {
            print("left sensor");
            grid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 1] = -1;
        }
        FillSolveGrid();
        smallest = 145;
        if (IsEmpty(0) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y))) && solveGrid[Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y)] < smallest)
        {
            print("Up available");
            smallest = solveGrid[Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y)];
        }
        if (IsEmpty(1) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2)) && solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2] < smallest)
        {
            print("Right available");
            smallest = solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2];
        }
        if (IsEmpty(3) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2)) && solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2] < smallest)
        {
            print("Left available");
            smallest = solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2];
        }
        if (IsEmpty(2) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y))) && solveGrid[Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y)] < smallest)
        {
            print("Down available");
            smallest = solveGrid[Mathf.RoundToInt(robotPosition.x + 2), Mathf.RoundToInt(robotPosition.y)];
        }

        if (IsEmpty(0) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y))) && solveGrid[Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y)] == smallest)
        {
            print("Moving up");
            Move(0);
        }
        else if (IsEmpty(1) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2)) && solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2] == smallest)
        {
            print("Moving right");
            Move(1);
        }
        else if (IsEmpty(3) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2)) && solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2] == smallest)
        {
            print("Moving left");
            Move(3);
        }
        else if (IsEmpty(2) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y))) && solveGrid[Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y)] == smallest)
        {
            print("Moving down");
            Move(2);
        }

        if (robotPosition == goalPosition)
        {
            destination = new Vector2(22, 0);
            FillSolveGrid();
            state = 1;
        }

    }

    public void Step3()// unused
    {
        print("1");
        //go back to start using shortest path
        smallest = 145;
        if (IsEmpty(0) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y))) && solveGrid[Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y)] < smallest)
        {
            smallest = solveGrid[Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y)];
        }
        if (IsEmpty(1) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2)) && solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2] < smallest)
        {
            smallest = solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2];
        }
        if (IsEmpty(3) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2)) && solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2] < smallest)
        {
            smallest = solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2];
        }
        if (IsEmpty(2) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y))) && solveGrid[Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y)] < smallest)
        {
            smallest = solveGrid[Mathf.RoundToInt(robotPosition.x + 2), Mathf.RoundToInt(robotPosition.y)];
        }

        if (IsEmpty(0) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y))) && solveGrid[Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y)] == smallest)
        {
            Move(0);
        }
        if (IsEmpty(1) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2)) && solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2] == smallest)
        {
            Move(1);
        }
        if (IsEmpty(3) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2)) && solveGrid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2] == smallest)
        {
            Move(3);
        }
        if (IsEmpty(2) && IsInGrid(new Vector2(Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y))) && solveGrid[Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y)] == smallest)
        {
            Move(2);
        }

        if (robotPosition == new Vector2(22, 0))
        {
            destination = goalPosition;
            FillSolveGrid();
            state = 2;
        }
    }

    public bool IsEmpty(int direction) // check if there is a wall in direction
    {
        switch (direction)
        {
            case 0://front
                if (Physics.Raycast(transform.position, transform.forward, 1f, mask))// sends a ray in direction to check if hits and objects
                {
                    return false;
                }
                break;
            case 1://right
                if (Physics.Raycast(transform.position, transform.right, 1f, mask))
                {
                    return false;
                }
                break;
            case 2://back
                if (Physics.Raycast(transform.position, -transform.forward, 1f, mask))
                {
                    return false;
                }
                break;
            case 3://left
                if (Physics.Raycast(transform.position, -transform.right, 1f, mask))
                {
                    return false;
                }
                break;
        }
        return true;
    }

    public bool IsNumbered(int direction)// check if the available square in given direction has been assigned a number before
    {
        switch (direction)
        {
            case 0:
                if (grid[Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y)] == 0)
                {
                    return false;
                }
                break;
            case 1:
                if (grid[Mathf.RoundToInt(robotPosition.x) , Mathf.RoundToInt(robotPosition.y) + 2] == 0)
                {
                    return false;
                }
                break;
            case 2:
                if (grid[Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y)] == 0)
                {
                    return false;
                }
                break;
            case 3:
                if (grid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2] == 0)
                {
                    return false;
                }
                break;
        }
        return true;
    }

    public int Number(int direction) // get the number in the current square
    {
        switch (direction)
        {
            case 0:
                return grid[Mathf.RoundToInt(robotPosition.x) - 2, Mathf.RoundToInt(robotPosition.y)];
            case 1:
                return grid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 2];
            case 2:
                return grid[Mathf.RoundToInt(robotPosition.x) + 2, Mathf.RoundToInt(robotPosition.y)];
            case 3:
                return grid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 2];
        }
        return -1;
    }

    public void Move(int direction) // move robot and update grid
    {
        switch (direction)
        {
            case 0:
                transform.position += transform.forward;// 20 cm git
                robotPosition -= new Vector2(2, 0);
                break;
            case 1:
                transform.position += transform.right;
                robotPosition += new Vector2(0, 2);
                break;
            case 2:
                transform.position -= transform.forward;
                robotPosition += new Vector2(2, 0);
                break;
            case 3:
                transform.position -= transform.right;
                robotPosition -= new Vector2(0, 2);
                break;
        }
    }

    public bool IsInGrid(Vector2 position) // check if position is in the bounds of the grid
    {
        if(position.x >= 0 && position.x < 23 && position.y >= 0 && position.y < 23)
        {
            return true;
        }
        return false;
    }

    public void PrintGrid(int[,] inputGrid) // print grid for debugging
    {
        
        string str = "";
        for (int y = 0; y < 23; y++)
        {
            for (int x = 0; x < 23; x++)
            {

                //Text txt = Instantiate(box, new Vector3(747 + x *40,  420 + y * -40), Quaternion.identity,canvas).GetComponent<Text>();
                if(inputGrid[y, x] == 145)
                {
                    str += "O";
                }
                else if(robotPosition == new Vector2(y,x))
                {
                    str += "X";
                    //txt.text = "X";
                }
                else
                {
                    str += inputGrid[y, x];
                    //txt.text += inputGrid[y, x];
                }
            }
            str += "\n";
        }
        print(str);
    }

    void FillSolveGrid() // fill another grid called "solveGrid" according  to original grid
    {
        solveGrid = new int[23, 23];

        for (int i = 0; i < 23; i++)
        {
            for (int j = 0; j < 23; j++)
            {
                if (grid[i, j] == -1)
                {
                    solveGrid[i, j] = -1;
                }
                else if (destination != new Vector2(i, j))
                {
                    solveGrid[i, j] = 145;
                }
                else if(destination == new Vector2(i, j))
                {
                    solveGrid[i, j] = 0;
                }
            }
        }

        for (int i = 0; i < 145; i++)
        {
            for (int y = 0; y < 23; y+= 2)
            {
                for (int x = 0; x < 23; x+= 2)
                {
                    if(solveGrid[y,x] == i)
                    {
                        if (y - 2 > -1 && y - 2 < 23 && solveGrid[y - 1, x] != -1 && solveGrid[y - 2, x] == 145)
                        {
                            solveGrid[y - 2, x] = i + 1;
                        }
                        if (y + 2 > -1 && y + 2 < 23 && solveGrid[y + 1, x] != -1 && solveGrid[y + 2, x] == 145)
                        {
                            solveGrid[y + 2, x] = i + 1;
                        }
                        if (x - 2 > -1 && x - 2 < 23 && solveGrid[y, x - 1] != -1 && solveGrid[y, x - 2] == 145)
                        {
                            solveGrid[y, x - 2] = i + 1;
                        }
                        if (x + 2 > -1 && x + 2 < 23 && solveGrid[y, x + 1] != -1 && solveGrid[y, x + 2] == 145)
                        {
                            solveGrid[y, x + 2] = i + 1;
                        }
                    }

                }
            }
        }
        PrintGrid(solveGrid);


    }

	private void OnTriggerEnter(Collider other) // detect the  goal
	{
        Debug.Log("triggered");
		if(other.tag == "Goal")
        {
            goalPosition = robotPosition;
            state = 1;
            destination = new Vector2(22, 0);
            if (!IsEmpty(0) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) - new Vector2(0, 1)))
            {
                grid[Mathf.RoundToInt(robotPosition.x) - 1, Mathf.RoundToInt(robotPosition.y)] = -1;
            }
            if (!IsEmpty(1) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) + new Vector2(1, 0)))
            {
                print("right sensor");
                grid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) + 1] = -1;
            }
            if (!IsEmpty(2) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) + new Vector2(0, 1)))
            {
                grid[Mathf.RoundToInt(robotPosition.x) + 1, Mathf.RoundToInt(robotPosition.y)] = -1;
            }
            if (!IsEmpty(3) && IsInGrid(new Vector2(robotPosition.y, robotPosition.x) + new Vector2(-1, 0)))
            {
                print("left sensor");
                grid[Mathf.RoundToInt(robotPosition.x), Mathf.RoundToInt(robotPosition.y) - 1] = -1;
            }
            FillSolveGrid();
        }
	}
}
