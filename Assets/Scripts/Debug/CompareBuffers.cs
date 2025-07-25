using Sirenix.OdinInspector;
using UnityEngine;

public class CompareBuffers : MonoBehaviour
{
    public BlendChunk blendChunk;
    public BiomeChunk biomeChunk;

    string dump = "";
    string dumpInfo = "";

    [Button("Compare Buffers")]
    public void Compare(int index)
    {
        Vector4[] dBlend = new Vector4[blendChunk.densityBuffer.count];
        Vector4[] dBiome = new Vector4[biomeChunk.densityBuffer.count];

        blendChunk.densityBuffer.GetData(dBlend);
        biomeChunk.densityBuffer.GetData(dBiome);

        Vector4 vBlend = dBlend[index];
        Vector4 vBiome = dBiome[index];

        dumpInfo += $"[INDEX:{index}] Value at Blend: {vBlend} | Value at Biome: {vBiome}" + "\n";
    }

    [Button("Compare Auto")]
    public void CompareAuto()
    {
        for (int i = 0; i < blendChunk.densityBuffer.count; i++)
        {
            Compare(i);
        }
    }

    [Button("Dump Compares")]
    public void Dump()
    {
        dump += $"Dump begin. \n";
        dump += $"numPointsPerAxis: {WorldGenerator.Settings.numPointsPerAxis}\n";
        dump += $"blendChunk: {blendChunk.name}\n";
        dump += $"biomeChunk: {biomeChunk.name}\n";
        dump += "\n";
        dump += dumpInfo;

        System.IO.File.WriteAllText(
            $"{Application.dataPath}/Dumps/CompareBuffers-{System.DateTime.Now.ToString("MM-dd-HH-mm-ss")}.txt",
            dump
        );
    }
}