using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomPathGenerator), true), CanEditMultipleObjects]
public class RandomPathGeneratorEditor : PathGeneratorEditor
    {
    public static bool randomGenerationFoldOut = false;
    protected SerializedProperty minDistance = null;
    protected SerializedProperty maxDistance = null;
    protected SerializedProperty alternateY = null;

    protected override void InitializeVariables()
        {
        base.InitializeVariables();
        minDistance = serializedObject.FindProperty("minDistance");
        maxDistance = serializedObject.FindProperty("maxDistance");
        alternateY = serializedObject.FindProperty("alternateY");
        }

    protected override void ShowGeneratorSettings(GUIStyle headerStyle)
        {
        GUILayout.BeginVertical(EditorStyles.helpBox);
            {
            EditorGUI.indentLevel++;
            ShowPath(headerStyle);
            DrawUILine();
            ShowRandomGenerationSettings(headerStyle);
            DrawUILine();
            ShowSettings(headerStyle);
            DrawUILine();
            ShowLimits(headerStyle);
            DrawUILine();
            ShowTime(headerStyle);
            DrawUILine();
            ShowCollider(headerStyle);
            }
        GUILayout.EndVertical();
        }

    protected virtual void ShowRandomGenerationSettings(GUIStyle headerStyle)
        {
        randomGenerationFoldOut = EditorGUILayout.Foldout(randomGenerationFoldOut, "Random Generation Settings", true, headerStyle);
        if (randomGenerationFoldOut)
            {
            EditorGUILayout.PropertyField(minDistance);
            EditorGUILayout.PropertyField(maxDistance);
            EditorGUILayout.PropertyField(alternateY);
            }
        }
    }
