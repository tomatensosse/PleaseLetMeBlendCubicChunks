using Sirenix.Utilities.Editor;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public float margin = 10f;

    private void OnGUI()
    {
        // --- Top Left Box ---
        GUILayout.BeginArea(new Rect(margin, margin, Screen.width / 3f, 100));
        GUI.Box(new Rect(0, 0, Screen.width / 3f, 100), ""); // Background box
        GUILayout.BeginVertical();
        GUILayout.Label($"RenDistanceH: {WorldGenerator.RenderDistanceHorizontal}");
        GUILayout.Label($"RenDistanceV: {WorldGenerator.RenderDistanceVertical}");
        GUILayout.Label($"ChunkPosition: {DebugController.Position}");
        // Add more GUILayout elements here
        GUILayout.EndVertical();
        GUILayout.EndArea();

        // --- Top Right Box ---
        float areaWidth = Screen.width / 3f;
        GUILayout.BeginArea(new Rect(Screen.width - areaWidth - margin, margin, areaWidth, 120));
        GUI.Box(new Rect(0, 0, areaWidth, 250), ""); // Background box
        GUILayout.BeginVertical();
        GUILayout.Label("CONTROLS:", SirenixGUIStyles.BoldTitleCentered);
        GUILayout.Label("[WASDQE] - Move through chunks");
        GUILayout.Label("[ARROW KEYS] - Change render distance");
        GUILayout.Label("[R] - Regenerate world");
        GUILayout.Label("[F] - Display densities");
        // Add more GUILayout elements here
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}