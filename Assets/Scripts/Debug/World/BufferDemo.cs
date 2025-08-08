using Sirenix.OdinInspector;
using UnityEngine;

public class BufferDemo : MonoBehaviour
{
    private Vector3[,,] dummyArray;
    private ComputeBuffer dummyBuffer;

    private int index = 0;
    private int size = 4;

    [Button("Advance")]
    public void Advance()
    {
        if (dummyBuffer == null)
        {
            GenerateDummyArray();
        }
        else
        {
            GetDummyArrayFromBuffer();
        }

        Vector3[] dummyArrayFlat = new Vector3[size * size * size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    dummyArrayFlat[x + y * size + z * size * size] = dummyArray[x, y, z];
                }
            }
        }

        Debug.Log($"[{index}] Dummy Array Flat: \n \n {string.Join("\n", values: dummyArrayFlat)}");

        dummyBuffer = new ComputeBuffer(size * size * size, sizeof(float) * 3);
        dummyBuffer.SetData(dummyArray);
    }

    private void GenerateDummyArray()
    {
        dummyArray = new Vector3[size, size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    dummyArray[x, y, z] = new Vector4(x, y, z);
                }
            }
        }
    }

    private void GetDummyArrayFromBuffer()
    {
        if (dummyBuffer != null)
        {
            dummyBuffer.GetData(dummyArray);
        }
    }
}