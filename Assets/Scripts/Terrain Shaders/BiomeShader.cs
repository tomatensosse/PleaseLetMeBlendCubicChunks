using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BiomeShader
{
    public ComputeShader shader;
    public ComputeBuffer pointsBuffer;

    [Header("Base Parameters")]
    public Vector3 offset = Vector3.zero;
    public Vector4 parameters = new Vector4(1, 0, 0, 0);
    public int numOctaves = 2;

    protected List<ComputeBuffer> buffersToRelease = new List<ComputeBuffer>();

    protected ComputeBuffer offsetsBuffer;

    private WorldSettings ws => WorldGenerator.Settings; // point to world, demoGenerator etc... ws => World.Settings

    public void SetBaseParameters()
    {
        shader.SetFloat("boundsSize", ws.boundsSize);
        shader.SetFloat("spacing", ws.pointSpacing);
        shader.SetVector("worldSize", (Vector3)ws.worldSize);

        shader.SetVector("offset", offset);
        shader.SetVector("params", parameters);
        shader.SetInt("octaves", Mathf.Max(1, numOctaves));
    }

    public void SetDynamicParameters()
    {
        shader.SetBuffer(0, "points", pointsBuffer);
        shader.SetBuffer(0, "offsets", offsetsBuffer);
        shader.SetInt("numPointsPerAxis", ws.numPointsPerAxis);
        shader.SetInt("numThreadsPerAxis", ws.numThreadsPerAxis);
    }

    public void GenerateDynamicParameters()
    {
        var prng = new System.Random(ws.seed);
        var offsets = new Vector3[numOctaves];
        float offsetRange = 1000;
        for (int i = 0; i < numOctaves; i++)
        {
            offsets[i] = new Vector3((float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1, (float)prng.NextDouble() * 2 - 1) * offsetRange;
        }

        var offsetsBuffer = new ComputeBuffer(offsets.Length, sizeof(float) * 3);
        offsetsBuffer.SetData(offsets);

        buffersToRelease.Add(offsetsBuffer);

        this.offsetsBuffer = offsetsBuffer;

        pointsBuffer = new ComputeBuffer(ws.numPoints, sizeof(float) * 4);
    }

    public void Dispatch(Vector3 worldPositionOfChunk)
    {
        shader.SetVector("centre", worldPositionOfChunk);

        int ntpa = ws.numThreadsPerAxis;

        shader.Dispatch(0, ntpa, ntpa, ntpa);
    }

    public abstract ComputeBuffer GenerateDensity(Vector3 worldPositionOfChunk);
}