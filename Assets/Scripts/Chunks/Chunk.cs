using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Chunk : MonoBehaviour
{
    [PropertyRange(0.4f, 0.8f)] public float indicatorThickness = 0.5f;
    [PropertyRange(0.4f, 0.6f)] public float indicatorOffset = 0.5f;

    protected bool secondInitialized = false;

    public Vector4[,,] densityValues;

    protected ComputeBuffer densityBuffer;

    public bool isDensityGenerated = false;

    private bool displayNeighbors = false;

    protected WorldSettings ws => WorldGenerator.Settings;

    public enum NeighborSearch
    {
        SixFace, // index: 0-5
        TwelveEdge, // index: 6-17
        EightCorner, //  index: 18-25
        All // index: 0-25
    }

    private List<Vector3Int> searchDirectionsAll = new List<Vector3Int>
    {
        // 6 face directions
        new Vector3Int(1, 0, 0),   // Right
        new Vector3Int(-1, 0, 0),  // Left
        new Vector3Int(0, 1, 0),   // Up
        new Vector3Int(0, -1, 0),  // Down
        new Vector3Int(0, 0, 1),   // Forward
        new Vector3Int(0, 0, -1),  // Backward

        // 12 edge directions (combinations of 2 axes)
        new Vector3Int(1, 1, 0),   // Right Up
        new Vector3Int(-1, 1, 0),  // Left Up
        new Vector3Int(1, -1, 0),  // Right Down
        new Vector3Int(-1, -1, 0), // Left Down

        new Vector3Int(1, 0, 1),   // Right Forward
        new Vector3Int(-1, 0, 1),  // Left Forward
        new Vector3Int(1, 0, -1),  // Right Backward
        new Vector3Int(-1, 0, -1), // Left Backward

        new Vector3Int(0, 1, 1),   // Up Forward
        new Vector3Int(0, 1, -1),  // Up Backward
        new Vector3Int(0, -1, 1),  // Down Forward
        new Vector3Int(0, -1, -1), // Down Backward

        // 8 corner directions (combinations of 3 axes)
        new Vector3Int(1, 1, 1),    // Right Up Forward
        new Vector3Int(1, 1, -1),   // Right Up Backward
        new Vector3Int(1, -1, 1),   // Right Down Forward
        new Vector3Int(1, -1, -1),  // Right Down Backward
        new Vector3Int(-1, 1, 1),   // Left Up Forward
        new Vector3Int(-1, 1, -1),  // Left Up Backward
        new Vector3Int(-1, -1, 1),  // Left Down Forward
        new Vector3Int(-1, -1, -1), // Left Down Backward
    };

    public Dictionary<Vector3Int, Chunk> neighbors;

    protected Vector3Int chunkPosition;

    protected Color chunkColor = Color.white;
    protected Color indicatorColor = Color.white;

    protected MeshFilter meshFilter;
    protected MeshRenderer meshRenderer;
    protected MeshCollider meshCollider;

    public virtual void Initialize(Vector3Int chunkPosition, Biome biome, bool displayNeighbors)
    {
        meshFilter = this.AddComponent<MeshFilter>();
        meshRenderer = this.AddComponent<MeshRenderer>();
        meshCollider = this.AddComponent<MeshCollider>();

        this.displayNeighbors = displayNeighbors;
    }

    public virtual void SecondInitialize()
    {
        FindNeighbors();

        secondInitialized = true;
    }

    public virtual void DrawDirection(Vector3Int direction)
    {
        if (!displayNeighbors) return;

        int c = 0; // case

        if (direction.x != 0) c++;
        if (direction.y != 0) c++;
        if (direction.z != 0) c++;

        int chunkSize = ws.chunkSize;

        Vector3 center = new Vector3(
            direction.x * ((chunkSize / 2f) - indicatorOffset),
            direction.y * ((chunkSize / 2f) - indicatorOffset),
            direction.z * ((chunkSize / 2f) - indicatorOffset)
        );

        Vector3 size = new Vector3(
            direction.x != 0 ? indicatorThickness : chunkSize - indicatorOffset * 4,
            direction.y != 0 ? indicatorThickness : chunkSize - indicatorOffset * 4,
            direction.z != 0 ? indicatorThickness : chunkSize - indicatorOffset * 4
        );

        Gizmos.color = new Color(
            indicatorColor.r,
            indicatorColor.g,
            indicatorColor.b,
            0.5f // semi-transparent
        );

        switch (c)
        {
            case 1:
                Gizmos.DrawCube(transform.position + center, size);
                break;
            case 2:
                Gizmos.DrawCube(transform.position + center, size);
                break;
            case 3:
                Gizmos.DrawCube(transform.position + center, size);
                break;
        }
    }

    public virtual void GenerateMesh()
    {
        Mesh mesh;

        mesh = MeshGenerator.Instance.GenerateMesh(densityBuffer, 1);

        meshFilter.mesh = mesh;
        meshFilter.sharedMesh = mesh;
        meshRenderer.material = WorldGenerator.DefaultMaterial;
        meshRenderer.sharedMaterial = WorldGenerator.DefaultMaterial;

        if (mesh.vertexCount >= 3)
        {
            meshCollider.sharedMesh = mesh;
        }

        //densityBuffer.Release(); Disabled for dumping densities
    }

    [Button("Dump Densities")]
    public void DumpDensities()
    {
        string dumpName = $"{this.GetType().Name}-at-{System.DateTime.Now.ToString("MM-dd-HH-mm-ss")}.txt";

        int dvl = densityValues.GetLength(0);

        Vector4[] densityValuesFlat = new Vector4[dvl * dvl * dvl];

        for (int x = 0; x < dvl; x++)
        {
            for (int y = 0; y < dvl; y++)
            {
                for (int z = 0; z < dvl; z++)
                {
                    densityValuesFlat[x + y * dvl + z * dvl * dvl] = densityValues[x, y, z];
                }
            }
        }

        Vector4[] densityBufferFlat = new Vector4[dvl * dvl * dvl];

        densityBuffer.GetData(densityBufferFlat);

        System.IO.File.WriteAllText(
            $"{Application.dataPath}/Dumps/{dumpName}",
            $"Density Values:\n{string.Join("\n", densityValuesFlat)}\n\nDensity Buffer:\n{string.Join("\n", densityBufferFlat)}"
        );
    }

    protected ComputeBuffer BuildBuffer()
    {
        // Convert densityValues Vector4[,,] to Vector4[]
        int nppa = ws.numPointsPerAxis;
        Vector4[] flat = new Vector4[nppa * nppa * nppa];

        for (int x = 0; x < nppa; x++)
        {
            for (int y = 0; y < nppa; y++)
            {
                for (int z = 0; z < nppa; z++)
                {
                    flat[x + y * nppa + z * nppa * nppa] = densityValues[x, y, z];
                }
            }
        }

        // Create a new compute buffer
        ComputeBuffer newDensityBuffer = new ComputeBuffer(flat.Length, sizeof(float) * 4);
        newDensityBuffer.SetData(flat);

        return newDensityBuffer;
    }

    protected void SaveDensities(ComputeBuffer densityBuffer)
    {
        int n = ws.numPointsPerAxis;
        densityValues = new Vector4[n, n, n];

        Vector4[] flat = new Vector4[n * n * n];

        densityBuffer.GetData(flat);

        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < n; y++)
            {
                for (int z = 0; z < n; z++)
                {
                    densityValues[x, y, z] = flat[x + y * n + z * n * n];
                }
            }
        }
    }

    private void FindNeighbors()
    {
        NeighborSearch neighborSearchMode = WorldGenerator.Instance.neighborSearchMode;
        // SixFace | index: 0-5
        // TwelveEdge | index: 6-17
        // EightCorner | index: 18-25
        // All | index: 0-25

        neighbors = new Dictionary<Vector3Int, Chunk>();

        List<Vector3Int> searchDirections = new List<Vector3Int>();

        switch (neighborSearchMode)
        {
            case NeighborSearch.SixFace:
                searchDirections.AddRange(searchDirectionsAll.GetRange(0, 6));
                break;
            case NeighborSearch.TwelveEdge:
                searchDirections.AddRange(searchDirectionsAll.GetRange(6, 12));
                break;
            case NeighborSearch.EightCorner:
                searchDirections.AddRange(searchDirectionsAll.GetRange(18, 8));
                break;
            case NeighborSearch.All:
                searchDirections.AddRange(searchDirectionsAll);
                break;
        }

        foreach (Vector3Int direction in searchDirections)
        {
            Vector3Int neighborPosition = chunkPosition + direction;

            if (ChunkGenerator.Chunks.TryGetValue(neighborPosition, out Chunk neighborChunk))
            {
                neighbors.Add(direction, neighborChunk);
            }
        }
    }
}