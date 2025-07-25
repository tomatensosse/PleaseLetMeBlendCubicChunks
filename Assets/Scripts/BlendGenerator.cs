using UnityEngine;

public class BlendGenerator : MonoBehaviour
{
    public static BlendGenerator Instance { get; private set; }

    private WorldSettings ws => WorldGenerator.Settings;

    private static readonly Vector3Int[] sixFaceOffsets = new Vector3Int[]
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1),
    };

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            return;
        }

        Instance = this;
    }

    /// If WorldGenerator.Instance.neighborSearchMode is set to :
    /// 
    /// SixFaces it will only blend neighbors that are directly adjacent
    /// TwelveEdge it will blend neighbors that are adjacent and on the edges
    /// EightCorner it will blend neighbors that are adjacent, on the edges and diagonally
    /// AllFaces, it will blend all neighbors including diagonals
    /// Goal is to implement a single function that can handle all blending cases and change neighborSearchMode to debug it
    public Vector4[,,] GenerateBlendedDensity(BlendChunk blendChunk)
    {
        int nppa = ws.numPointsPerAxis;
        Vector4[,,] blendedDensity = new Vector4[nppa, nppa, nppa];

        foreach (var kvp in blendChunk.neighbors)
        {
            if (kvp.Value is BiomeChunk biomeChunk && biomeChunk.isDensityGenerated)
            {
                Vector3Int direction = kvp.Key;
                if (!IsDirectionAllowed(direction)) continue;

                // Determine points to blend based on direction
                for (int x = 0; x < nppa; x++)
                {
                    for (int y = 0; y < nppa; y++)
                    {
                        for (int z = 0; z < nppa; z++)
                        {
                            Vector3Int localPos = new Vector3Int(x, y, z);
                            Vector3Int blendPos = localPos;
                            Vector3Int neighborPos = localPos;

                            // Conditions for faces/edges/corners
                            bool match =
                                (direction.x != 0 && (x == (direction.x > 0 ? nppa - 1 : 0))) ||
                                (direction.y != 0 && (y == (direction.y > 0 ? nppa - 1 : 0))) ||
                                (direction.z != 0 && (z == (direction.z > 0 ? nppa - 1 : 0)));

                            if (!match) continue;

                            // Set positions
                            if (direction.x != 0) {
                                blendPos.x = direction.x > 0 ? nppa - 1 : 0;
                                neighborPos.x = direction.x > 0 ? 0 : nppa - 1;
                            }
                            if (direction.y != 0) {
                                blendPos.y = direction.y > 0 ? nppa - 1 : 0;
                                neighborPos.y = direction.y > 0 ? 0 : nppa - 1;
                            }
                            if (direction.z != 0) {
                                blendPos.z = direction.z > 0 ? nppa - 1 : 0;
                                neighborPos.z = direction.z > 0 ? 0 : nppa - 1;
                            }

                            blendedDensity[blendPos.x, blendPos.y, blendPos.z] =
                                biomeChunk.densityValues[neighborPos.x, neighborPos.y, neighborPos.z];
                        }
                    }
                }
            }
        }

        return blendedDensity;
    }

    bool IsDirectionAllowed(Vector3Int dir)
    {
        switch (WorldGenerator.Instance.neighborSearchMode)
        {
            case Chunk.NeighborSearch.SixFace:
                return Mathf.Abs(dir.x) + Mathf.Abs(dir.y) + Mathf.Abs(dir.z) == 1;
            case Chunk.NeighborSearch.TwelveEdge:
                return Mathf.Abs(dir.x) + Mathf.Abs(dir.y) + Mathf.Abs(dir.z) == 2;
            case Chunk.NeighborSearch.EightCorner:
                return Mathf.Abs(dir.x) + Mathf.Abs(dir.y) + Mathf.Abs(dir.z) == 3;
            case Chunk.NeighborSearch.All:
                return dir != Vector3Int.zero;
            default:
                return false;
        }
    }
}