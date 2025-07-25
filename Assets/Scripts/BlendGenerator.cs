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
    // Fix in BlendGenerator.cs - GenerateBlendedDensity method
    public Vector4[,,] GenerateBlendedDensity(BlendChunk blendChunk)
    {
        int nppa = ws.numPointsPerAxis;
        Vector4[,,] blendedDensity = new Vector4[nppa, nppa, nppa];
        bool[,,] isSet = new bool[nppa, nppa, nppa];

        foreach (var kvp in blendChunk.neighbors)
        {
            if (kvp.Value is BiomeChunk biomeChunk && biomeChunk.isDensityGenerated)
            {
                Vector3Int dir = kvp.Key;
                if (!IsDirectionAllowed(dir)) continue;

                // Determine the slice to copy
                for (int x = 0; x < nppa; x++)
                {
                    for (int y = 0; y < nppa; y++)
                    {
                        for (int z = 0; z < nppa; z++)
                        {
                            Vector3Int pos = new Vector3Int(x, y, z);
                            if (!IsOnFace(pos, dir, nppa)) continue;

                            Vector3Int blendPos = GetEdgeOrCornerBlendPos(pos, dir, nppa);
                            Vector3Int neighborPos = GetEdgeOrCornerNeighborPos(pos, dir, nppa);

                            // Get the original neighbor data
                            Vector4 neighborData = biomeChunk.densityValues[neighborPos.x, neighborPos.y, neighborPos.z];
                            
                            // FIXED: Transform the coordinates to blend chunk's local space
                            Vector4 transformedData = TransformCoordinatesToBlendSpace(
                                neighborData, 
                                blendPos, 
                                blendChunk.transform.position,
                                nppa,
                                ws.chunkSize
                            );

                            blendedDensity[blendPos.x, blendPos.y, blendPos.z] = transformedData;
                            isSet[blendPos.x, blendPos.y, blendPos.z] = true;
                        }
                    }
                }
            }
        }

        // Fill unset face values by interpolating
        InterpolateUnsetFaceSlices(blendedDensity, isSet);

        return blendedDensity;
    }

    // NEW METHOD: Transform coordinates from neighbor space to blend space
    private Vector4 TransformCoordinatesToBlendSpace(Vector4 neighborData, Vector3Int blendPos, Vector3 blendChunkCenter, int nppa, int chunkSize)
    {
        // Calculate what the local coordinates should be for this blend position
        float localX = blendPos.x * (chunkSize / (float)(nppa - 1)) - chunkSize/2f;
        float localY = blendPos.y * (chunkSize / (float)(nppa - 1)) - chunkSize/2f;
        float localZ = blendPos.z * (chunkSize / (float)(nppa - 1)) - chunkSize/2f;
        
        // Return the corrected coordinate data (keep the original density value in .w)
        return new Vector4(localX, localY, localZ, neighborData.w);
    }

    bool IsOnFace(Vector3Int pos, Vector3Int dir, int n)
    {
        return
            (dir.x != 0 && pos.x == (dir.x > 0 ? n - 1 : 0)) ||
            (dir.y != 0 && pos.y == (dir.y > 0 ? n - 1 : 0)) ||
            (dir.z != 0 && pos.z == (dir.z > 0 ? n - 1 : 0));
    }

    Vector3Int GetEdgeOrCornerBlendPos(Vector3Int pos, Vector3Int dir, int n)
    {
        return new Vector3Int(
            dir.x != 0 ? (dir.x > 0 ? n - 1 : 0) : pos.x,
            dir.y != 0 ? (dir.y > 0 ? n - 1 : 0) : pos.y,
            dir.z != 0 ? (dir.z > 0 ? n - 1 : 0) : pos.z
        );
    }

    Vector3Int GetEdgeOrCornerNeighborPos(Vector3Int pos, Vector3Int dir, int n)
    {
        return new Vector3Int(
            dir.x != 0 ? (dir.x > 0 ? 0 : n - 1) : pos.x,
            dir.y != 0 ? (dir.y > 0 ? 0 : n - 1) : pos.y,
            dir.z != 0 ? (dir.z > 0 ? 0 : n - 1) : pos.z
        );
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

    void InterpolateUnsetFaceSlices(Vector4[,,] grid, bool[,,] isSet)
    {
        int n = ws.numPointsPerAxis;

        for (int axis = 0; axis < 3; axis++) // 0=x, 1=y, 2=z
        {
            for (int i = 0; i < n; i++) // each face slice
            {
                for (int j = 0; j < n; j++)
                {
                    Vector2Int fixedIndices = new Vector2Int(i, j);

                    // Interpolate along the remaining axis
                    for (int k = 0; k < n; k++)
                    {
                        Vector3Int pos = GetVector3IntFromAxes(axis, fixedIndices, k);
                        if (isSet[pos.x, pos.y, pos.z]) continue;

                        // Find nearest set values on both sides
                        int before = -1, after = -1;
                        for (int d = k - 1; d >= 0; d--)
                        {
                            var p = GetVector3IntFromAxes(axis, fixedIndices, d);
                            if (isSet[p.x, p.y, p.z]) { before = d; break; }
                        }
                        for (int d = k + 1; d < n; d++)
                        {
                            var p = GetVector3IntFromAxes(axis, fixedIndices, d);
                            if (isSet[p.x, p.y, p.z]) { after = d; break; }
                        }

                        if (before != -1 && after != -1)
                        {
                            float t = (k - before) / (float)(after - before);
                            Vector3Int pb = GetVector3IntFromAxes(axis, fixedIndices, before);
                            Vector3Int pa = GetVector3IntFromAxes(axis, fixedIndices, after);
                            grid[pos.x, pos.y, pos.z] = Vector4.Lerp(grid[pb.x, pb.y, pb.z], grid[pa.x, pa.y, pa.z], t);
                            isSet[pos.x, pos.y, pos.z] = true;
                        }
                    }
                }
            }
        }
    }

    Vector3Int GetVector3IntFromAxes(int axis, Vector2Int fixedIndices, int changing)
    {
        switch (axis)
        {
            case 0: return new Vector3Int(changing, fixedIndices.x, fixedIndices.y); // x varies
            case 1: return new Vector3Int(fixedIndices.x, changing, fixedIndices.y); // y varies
            case 2: return new Vector3Int(fixedIndices.x, fixedIndices.y, changing); // z varies
            default: return Vector3Int.zero;
        }
    }
}