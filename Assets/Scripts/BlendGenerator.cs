using UnityEngine;

public class BlendGenerator : MonoBehaviour
{
    public static BlendGenerator Instance { get; private set; }

    private WorldSettings ws => WorldGenerator.Settings;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            return;
        }

        Instance = this;
    }

    public Vector4[,,] GenerateBlendedDensity(BlendChunk blendChunk)
    {
        if (WorldGenerator.Instance.neighborSearchMode != Chunk.NeighborSearch.SixFace)
        {
            Debug.LogError("Blended density generation CURRENTLY only supports SixFace neighbor search mode.");
            return null;
        }

        int nppa = ws.numPointsPerAxis;

        Vector4[,,] blendedDensity = new Vector4[nppa, nppa, nppa];

        foreach (var kvp in blendChunk.neighbors)
        {
            if (kvp.Value is BiomeChunk biomeChunk && biomeChunk.isDensityGenerated)
            {
                Vector3Int direction = kvp.Key;

                int xMax = nppa;
                int yMax = nppa;
                int zMax = nppa;

                if (direction.x != 0) xMax = 1;
                if (direction.y != 0) yMax = 1;
                if (direction.z != 0) zMax = 1;

                for (int x = 0; x < xMax; x++)
                {
                    for (int y = 0; y < yMax; y++)
                    {
                        for (int z = 0; z < zMax; z++)
                        {
                            Vector3Int blendPosition = new Vector3Int(x, y, z);
                            Vector3Int neighborPosition = new Vector3Int(x, y, z);

                            // Adjust positions based on direction
                            if (direction.x != 0)
                            {
                                blendPosition.x = direction.x > 0 ? nppa - 1 : 0;  // Right face if positive, left if negative
                                neighborPosition.x = direction.x > 0 ? 0 : nppa - 1;  // Opposite face of neighbor
                            }
                            if (direction.y != 0)
                            {
                                blendPosition.y = direction.y > 0 ? nppa - 1 : 0;
                                neighborPosition.y = direction.y > 0 ? 0 : nppa - 1;
                            }
                            if (direction.z != 0)
                            {
                                blendPosition.z = direction.z > 0 ? nppa - 1 : 0;
                                neighborPosition.z = direction.z > 0 ? 0 : nppa - 1;
                            }

                            blendedDensity[blendPosition.x, blendPosition.y, blendPosition.z] =
                                biomeChunk.densityValues[neighborPosition.x, neighborPosition.y, neighborPosition.z];
                        }
                    }
                }
            }
        }

        // Fill in the rest of the blended density

        return blendedDensity;
    }
}