using UnityEngine;

public class BiomeChunk : Chunk
{
    public Biome biome;

    public override void Initialize(Vector3Int chunkPosition, Biome biome)
    {
        this.chunkPosition = chunkPosition;
        this.biome = biome;
        chunkColor = new Color(biome.biomeColor.r, biome.biomeColor.g, biome.biomeColor.b, 0.2f);
    }

    public override void DrawDirection(Vector3Int direction)
    {
        indicatorColor = Color.yellow;

        base.DrawDirection(direction);
    }
}