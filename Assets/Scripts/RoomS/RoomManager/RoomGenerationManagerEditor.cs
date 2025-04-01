using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomGenerationManager))]
public class RoomGenerationManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomGenerationManager roomGen = (RoomGenerationManager)target;
        if (GUILayout.Button("Spawn Test Room"))
        {
            roomGen.SpawnRoom(Vector2Int.zero, Vector2Int.up);
        }
    }
}
