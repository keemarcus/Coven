using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
using System.IO;
using UnityEditor;
using System.Linq;

public class TileMapGenerator : MonoBehaviour
{
    public NavMeshSurface surface;
    public string refFileName;
    private Tilemap groundLayer;
    private Tilemap wallLayer;
    private Tilemap overhangLayer;

    public Tile groundTile;
    public RuleTile wallTile;
    public Tile headerTile;
    public GameObject door;

    public TextAsset refFile;
    private int numberOfRows;
    private int numberOfColumns;
    public GridManager gridManager;
    public SpawnManager spawnManager;


    void Start()
    {
        // get all the tilemap objects
        groundLayer = this.gameObject.transform.Find("Ground").GetComponent<Tilemap>();
        if(groundLayer == null) { Debug.Log("No tilemap found for ground layer"); }
        wallLayer = this.gameObject.transform.Find("Walls").GetComponent<Tilemap>();
        if (wallLayer == null) { Debug.Log("No tilemap found for wall layer"); }
        overhangLayer = this.gameObject.transform.Find("Overhang").GetComponent<Tilemap>();
        if (wallLayer == null) { Debug.Log("No tilemap found for overhang layer"); }

        // get the grid manager
        gridManager = this.gameObject.GetComponent<GridManager>();
        spawnManager = this.gameObject.GetComponent<SpawnManager>();


        // generate the ref file
        if (!CheckForRefFile())
        {
            //GenerateRefFile(20, 20,new[] {(2, 2, 5, 5), (10, 10, 15, 15) });
            //GenerateRefFile(50, 50, GenerateRoomPositions(50,50,5,20));
            GenerateRefFile(50, 50, 3);
        }
        

        // get the ref file
        string refData = refFile.ToString();
        string[] refDataLines = refData.Split('\n');
        for(int ctr = 0; ctr < refDataLines.Length; ctr++)
        {
            //refDataLines[ctr] = refDataLines[ctr].Trim();
        }

        // determine the number of rows in the tilemap
        numberOfRows = refDataLines.Length;
        Debug.Log("Tilemap Height = " + numberOfRows);

        // determine the number of columns in the tilemap
        numberOfColumns = 0;
        foreach(string line in refDataLines)
        {
            if((line.Length) > numberOfColumns) { numberOfColumns = line.Length; }
        }
        Debug.Log("Tilemap Width = " + numberOfColumns);

        // build the tilemap
        ClearTileMap();
        spawnManager.Initialize();
        GenerateTileMap(refDataLines);

        // set the bounds of the grid
        gridManager.minBound.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        gridManager.maxBound.SetPositionAndRotation(new Vector3(numberOfColumns, numberOfRows, 0f), Quaternion.identity);

        // update the navmesh
        surface.BuildNavMesh();

        // spawn all the characters
        spawnManager.SpawnPlayers();
        spawnManager.SpawnEnemies();
    }

    private void GenerateTileMap(string[] refDataLines)
    {
        for(int ctr = 0; ctr < numberOfRows; ctr++)
        {
            GenerateRow(ctr, refDataLines[ctr]);
        }
    }

