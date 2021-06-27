using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuilder : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] public GameObject raycatcherPrefab;
    [SerializeField] public List<GameObject> buildingPrefabs;

    [Header("Procedural generation settings")]
    [SerializeField] public float mapSizeX = 201f;
    [SerializeField] public float mapSizeY = 150f;
    [SerializeField] public float buildingSeparation = 3f;
    [SerializeField] public int numberOfPaths = 20;
    [SerializeField] public float maxPathLength = 89f;
    [SerializeField] public float minPathLength = 21f;
    [SerializeField] public float maxPathWidth = 15f;
    [SerializeField] public float minPathWidth = 6f;

    void Start()
    {
        StartCoroutine(GenerateBuildings());
    }

    int DivideAndRound(float value)
    {
        value /= 3;
        float roundedFloat;
        if (value % 0.5f == 0)
            roundedFloat = Mathf.Ceil(value);
        else
            roundedFloat = Mathf.Round(value);
        if (roundedFloat < 0f)
        {
            return 0;
        }
        else
        {
            return (int)roundedFloat;
        }
    }

    bool CheckCoords(Dictionary<string, bool> grid, int x, int y)
    {
        try
        {
            if (grid[x.ToString() + " " + y.ToString()])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (KeyNotFoundException)
        {
            return true;
        }
    }

    string cardinalTranslate(string origin, string dir, int extendForward, int extendRight)
    {
        List<string> originCoords = new List<string>(origin.Split(' '));
        int originX = int.Parse(originCoords[0]);
        int originY = int.Parse(originCoords[1]);
        switch (dir)
        {
            case "n":
                originX += extendRight;
                originY -= extendForward;
                break;
            case "e":
                originX += extendForward;
                originY += extendRight;
                break;
            case "s":
                originX -= extendRight;
                originY += extendForward;
                break;
            case "w":
                originX -= extendForward;
                originY -= extendRight;
                break;
        }
        return originX.ToString() + " " + originY.ToString();
    }

    IEnumerator GenerateBuildings()
    {
        float startTime = Time.realtimeSinceStartup;
        int gridX = DivideAndRound(mapSizeX);
        int gridY = DivideAndRound(mapSizeY);
        int gap = DivideAndRound(buildingSeparation);
        int maxPL = DivideAndRound(maxPathLength - (gap * 6));
        int minPL = DivideAndRound(minPathLength - (gap * 6));
        int maxPW = DivideAndRound((maxPathWidth - (gap * 6) - 1) / 2) * 2 + 1;
        int minPW = DivideAndRound((minPathWidth - (gap * 6) - 1) / 2) * 2 + 1;
        int originX;
        int originY;
        List<int> buildingDimensions = new List<int>();
        List<int> possibilities;

        //Create list of building prefab dimensions
        List<List<int>> prefabDimensions = new List<List<int>>();
        List<List<float>> prefabOffsets = new List<List<float>>();
        Vector3 centre;
        Vector3 size;
        float minX;
        float minY;
        float maxX;
        float maxY;
        List<float> offset;
        List<int> dimensions;
        foreach (GameObject prefab in buildingPrefabs)
        {
            minX = 0f;
            minY = 0f;
            maxX = 0f;
            maxY = 0f;
            offset = new List<float>();
            dimensions = new List<int>();
            foreach (BoxCollider collider in prefab.GetComponents<BoxCollider>())
            {
                centre = collider.center;
                size = collider.size;
                if (centre.x + (size.x / 2) > maxX)
                {
                    maxX = centre.x + (size.x / 2);
                }
                if (centre.x - (size.x / 2) < minX)
                {
                    minX = centre.x - (size.x / 2);
                }
                if (centre.z + (size.z / 2) > maxY)
                {
                    maxY = centre.z + (size.z / 2);
                }
                if (centre.z - (size.z / 2) < minY)
                {
                    minY = centre.z - (size.z / 2);
                }
            }
            offset.Add(Mathf.Abs(minX));
            offset.Add(Mathf.Abs(minY));
            dimensions.Add(Mathf.CeilToInt((maxX - minX) / 3));
            dimensions.Add(Mathf.CeilToInt((maxY - minY) / 3));
            prefabOffsets.Add(offset);
            prefabDimensions.Add(dimensions);
        }

        //Create grid
        Dictionary<string, bool> grid = new Dictionary<string, bool>();
        List<List<int>> gridCells = new List<List<int>>();
        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                grid.Add(x.ToString() + " " + y.ToString(), false);
                gridCells.Insert(Random.Range(0, gridCells.Count + 1), new List<int> { x, y });
            }
        }

        //Generate paths in between buildings
        List<List<string>> corners = new List<List<string>>();
        List<string> currentCorner = new List<string>();
        int cornerEdge;
        int pathsAdded = 0;
        List<string> originCorner = new List<string>();
        int index;
        string cornerCoords;
        string testCoords;
        int pathLength;
        int pathWidth;
        int newPathWidth;
        List<string> cardinalDirections = new List<string> { "n", "e", "s", "w" };
        int offset1;
        int offset2;
        int offset3;
        string originDir;
        while (pathsAdded < numberOfPaths)
        {
            //Start a new path system at the edge if necessary
            if (corners.Count < 1)
            {
                if (Random.Range(0, 2) == 0)
                {
                    cornerEdge = (gridX - 1) * Random.Range(0, 2);
                    currentCorner.Add(cornerEdge.ToString() + " " + Random.Range(0, gridY).ToString());
                    if (cornerEdge == 0)
                    {
                        currentCorner.Add("e");
                    }
                    else
                    {
                        currentCorner.Add("w");
                    }
                }
                else
                {
                    cornerEdge = (gridY - 1) * Random.Range(0, 2);
                    currentCorner.Add(Random.Range(0, gridX).ToString() + " " + cornerEdge.ToString());
                    if (cornerEdge == 0)
                    {
                        currentCorner.Add("s");
                    }
                    else
                    {
                        currentCorner.Add("n");
                    }
                }
                currentCorner.Add(Random.Range(minPW, maxPW + 1).ToString());
                corners.Add(currentCorner);
            }
            index = Random.Range(0, corners.Count);
            originCorner = corners[index];
            corners.RemoveAt(index);
            //Record all options for new paths branching from the path I am about to make
            pathLength = Random.Range(minPL, maxPL + 1);
            pathWidth = int.Parse(originCorner[2]);
            originDir = originCorner[1];
            foreach (string dir in cardinalDirections)
            {
                if (dir == originDir)
                {
                    newPathWidth = pathWidth;
                }
                else
                {
                    newPathWidth = Random.Range(minPW, maxPW + 1);
                }
                if (!(
                    (originDir == "n" && dir == "s") ||
                    (originDir == "s" && dir == "n") ||
                    (originDir == "w" && dir == "e") ||
                    (originDir == "e" && dir == "w")
                    ))
                {
                    if
                    (
                        ((dir == "n" || dir == "s") && (originDir == "w" || originDir == "e")) ||
                        ((dir == "w" || dir == "e") && (originDir == "s" || originDir == "n"))
                    )
                    {
                        offset1 = (newPathWidth - 1) / 2;
                        offset2 = (pathWidth - 1) / 2;
                        offset3 = (pathWidth + 1) / 2;
                    }
                    else
                    {
                        offset1 = 0;
                        offset2 = 0;
                        offset3 = 1;
                    }
                    cornerCoords = cardinalTranslate(originCorner[0], originDir, pathLength - offset1 - 1, 0);
                    cornerCoords = cardinalTranslate(cornerCoords, dir, offset2, 0);
                    testCoords = cardinalTranslate(cornerCoords, dir, offset3, 0);
                    if (grid.ContainsKey(testCoords))
                    {
                        corners.Add(new List<string> { cornerCoords, dir, newPathWidth.ToString() });
                    }
                }
            }
            //Mark new path on grid
            for (int f = 0; f < pathLength; f++)
            {
                for (int r = (1 - pathWidth) / 2; r < (pathWidth + 1) / 2; r++)
                {
                    try
                    {
                        grid[cardinalTranslate(originCorner[0], originDir, f, r)] = true;
                    }
                    catch (KeyNotFoundException) { }
                }
            }
            pathsAdded++;
        }

        //Generate buildings
        Vector3 parentPosition;
        int randomHeight;
        float smoothHeight;
        float normalisedDistance;
        float maxDistance;
        GameObject raycatcher;
        BoxCollider raycatcherCollider;
        foreach (List<int> originCoords in gridCells)
        {
            possibilities = new List<int>();
            originX = originCoords[0];
            originY = originCoords[1];
            if (!(CheckCoords(grid, originX, originY)))
            {
                //Determine possibilities
                for (int i = 0; i < buildingPrefabs.Count; i++)
                {
                    possibilities.Add(i);
                    for (int x = 0; x < prefabDimensions[i][0] + gap * 2; x++)
                    {
                        for (int y = 0; y < prefabDimensions[i][1] + gap * 2; y++)
                        {
                            if (CheckCoords(grid, originX + x, originY + y))
                            {
                                possibilities.RemoveAt(possibilities.Count - 1);
                                x = prefabDimensions[i][0] + gap * 2;
                                break;
                            }
                        }
                    }
                }

                if (possibilities.Count != 0)
                {
                    index = possibilities[Random.Range(0, possibilities.Count)];
                    //Instantiate raycatcher
                    raycatcher = Instantiate(
                        raycatcherPrefab,
                        new Vector3(
                            (originX + gap - (gridX / 2f)) * 3f + prefabOffsets[index][0],
                            0,
                            (originY + gap - (gridY / 2f)) * 3f + prefabOffsets[index][1]
                            ),
                        Quaternion.identity
                        );
                    raycatcherCollider = raycatcher.GetComponent<BoxCollider>();
                    raycatcherCollider.center = new Vector3(
                        prefabDimensions[index][0] * 1.5f - prefabOffsets[index][0],
                        150,
                        prefabDimensions[index][1] * 1.5f - prefabOffsets[index][1]
                        );
                    raycatcherCollider.size = new Vector3(
                        prefabDimensions[index][0] * 3f,
                        300,
                        prefabDimensions[index][1] * 3f
                        );

                    //Instantiate building
                    Instantiate(
                        buildingPrefabs[index],
                        new Vector3(
                            (originX + gap - (gridX / 2f)) * 3f + prefabOffsets[index][0],
                            0,
                            (originY + gap - (gridY / 2f)) * 3f + prefabOffsets[index][1]
                            ),
                        Quaternion.identity,
                        raycatcher.transform
                        );

                    //Fill in grid
                    for (int x = gap; x < prefabDimensions[index][0] + gap; x++)
                    {
                        for (int y = gap; y < prefabDimensions[index][1] + gap; y++)
                        {
                            grid[(originX + x).ToString() + " " + (originY + y).ToString()] = true;
                        }
                    }
                }
            }
        }
        if (Time.realtimeSinceStartup - startTime > 0.1)
        {
            yield return null;
            startTime = Time.realtimeSinceStartup;
        }
    }
}
