using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlendCaseMap
{
    public BlendCaseY y1;
    public BlendCaseY y2;
    public BlendCaseY y3;

    public int[,,] GenerateMap()
    {
        int[,,] map = new int[3, 3, 3];

        List<BlendCaseY> yLevel = new List<BlendCaseY> { y1, y2, y3 };

        for (int y = 0; y < 3; y++)
        {
            BlendCaseY blendCaseY = yLevel[y];
            map[0, y, 0] = blendCaseY.x1.x;
            map[1, y, 0] = blendCaseY.x1.y;
            map[2, y, 0] = blendCaseY.x1.z;

            map[0, y, 1] = blendCaseY.x2.x;
            map[1, y, 1] = blendCaseY.x2.y;
            map[2, y, 1] = blendCaseY.x2.z;

            map[0, y, 2] = blendCaseY.x3.x;
            map[1, y, 2] = blendCaseY.x3.y;
            map[2, y, 2] = blendCaseY.x3.z;
        }

        return map;
    }
}

[System.Serializable]
public class BlendCaseY
{
    public Vector3Int x1;
    public Vector3Int x2;
    public Vector3Int x3;
}