    private void GenerateRow(int rownumber, string row)
    {
        for(int ctr = 0; ctr < row.Length; ctr++)
        {            
            switch (row[ctr])
            {
                case ' ':
                    // leave this space empty
                    break;
                case 't':
                    // place a scene transition here

                    // also place a ground tile under this space
                    groundLayer.SetTile(new Vector3Int(ctr, rownumber), groundTile);
                    break;
                case '-':
                    // place a path tile in this space
                    groundLayer.SetTile(new Vector3Int(ctr, rownumber), groundTile);
                    break;
                case 'w':
                    // place a wall tile in this space
                    wallLayer.SetTile(new Vector3Int(ctr, rownumber), wallTile);
                    // also place a ground tile under this space
                    groundLayer.SetTile(new Vector3Int(ctr, rownumber), groundTile);
                    break;
                case 'g':
                    // place a ground tile in this space
                    groundLayer.SetTile(new Vector3Int(ctr, rownumber), groundTile);
                    break;
                case 'p':
                    // add a player spawn point for this space
                    spawnManager.AddPlayerSpawnPoint(new Vector2(ctr + 1, rownumber + 1));
                    // also place a ground tile under this space
                    groundLayer.SetTile(new Vector3Int(ctr, rownumber), groundTile);
                    break;
                case 'e':
                    // add an enemy spawn point for this space
                    spawnManager.AddEnemySpawnPoint(new Vector2(ctr + 1, rownumber + 1));
                    // also place a ground tile under this space
                    groundLayer.SetTile(new Vector3Int(ctr, rownumber), groundTile);
                    break;
                case 'h':
                    // place a header tile in this space
                    overhangLayer.SetTile(new Vector3Int(ctr, rownumber), headerTile);
                    // also place a ground tile under this space
                    groundLayer.SetTile(new Vector3Int(ctr, rownumber), groundTile);
                    break;
                case 'd':
                    // place a door in this space
                    Instantiate(door, new Vector3(ctr + 1, rownumber + 1.4f), Quaternion.identity);
                    // also place a ground tile under this space
                    groundLayer.SetTile(new Vector3Int(ctr, rownumber), groundTile);
                    break;
                default:
                    Debug.Log("Unknown Tile ID found at (" + ctr + "," + rownumber + ") = " + row[ctr]);
                    break;
            }
            
        }
    }

    private void ClearTileMap()
    {
        groundLayer.ClearAllTiles();
        wallLayer.ClearAllTiles();
    }

    private void GenerateRefFile(int width, int height, int numberOfSceneTransitions)
    {
        // genertate a blank map of the desired size
        string map = "";
        for(int ctr = 0; ctr < height; ctr++)
        {
            for(int ctr2 = 0; ctr2 < width; ctr2++)
            {
                map += ' ';
            }
            if (ctr < (height - 1)) { map += '\n'; }
        }
        // generate the scene transition points
        map = GenerateSceneTransitionPoints(width, height, map, numberOfSceneTransitions);
        // generate a path to each of the points
        map = GeneratePaths(width, height, map, numberOfSceneTransitions);
        // fill in the gorund around the paths
        // fill in the rest of the map with walls
        // generate the spawn points
        //Debug.Log(map);
        File.WriteAllText(AssetDatabase.GetAssetPath(refFile), map);
        EditorUtility.SetDirty(refFile);
        AssetDatabase.SaveAssetIfDirty(refFile);
        AssetDatabase.Refresh();
    }
    private string GenerateSceneTransitionPoints(int width, int height, string map, int numberOfTransitions)
    {
        Debug.Log("Generating Scene Transition Points");

        Vector2Int[] transitionPositions = new Vector2Int[numberOfTransitions];
        var xExclude = new HashSet<int>();
        var yExclude = new HashSet<int>();
        var rand = new System.Random();

        for (int ctr = 0; ctr < numberOfTransitions; ctr++)
        {
            var xRange = Enumerable.Range(1, width).Where(i => !xExclude.Contains(i));
            int xIndex = rand.Next(0, width - xExclude.Count);
            var yRange = Enumerable.Range(1, height).Where(i => !yExclude.Contains(i));
            int yIndex = rand.Next(0, height - yExclude.Count);

            int newX = xRange.ElementAt(xIndex);
            int newY = yRange.ElementAt(yIndex);

            xExclude.Add(newX);
            yExclude.Add(newY);

            transitionPositions[ctr] = new Vector2Int(newX, newY);
        }

        foreach(Vector2Int transitionPosition in transitionPositions)
        {
            Debug.Log(transitionPosition);
            string[] mapLines = map.Split('\n');
            string targetLine = mapLines[transitionPosition.y];
            targetLine = targetLine.Substring(0, transitionPosition.x) + 't' + targetLine.Substring(transitionPosition.x + 1, targetLine.Length - (transitionPosition.x + 1));
            mapLines[transitionPosition.y] = targetLine;
            map = string.Join("\n", mapLines);
        }

        Debug.Log(map);
        return map;
    }

