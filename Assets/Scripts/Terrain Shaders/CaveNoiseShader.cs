using UnityEngine;

[System.Serializable]
public class CaveNoiseShader : BiomeShader
{
    [Header("Cave Noise Parameters")]
    [Range(0.1f, 5f)]
    public float caveScale = 1f;
    
    [Range(0f, 1f)]
    public float caveThreshold = 0.4f;
    
    [Range(0.1f, 8f)]
    public float caveIntensity = 1f;
    
    [Range(1, 6)]
    public int caveOctaves = 3;
    
    [Range(1f, 3f)]
    public float caveLacunarity = 2f;
    
    [Range(0.1f, 1f)]
    public float cavePersistence = 0.5f;
    
    [Header("Cave Modulation")]
    [Range(-50f, 50f)]
    public float yOffset = 1f;
    
    [Range(0f, 2f)]
    public float verticalScale = 1f;
    
    [Range(-50f, 50f)]
    public float caveFloor = -10f;
    
    [Range(-50f, 50f)]
    public float caveCeiling = 30f;
    
    [Range(0f, 10f)]
    public float surfaceBlend = 5f;

    public override ComputeBuffer GenerateDensity(Vector3 worldPositionForChunk)
    {
        GenerateDynamicParameters();

        SetBaseParameters();
        SetDynamicParameters();

        SetCaveNoiseParameters();

        Dispatch(worldPositionForChunk);

        if (buffersToRelease != null)
        {
            foreach (var b in buffersToRelease)
            {
                b.Release();
            }
        }

        return pointsBuffer;
    }

    public void SetCaveNoiseParameters()
    {
        shader.SetFloat("caveScale", caveScale);
        shader.SetFloat("caveThreshold", caveThreshold);
        shader.SetFloat("caveIntensity", caveIntensity);
        shader.SetInt("caveOctaves", caveOctaves);
        shader.SetFloat("caveLacunarity", caveLacunarity);
        shader.SetFloat("cavePersistence", cavePersistence);
        shader.SetFloat("yOffset", yOffset);
        shader.SetFloat("verticalScale", verticalScale);
        shader.SetFloat("caveFloor", caveFloor);
        shader.SetFloat("caveCeiling", caveCeiling);
        shader.SetFloat("surfaceBlend", surfaceBlend);
    }
}