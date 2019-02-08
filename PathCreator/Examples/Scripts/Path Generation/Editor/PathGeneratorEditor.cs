using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathGenerator),true)]
public class PathGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
        {
        DrawDefaultInspector();

        PathGenerator generator = (PathGenerator)target;
        if (GUILayout.Button("Generate point(s)"))
            {
            if (!generator.hasBeenInitialized)
                {
                generator.InitializeGenerator();
                }
            generator.GeneratePoint();
            }
        if (GUILayout.Button("RESET PATH"))
            {
            generator.Clear();
            }
        }
    }