    private string GeneratePaths(int width, int height, string map, int numberOfTransitions)
    {
        Debug.Log("Generating Paths");

        string[] mapLines = map.Split('\n');
        for(int ctr = 0; ctr < mapLines.Length; ctr++)
        {
            if (mapLines[ctr].Contains('t'))
            {
                int pathPosition = mapLines[ctr].IndexOf('t');
                string targetLine = mapLines[ctr + 1];
                targetLine = targetLine.Substring(0, pathPosition) + '-' + targetLine.Substring(pathPosition + 1, targetLine.Length - (pathPosition + 1));
                mapLines[ctr + 1] = targetLine;
                targetLine = mapLines[ctr - 1];
                targetLine = targetLine.Substring(0, pathPosition) + '-' + targetLine.Substring(pathPosition + 1, targetLine.Length - (pathPosition + 1));
                mapLines[ctr - 1] = targetLine;
            }
        }
        map = string.Join("\n", mapLines);

        Debug.Log(map);
        return map;
    }

    private void GenerateRefFile(int width, int height, (int,int,int,int)[] rooms)
    {
        string map = "";
        bool inRoom = false;
        for(int ctr = 0; ctr < height; ctr++)
        {
            inRoom = false;
            for(int ctr2 = 0; ctr2 < width; ctr2++)
            {
                foreach((int,int,int,int) room in rooms)
                {
                    if((ctr2 == room.Item1 || ctr2 == (room.Item3 + 1)) && (ctr >= room.Item2 & ctr <= room.Item4))
                    {
                        inRoom = !inRoom;
                    }
                }
                
                if(inRoom)
                {
                    map += 'g';
                }
                else
                {
                    map += 'w'; 
                }
            }
            if(ctr < (height - 1)) { map += '\n'; }
        }

        Debug.Log(map);
        File.WriteAllText(AssetDatabase.GetAssetPath(refFile), map);
        EditorUtility.SetDirty(refFile);
        AssetDatabase.SaveAssetIfDirty(refFile);
        AssetDatabase.Refresh();
    }

    private bool CheckForRefFile()
    {
        // check to see if we've already generated the ref file
        if (refFile.ToString().Equals(""))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private (int, int, int, int)[] GenerateRoomPositions(int width, int height, int numberOfRooms, int maxFailures)
    {
        List<(int, int, int, int)> rooms = new List<(int, int, int, int)>();
        int failures = 0;
        int roomsGenerated = 0;

        while (failures < maxFailures && roomsGenerated < numberOfRooms)
        {
            int startWidth = Random.Range((int) 2, width - 5);
            int startHeight = Random.Range((int) 2, height - 5);
            int endWidth = Random.Range(startWidth + 1, width - 1);
            int endHeight = Random.Range(startHeight + 1, height - 1);
            Debug.Log(startWidth + ", " + startHeight + ", " + endWidth + ", " + endHeight);
            bool failure = false;

            foreach((int, int, int, int) room in rooms)
            {
                if((startWidth >= room.Item1 && startWidth <= room.Item3) || (endWidth >= room.Item1 && endWidth <= room.Item3))
                {
                    if(((startHeight >= room.Item2 && startHeight <= room.Item4) || (endHeight >= room.Item2 && endHeight <= room.Item4)) || (startHeight <= room.Item2 && endHeight >= room.Item4))
                    {
                        failure = true;
                        break;
                    }
                }

                if ((startHeight >= room.Item2 && startHeight <= room.Item4) || (endHeight >= room.Item2 && endHeight <= room.Item4))
                {
                    if (((startWidth >= room.Item1 && startWidth <= room.Item3) || (endWidth >= room.Item1 && endWidth <= room.Item3)) || (startWidth <= room.Item1 && endWidth >= room.Item3))
                    {
                        failure = true;
                        break;
                    }
                }

                //if(((startWidth >= room.Item1 && startWidth <= room.Item3) || (endWidth >= room.Item1 && endWidth <= room.Item3) ) &&
                //    (((startHeight >= room.Item2 && startHeight <= room.Item4) || (endHeight >= room.Item2 && endHeight <= room.Item4)) || 
                //    (startHeight <= room.Item2 && endHeight >= room.Item4)))
                //{
                //    failure = true;
                //    break;
                //}
            }

            if(failure)
            {
                failures ++;
                Debug.Log("FAIL");
                continue;
            }
            else
            {
                rooms.Add((startWidth, startHeight, endWidth, endHeight));
                roomsGenerated++;
                Debug.Log("SUCCESS");
            }
        }

        return rooms.ToArray();
    }
}
