using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DensityDisplayer : MonoBehaviour
{
    public static DensityDisplayer Instance { get; private set; }

    public GameObject densityPointPrefab;

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

    public void ToggleDensities(Chunk chunk)
    {
        if (!chunk.isDensityGenerated)
        {
            return;
        }

        if (chunk.transform.childCount > 0)
        {
            Destroy(chunk.transform.GetChild(0).gameObject);
        }
        else
        {
            DisplayDensities(chunk);
        }
    }

    private void DisplayDensities(Chunk chunk)
    {
        Debug.Log($"{chunk.GetType().Name}.");

        GameObject densityHolder = new GameObject("DensityDisplay");
        densityHolder.transform.SetParent(chunk.transform);

        Vector4[,,] densities = chunk.densityValues;

        Vector3 basePosition = chunk.transform.position - (Vector3.one * ws.chunkSize / 2f);

        float spacing = ws.pointSpacing;
        int nppa = ws.numPointsPerAxis;

        for (int x = 0; x < nppa; x++)
        {
            for (int y = 0; y < nppa; y++)
            {
                for (int z = 0; z < nppa; z++)
                {
                    float density = densities[x, y, z].w; // Assuming w is the density value

                    Vector3 pointPosition = basePosition + new Vector3(x, y, z) * spacing;

                    GameObject densityPointObject = Instantiate(densityPointPrefab, pointPosition, Quaternion.identity, densityHolder.transform);
                    DensityPoint densityPoint = densityPointObject.GetComponent<DensityPoint>();
                    densityPoint.SetDensity(new Vector3Int(x, y, z), density);
                }
            }
        }
    }
}
