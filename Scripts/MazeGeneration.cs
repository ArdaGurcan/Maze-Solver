using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneration : MonoBehaviour
{
    public GameObject wall;
    public int[,] walls = new int[12,12];
    public int seed;
    public bool randomSeed = true;
    public bool generate;
    public Vector2 goalPosition;
    public GameObject goal;

    public void Generate()
    {
        //Random.seed = seed;
        goalPosition = new Vector2(Random.Range(0, 12), Random.Range(0, 12));
        for (int x = 0; x < 12; x++)
        {
            for (int y = 0; y < 12; y++)
            {
                
                if(Random.Range(0,2) == 0 || (x == 0 && y == 0))
                {
                    walls[x, y] = 0;
                }
                else
                {
                    
                        Instantiate(wall, new Vector3(x, 0, y + 0.5f), Quaternion.Euler(new Vector3(0, 90, 0)), transform);


                        Instantiate(wall, new Vector3(x + 0.5f, 0, y), Quaternion.Euler(new Vector3(0, 0, 0)), transform);



                    walls[x, y] = 1;
                }


                //Random.seed = Random.Range(0,10000);
                //print(walls[x, y]);
            }
        }
        Instantiate(goal, new Vector3(goalPosition.x, 0, goalPosition.y),Quaternion.identity);
    }

    void Start()
    {
        Generate();

    }

    void Update()
    {
        if(generate)
        {
            if(randomSeed)
            {
                //seed = Mathf.RoundToInt(Mathf.Sin(Time.frameCount) * 1000) + 1000;
            }
            Generate();
            generate = false;
        }
    }
}
