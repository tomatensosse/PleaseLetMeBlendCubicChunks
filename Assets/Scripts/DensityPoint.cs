using TMPro;
using UnityEngine;

public class DensityPoint : MonoBehaviour
{
    public Vector3Int pointPosition;
    public float densityValue;

    public MeshRenderer meshRenderer;
    public TMP_Text text;

    public void SetDensity(Vector3Int position, float density)
    {
        pointPosition = position;
        densityValue = density;

        meshRenderer.material.color = Color.Lerp(Color.red, Color.blue, Mathf.InverseLerp(-8f, 8f, density));
        text.text = densityValue.ToString("F1");
    }
}