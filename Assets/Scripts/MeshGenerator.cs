using System.Collections;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public static MeshGenerator Instance { get; private set; }
    public static bool IsReady => Instance._isReady;

    private bool _isReady = false;

    public ComputeShader marchingCubesShader;

    public float zeroPoint = 0f;

    private ComputeBuffer pointsBuffer;
    private ComputeBuffer triangleBuffer;
    private ComputeBuffer triCountBuffer;

    private WorldSettings ws => WorldGenerator.Settings;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnDestroy()
    {
        if (triangleBuffer != null)
        {
            triangleBuffer.Release();
            triangleBuffer = null;
        }
        if (pointsBuffer != null)
        {
            pointsBuffer.Release();
            pointsBuffer = null;
        }
        if (triCountBuffer != null)
        {
            triCountBuffer.Release();
            triCountBuffer = null;
        }
    }

    public Mesh GenerateMesh(ComputeBuffer pointsBuffer) // add zeroPoint as parameter
    {
        triangleBuffer.SetCounterValue(0);
        marchingCubesShader.SetBuffer(0, "points", pointsBuffer);
        marchingCubesShader.SetBuffer(0, "triangles", triangleBuffer);
        marchingCubesShader.SetInt("numPointsPerAxis", ws.numPointsPerAxis);
        marchingCubesShader.SetFloat("zeroPoint", zeroPoint);

        marchingCubesShader.Dispatch(0, ws.numThreadsPerAxis, ws.numThreadsPerAxis, ws.numThreadsPerAxis);

        // Get number of triangles in the triangle buffer
        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
        int[] triCountArray = { 0 };
        triCountBuffer.GetData(triCountArray);
        int numTris = triCountArray[0];

        // Get triangle data from shader
        Triangle[] tris = new Triangle[numTris];
        triangleBuffer.GetData(tris, 0, 0, numTris);

        Mesh mesh = new Mesh();

        var vertices = new Vector3[numTris * 3];
        var meshTriangles = new int[numTris * 3];
        var uvs = new Vector2[numTris * 3]; // NEW: UV coordinates array

        for (int i = 0; i < numTris; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int vertIndex = i * 3 + j;
                meshTriangles[vertIndex] = vertIndex;
                vertices[vertIndex] = tris[i][j];

                uvs[vertIndex] = GenerateUV(vertices[vertIndex]);
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = meshTriangles;
        mesh.uv = uvs; // NEW: Assign UV coordinates

        mesh.RecalculateNormals();

        return mesh;
    }

    public void CreateBuffers()
    {
        if (!Application.isPlaying || pointsBuffer == null || ws.numPoints != pointsBuffer.count)
        {
            if (Application.isPlaying)
            {
                ReleaseBuffers();
            }
            triangleBuffer = new ComputeBuffer(ws.maxTriangleCount, sizeof(float) * 3 * 3, ComputeBufferType.Append);
            pointsBuffer = new ComputeBuffer(ws.numPoints, sizeof(float) * 4);
            triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

            _isReady = true;
        }
        
        Debug.Log("MeshGenerator | Buffers created successfully.");
    }

    // FIXED: Simple UV mapping to avoid cross patterns
    private Vector2 GenerateUV(Vector3 worldPosition)
    {
        // Simple approach: use just X and Z coordinates with consistent scaling
        float scale = 0.25f; // Adjust this value to change texture size
        
        float u = worldPosition.x * scale;
        float v = worldPosition.z * scale;
        
        // Keep only the fractional part for tiling
        u = u - Mathf.Floor(u);
        v = v - Mathf.Floor(v);
        
        // Ensure we're in 0-1 range
        u = Mathf.Abs(u);
        v = Mathf.Abs(v);
        
        return new Vector2(u, v);
    }
    
    private void ReleaseBuffers()
    {
        if (triangleBuffer != null)
        {
            triangleBuffer.Release();
            pointsBuffer.Release();
            triCountBuffer.Release();
        }
    }

    struct Triangle {
#pragma warning disable 649 // disable unassigned variable warning

        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Vector3 this [int i] {
            get {
                switch (i) {
                    case 0:
                        return a;
                    case 1:
                        return b;
                    default:
                        return c;
                }
            }
        }
    }
}