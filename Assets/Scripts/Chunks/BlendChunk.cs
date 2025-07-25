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