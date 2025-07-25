using UnityEngine;

public class BlendChunk : Chunk
{
    public override void Initialize(Vector3Int chunkPosition, Biome biome, bool displayNeighbors)
    {
        base.Initialize(chunkPosition, biome, displayNeighbors);

        this.chunkPosition = chunkPosition;
        chunkColor = new Color(1f, 0f, 1f, 0.2f); // magenta with transparency
    }

    public override void DrawDirection(Vector3Int direction)
    {
        indicatorColor = Color.magenta;

        base.DrawDirection(direction);
    }

    public void GenerateBlendedDensity()
    {
        densityValues = BlendGenerator.Instance.GenerateBlendedDensity(this);

        isDensityGenerated = true;
    }

    public override void GenerateMesh()
    {
        densityBuffer = BuildBuffer();

        base.GenerateMesh();
    }

    private ComputeBuffer BuildBuffer()
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
        ComputeBuffer densityBuffer = new ComputeBuffer(flat.Length, sizeof(float) * 4);
        densityBuffer.SetData(flat);

        return densityBuffer;
    }

    void OnDrawGizmos()
    {
        if (!WorldGenerator.Instance) { return; }
        if (!WorldGenerator.IsReady) { return; }
        if (!secondInitialized) { return; }

        foreach (var kvp in neighbors)
        {
            if (kvp.Value is BiomeChunk biomeChunk)
            {
                DrawDirection(kvp.Key);
            }
        }

        Gizmos.color = chunkColor;

        Gizmos.DrawWireCube(transform.position, Vector3.one * (ws.chunkSize - 0.5f));
    }
}