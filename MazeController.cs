using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour {

	public int[,] map;
	
	public Vector2 startPosition;
	public Vector2 endPosition;

	public GameObject player;

	GameObject closestStartNode;

	public GameObject pathPrefab;
	public GameObject pathPrefab2;
	public GameObject pathPrefab3;
	public GameObject endPrefab;
		
	public GeneticAlgorithm geneticAlgorithm;
	public List<int> fittestDirections;
	public List <GameObject> pathTiles;

	public Material exitFound;

    public int distance = 0;
 
	public Vector2 Move (Vector2 position, int direction) {
		switch (direction) {
		// map[y,x == 1] -> collide w/ wall
		// North
		case 0:
			if (position.y - 1 < 0 || map [(int)(position.y - 1), (int)position.x] == 1) {
				break;
			} else {
                //position.y -= 1;

                if (map[(int)(position.y - 1), (int)position.x] == 4) {
                    GameObject pathTile = Instantiate(pathPrefab3);
                    pathTile.transform.position = new Vector3(position.x, 0, -position.y + 3);
                    pathTiles.Add(pathTile);

                    position.y -= 6;
                        distance += 4;
                    } else {
                    position.y -= 1;
                        distance += 1;
                    }
            }
			break;
		// South
		case 1:
			if (position.y + 1 >= map.GetLength (0) || map [(int)(position.y + 1), (int)position.x] == 1) {
				break;
			} else {
                //position.y += 1;
                
                if (map[(int)(position.y + 1), (int)position.x] == 4) {

                    GameObject pathTile = Instantiate(pathPrefab3);
                    pathTile.transform.position = new Vector3(position.x, 0, -position.y - 3);
                    pathTiles.Add(pathTile);

                    position.y += 6;
                        distance += 4;
                } else {
                    position.y += 1;
                        distance += 1;
                }
            }
			break;
		// East
		case 2:
			if (position.x + 1 >= map.GetLength (1) || map [(int)position.y, (int)(position.x + 1)] == 1) {
				break;
			} else {
                //position.x += 1;
                    
                if (map[(int)position.y, (int)(position.x + 1)] == 3) {

                    GameObject pathTile = Instantiate (pathPrefab2);
                    pathTile.transform.position = new Vector3 (position.x + 2, 0, -position.y);
                    //pathTile.transform.position = new Vector3 (position.x, 0, -position.y);
                    pathTiles.Add (pathTile);

                    position.x += 4;
                        distance += 3;
                } else {
                    position.x += 1;
                        distance += 1;
                    }
            }
			break;
		// West
		case 3:
			if (position.y - 1 < 0 ||map[(int)position.y, (int)(position.x - 1)] == 1) {
				break;
			} else {
                //position.x -= 1;

                    if (map[(int)position.y, (int)(position.x - 1)] == 3) {

                    	GameObject pathTile = Instantiate (pathPrefab2);
                        //pathTile.transform.position = new Vector3 (position.x, 0, -position.y);
                    	pathTile.transform.position = new Vector3 (position.x - 2, 0, -position.y);
                    	pathTiles.Add (pathTile);

                    	position.x -= 4;
                            distance += 3;
                    } else {
                    	position.x -= 1;
                            distance += 1;
                    }
                }
			break;
		}
		return position;
	}

	// calculate the fitness
	// f(x) = chromosome w/ better fitness is creature w/ end position closer to the exit

	public double TestRoute (List<int> directions) {
		Vector2 position = startPosition;

		for (int directionsIndex = 0; directionsIndex < directions.Count; directionsIndex ++) {
			int nextDirection = directions [directionsIndex];
			position = Move (position, nextDirection);
		}

		Vector2 deltaPosition = new Vector2 (Mathf.Abs (position.x - endPosition.x),
											 Mathf.Abs (position.y - endPosition.y));
		
		// (14-25),(-1-26) = 11+27 +1 = 1/39
		// (25-25),(-2-26) = 0+28 +1 = 1/29
		// (25-25),(26-26) = 0+0 +1 = 1/1)

		double result = 1 / (double)(deltaPosition.x + deltaPosition.y + 1);

        //Debug.Log("Fitness =" + result +"");

		if (result == 1) {
			Debug.Log ("TestRoute result ="+ result +",("+ position.x +","+ position.y +")");
            Debug.Log("Distance ="+ distance +".");
		}
        distance = 0;
		return result;
	}

	// updating startPosition when algorithm stopped
	public GameObject FindClosestStartNode () {
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag ("Node");
		GameObject closest = null;
		float distance = Mathf.Infinity;

		Vector3 playerPos = player.transform.position;
		foreach (GameObject go in gos) {
			Vector3 diff = go.transform.position - playerPos;
			float currentDistance = diff.sqrMagnitude;
			if (currentDistance < distance) {
				closest = go;
				distance = currentDistance;
			}
		}
		return closest;
	}


	void Start () {

        map = new int[,] {
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            {1,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,1},
            {1,4,1,1,1,2,1,1,1,2,1,1,1,4,1,1,1,2,1,1,1,2,1,1,1,4,1},
            {1,4,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,1},
            {1,4,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,1},
            {1,4,1,1,1,2,1,1,1,2,1,1,1,4,1,1,1,2,1,1,1,2,1,1,1,4,1},
            {1,4,1,1,1,2,1,1,1,2,1,1,1,4,1,1,1,2,1,1,1,2,1,1,1,4,1},
            {1,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,1},
            {1,4,1,1,1,2,1,1,1,2,1,1,1,4,1,1,1,2,1,1,1,2,1,1,1,4,1},
            {1,4,1,1,1,2,1,1,1,2,1,1,1,4,1,1,1,2,1,1,1,2,1,1,1,4,1},
            {1,4,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,1},
            {1,4,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,1},
            {1,4,1,1,1,2,1,1,1,2,1,1,1,4,1,1,1,2,1,1,1,2,1,1,1,4,1},
            {1,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,1},
            {1,4,1,1,1,2,1,1,1,2,1,1,1,4,1,1,1,2,1,1,1,2,1,1,1,4,1},
            {1,4,1,1,1,2,1,1,1,2,1,1,1,4,1,1,1,2,1,1,1,2,1,1,1,4,1},
            {1,4,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,1},
            {1,4,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,1},
            {1,4,1,1,1,2,1,1,1,2,1,1,1,4,1,1,1,2,1,1,1,2,1,1,1,4,1},
            {1,2,1,1,1,2,1,1,1,2,1,1,1,2,1,1,1,2,1,1,1,2,1,1,1,2,1},
            {1,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,1},
            {1,4,1,1,1,2,1,1,1,2,1,1,1,4,1,1,1,2,1,1,1,2,1,1,1,4,1},
            {1,4,1,1,1,2,1,1,1,2,1,1,1,4,1,1,1,2,1,1,1,2,1,1,1,4,1},
            {1,4,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,1},
            {1,4,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,4,1},
            {1,4,1,1,1,2,1,1,1,2,1,1,1,4,1,1,1,2,1,1,1,2,1,1,1,4,1},
            {1,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,3,3,3,2,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
        };

        //map = new int[,] {
        //    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,1,1,1,1,2,2,1},
        //    {1,2,2,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,1,1,1,1,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,1,1,1,1,2,2,1},
        //    {1,2,2,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,1,1,1,1,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,1,1,1,1,2,2,1},
        //    {1,2,2,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,1,1,1,1,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,1,1,1,1,2,2,1},
        //    {1,2,2,1,1,1,1,1,1,1,1,1,2,2,2,1,1,1,1,1,1,1,1,1,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1},
        //    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
        //};

        // startPosition = new Vector2 (1f, 1f);
        // endPosition = new Vector2 (25f, 26f);

        startPosition = VirtualShopManager.virtualShopManager.startPos;
		endPosition = VirtualShopManager.virtualShopManager.endPos;
		fittestDirections = new List<int> ();
		pathTiles = new List<GameObject> ();

		geneticAlgorithm = new GeneticAlgorithm ();
		geneticAlgorithm.mazeController = this;
		geneticAlgorithm.Run ();
	}

	public void ClearPathTiles () {
		foreach (GameObject pathTile in pathTiles) {
			Destroy(pathTile);
		}
		pathTiles.Clear ();
	}

	public void ColorFittestChromosomePath () {
		foreach (GameObject pathTile in pathTiles) {
			pathTile.GetComponent<Renderer> ().material = exitFound;
		}
	}

	public void RenderFittestChromosomePath () {
		ClearPathTiles ();
		
		Genome fittestGenome = geneticAlgorithm.genomes[geneticAlgorithm.fittestGenome];
		List<int> fittestDirections = geneticAlgorithm.Decode (fittestGenome.bits);
		Vector2 position = startPosition;

		foreach (int direction in fittestDirections) {
			position = Move (position, direction);
			// GameObject pathTile = Instantiate (pathPrefab);
			// pathTile.transform.position = new Vector3 (position.x, 0, -position.y);
			// pathTiles.Add (pathTile);

			if (map[(int)position.y, (int)position.x] == 2) {
				if (position == VirtualShopManager.virtualShopManager.endPos) {
					GameObject pathTile = Instantiate (endPrefab);
					pathTile.transform.position = new Vector3 (position.x, 0, -position.y);
					pathTiles.Add(pathTile);
				} else {
					GameObject pathTile = Instantiate (pathPrefab);
					pathTile.transform.position = new Vector3 (position.x, 0, -position.y);
					pathTiles.Add (pathTile);
				}
			}

			if (!geneticAlgorithm.busy)
				ColorFittestChromosomePath ();
			
			// if (map[(int)position.y, (int)position.x] == 3) {
			// 	GameObject pathTile = Instantiate (pathPrefab2);
			// 	pathTile.transform.position = new Vector3 (position.x, 0, -position.y);
			// 	pathTiles.Add (pathTile);
			// }
			// if (map[(int)position.y, (int)position.x] == 4) {
			// 	GameObject pathTile = Instantiate (pathPrefab3);
			// 	pathTile.transform.position = new Vector3 (position.x, 0, -position.y);
			// 	pathTiles.Add (pathTile);
			// }
		}
	}

	void Update () {

        

        if (geneticAlgorithm.busy && VirtualShopManager.virtualShopManager.search){
            //time += Time.deltaTime;
            //if (time == 10f)
            //{
            //    Debug.Log("Fitness at 10s =" + result + " ");
            //}
            geneticAlgorithm.Epoch ();
			RenderFittestChromosomePath ();
		}
        //if (!geneticAlgorithm.busy)
        //{
        //    closestStartNode = FindClosestStartNode();
        //    startPosition = new Vector2(closestStartNode.transform.position.x, closestStartNode.transform.position.z);
        //    //(closestStartNode.transform.position.x, closestStartNode.transform.position.z);
        //    //startPosition = temp;
        //    VirtualShopManager.virtualShopManager.startPos.Set(Mathf.Abs(startPosition.x),
        //                                                       Mathf.Abs(startPosition.y));


        //}

    }
}