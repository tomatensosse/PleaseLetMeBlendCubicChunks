using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public int[,,] GenerateMap()
    {
        int[,,] map = null;

        if (WorldGenerator.Instance.worldGenMode == WorldGenerator.WorldGenMode.BlendCaseTest)
        {
            map = WorldGenerator.Instance.blendCaseMap.GenerateMap();
        }
        if (WorldGenerator.Instance.worldGenMode == WorldGenerator.WorldGenMode.IslandTest)
        {
            // Handle blend case generation
        }
        if (WorldGenerator.Instance.worldGenMode == WorldGenerator.WorldGenMode.CaveTest)
        {
            // Handle blend case generation
        }

        if (map == null)
        {
            Debug.LogError("Map generation failed.");
        }
        else
        {
            Debug.Log("MapGenerator | Map generated successfully.");
        }

        return map;
    }
}
