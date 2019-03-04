using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConvertToSpriteShape))]
public class ConvertToSpriteShapeEditor : Editor
    {
    public override void OnInspectorGUI()
        {
        ConvertToSpriteShape c = (ConvertToSpriteShape)target;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Convert to sprite shape (Smooth)"))
            {
            c.ConvertSmooth();
            }
        if (GUILayout.Button("Convert to sprite shape (Sharp)"))
            {
            c.ConvertSharp();
            }
        GUILayout.EndVertical();
        if (GUILayout.Button("Clear sprite shape"))
            {
            c.Clear();
            }
        }
    }
    


