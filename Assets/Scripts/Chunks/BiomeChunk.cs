using UnityEngine;

public class BiomeChunk : Chunk
{
    public Biome biome;

    public override void Initialize(Vector3Int chunkPosition, Biome biome, bool displayNeighbors)
    {
        base.Initialize(chunkPosition, biome, displayNeighbors);

        this.chunkPosition = chunkPosition;
        this.biome = biome;
        chunkColor = new Color(biome.biomeColor.r, biome.biomeColor.g, biome.biomeColor.b, 0.2f);
    }

    public override void DrawDirection(Vector3Int direction)
    {
        indicatorColor = Color.yellow;

        base.DrawDirection(direction);
    }

    public void GenerateDensity()
    {
        densityBuffer = biome.biomeShader.GenerateDensity(transform.position);

        SaveDensities(densityBuffer);

        isDensityGenerated = true;
    }

    void OnDrawGizmos()
    {
        if (!WorldGenerator.Instance) { return; }
        if (!WorldGenerator.IsReady) { return; }
        if (!secondInitialized) { return; }

        foreach (var kvp in neighbors)
        {
            if (kvp.Value is BlendChunk blendChunk)
            {
                DrawDirection(kvp.Key);
            }
        }

        Gizmos.color = chunkColor;

        Gizmos.DrawWireCube(transform.position, Vector3.one * (ws.chunkSize - 0.5f));
    }
}