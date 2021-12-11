using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{

    public GameObject buildingSegment;
    public GameObject buildingParent;

    //Procedural generation settings
    public float mapSizeX = 201f;
    public float mapSizeY = 150f;
    public float maxLength = 54f;
    public float minLength = 15f;
    public float maxWidth = 36f;
    public float minWidth = 9f;
    public float maxHeight = 54f;
    public float minHeight = 9f;
    public float heightRandomness = 0.5f;
    public float buildingSeparation = 3f;
    public int numberOfPaths = 20;
    public float maxPathLength = 89f;
    public float minPathLength = 21f;
    public float maxPathWidth = 15f;
    public float minPathWidth = 6f;

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
        int maxBuildingLength = DivideAndRound(maxLength);
        int maxBuildingWidth = DivideAndRound(maxWidth);
        int minBuildingLength = DivideAndRound(minLength);
        int minBuildingWidth = DivideAndRound(minWidth);
        int maxBuildingHeight = DivideAndRound(maxHeight);
        int minBuildingHeight = DivideAndRound(minHeight);
        int gap = DivideAndRound(buildingSeparation);
        int maxPL = DivideAndRound(maxPathLength - (gap * 6));
        int minPL = DivideAndRound(minPathLength - (gap * 6));
        int maxPW = DivideAndRound((maxPathWidth - (gap * 6) - 1) / 2) * 2 + 1;
        int minPW = DivideAndRound((minPathWidth - (gap * 6) - 1) / 2) * 2 + 1;
        //Check that input values are correct
        if (heightRandomness < 0 || heightRandomness > 1)
        {
            Debug.Log("Height randomness should be set between 0 and 1");
        }
        Mathf.Clamp(heightRandomness, 0f, 1f);
        if (gap * 2 + maxBuildingLength > gridX || gap * 2 + maxBuildingWidth > gridX
            || gap * 2 + maxBuildingWidth > gridY || gap * 2 + maxBuildingLength > gridY)
        {
            Debug.Log("Building separation is unexpectedly large");
        }
        if (minBuildingWidth < 1)
        {
            Debug.Log("Building generation error: Minimum building width is less than grid step (3m)");
        }
        if (maxBuildingWidth < minBuildingWidth)
        {
            Debug.Log("Building generation error: Maximum building width is less than minimum building width");
        }
        if (minBuildingLength < 1)
        {
            Debug.Log("Building generation error: Minimum building length is less than grid step (3m)");
        }
        if (maxBuildingLength < minBuildingLength)
        {
            Debug.Log("Building generation error: Maximum building length is less than minimum building length");
        }
        if (minBuildingHeight < 1)
        {
            Debug.Log("Building generation error: Minimum building height is less than grid step (3m)");
        }
        if (maxBuildingHeight < minBuildingHeight)
        {
            Debug.Log("Building generation error: Maximum building width is less than minimum building width");
        }
        if (gridX < 1)
        {
            Debug.Log("Building generation error: Grid X is less than grid step (3m)");
        }
        if (gridY < 1)
        {
            Debug.Log("Building generation error: Grid Y is less than grid step (3m)");
        }
        if (gridX < minBuildingWidth)
        {
            Debug.Log("Building generation error: Grid X is less than minimum building width");
        }
        if (gridX < maxBuildingWidth)
        {
            Debug.Log("Building generation error: Grid X is less than maximum building width");
        }
        if (gridY < minBuildingWidth)
        {
            Debug.Log("Building generation error: Grid Y is less than minimum building width");
        }
        if (gridY < maxBuildingWidth)
        {
            Debug.Log("Building generation error: Grid Y is less than maximum building width");
        }
        if (gridX < minBuildingLength)
        {
            Debug.Log("Building generation error: Grid X is less than minimum building length");
        }
        if (gridX < maxBuildingLength)
        {
            Debug.Log("Building generation error: Grid X is less than maximum building length");
        }
        if (gridY < minBuildingLength)
        {
            Debug.Log("Building generation error: Grid Y is less than minimum building length");
        }
        if (gridY < maxBuildingLength)
        {
            Debug.Log("Building generation error: Grid Y is less than maximum building length");
        }
        if (gridX < minBuildingHeight)
        {
            Debug.Log("Building generation error: Grid X is less than minimum building height");
        }
        if (gridX < maxBuildingHeight)
        {
            Debug.Log("Building generation error: Grid X is less than maximum building height");
        }
        if (gridY < minBuildingHeight)
        {
            Debug.Log("Building generation error: Grid Y is less than minimum building height");
        }
        if (gridY < maxBuildingHeight)
        {
            Debug.Log("Building generation error: Grid Y is less than maximum building height");
        }
        if (gridY < minPL)
        {
            Debug.Log("Building generation error: Grid Y is less than minimum path length");
        }
        if (gridY < maxPL)
        {
            Debug.Log("Building generation error: Grid Y is less than maximum path length");
        }
        if (gridX < minPL)
        {
            Debug.Log("Building generation error: Grid X is less than minimum path length");
        }
        if (gridX < maxPL)
        {
            Debug.Log("Building generation error: Grid X is less than maximum path length");
        }
        if (gridY < minPW)
        {
            Debug.Log("Building generation error: Grid Y is less than minimum path width");
        }
        if (gridY < maxPW)
        {
            Debug.Log("Building generation error: Grid Y is less than maximum path width");
        }
        if (gridX < minPW)
        {
            Debug.Log("Building generation error: Grid X is less than minimum path width");
        }
        if (gridX < maxPW)
        {
            Debug.Log("Building generation error: Grid X is less than maximum path width");
        }
        if (maxPW >= minPL)
        {
            Debug.Log("Building generation error: Minimum path length is not larger than maximum path width. This may cause unexpected results");
        }
        int extensionX;
        int originX;
        int originY;
        int height;
        GameObject parentClone;
        List<int> buildingDimensions = new List<int>();
        List<List<int>> possibilities;
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
                    catch (KeyNotFoundException) {}
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
        foreach (List<int> originCoords in gridCells)
        {
            possibilities = new List<List<int>>();
            originX = originCoords[0];
            originY = originCoords[1];
            if (!(CheckCoords(grid, originX, originY)))
            {
                extensionX = maxBuildingLength + (gap * 2) - 1;
                for (int extensionY = 0; extensionY < maxBuildingLength + (gap * 2); extensionY++)
                {
                    if (extensionY >= maxBuildingWidth + (gap * 2) && extensionX >= maxBuildingWidth + (gap * 2))
                    {
                        extensionX = maxBuildingWidth + (gap * 2) - 1;
                    }
                    for (int x = 0; x <= extensionX; x++)
                    {
                        if (CheckCoords(grid, originX + x, originY + extensionY))
                        {
                            extensionX = x - 1;
                            break;
                        }
                        else if ((x + 1 >= minBuildingLength + (gap * 2) && extensionY + 1 >= minBuildingWidth + (gap * 2)) ||
                            (extensionY + 1 >= minBuildingLength + (gap * 2) && x + 1 >= minBuildingWidth + (gap * 2)))
                        {
                            possibilities.Add(new List<int> { x + 1, extensionY + 1 });
                        }
                        if (Time.realtimeSinceStartup - startTime > 0.1)
                        {
                            yield return null;
                            startTime = Time.realtimeSinceStartup;
                        }
                    }
                }
                if (possibilities.Count != 0)
                {
                    buildingDimensions = possibilities[Random.Range(0, possibilities.Count)];
                    parentClone = Instantiate
                        (
                            buildingParent,
                            new Vector3
                            (
                                ((buildingDimensions[0] - gridX - 1) / 2f + originX) * 3f,
                                0,
                                ((buildingDimensions[1] - gridY - 1) / 2f + originY) * 3f
                            ),
                            Quaternion.identity
                        );
                    parentPosition = parentClone.transform.position;
                    maxDistance = Mathf.Min(mapSizeX, mapSizeY) / 2;
                    normalisedDistance = Mathf.Clamp(Vector3.Distance(parentPosition, transform.position) * (1 / maxDistance), 0, 1);
                    randomHeight = Random.Range(minBuildingHeight, maxBuildingHeight + 1);
                    smoothHeight = maxBuildingHeight - (normalisedDistance * (maxBuildingHeight - minBuildingHeight));
                    height = DivideAndRound((randomHeight * heightRandomness + (smoothHeight * (1 - heightRandomness))) * 3);
                    parentClone.transform.position = new Vector3(parentPosition.x, height * 1.5f, parentPosition.z);
                    parentClone.GetComponent<BoxCollider>().size = new Vector3
                        (
                            (buildingDimensions[0] - (gap * 2)) * 3,
                            height * 3,
                            (buildingDimensions[1] - (gap * 2)) * 3
                        );
                    for (int x = gap; x < buildingDimensions[0] - gap; x++)
                    {
                        for (int y = gap; y < buildingDimensions[1] - gap; y++)
                        {
                            grid[(originX + x).ToString() + " " + (originY + y).ToString()] = true;
                            for (int z = 1; z <= height; z++)
                            {
                                if
                                (
                                    (x == gap || x == buildingDimensions[0] - 1 - gap || y == gap || y == buildingDimensions[1] - 1 - gap)
                                    || z == height
                                )
                                {
                                    Instantiate
                                    (
                                        buildingSegment,
                                        new Vector3((originX + x - (gridX / 2f)) * 3f, z * 3f - 1.5f, (originY + y - (gridY / 2f)) * 3f),
                                        Quaternion.identity,
                                        //parentClone.transform
                                        //Building -> Building Parent -> Building Segment so that collapse works.
                                        parentClone.transform.GetChild(0)
                                    );
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
