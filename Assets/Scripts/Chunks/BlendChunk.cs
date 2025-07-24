using UnityEngine;

public class BlendChunk : Chunk
{
    public override void Initialize(Vector3Int chunkPosition, Biome biome)
    {
        this.chunkPosition = chunkPosition;
        chunkColor = new Color(1f, 0f, 1f, 0.2f); // magenta with transparency
    }

    public override void DrawDirection(Vector3Int direction)
    {
        indicatorColor = Color.magenta;

        base.DrawDirection(direction);
    }
}