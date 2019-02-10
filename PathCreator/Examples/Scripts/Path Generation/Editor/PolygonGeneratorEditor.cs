using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PolygonGenerator), true), CanEditMultipleObjects]
public class PolygonGeneratorEditor : PathGeneratorEditor
    {
    public static bool polygonGenerationFoldOut = false;
    protected SerializedProperty numEdges = null;
    protected SerializedProperty centerToVertexLength = null;
    protected SerializedProperty offsetAngle = null;
    protected SerializedProperty sharpCorners = null;
    protected SerializedProperty autoControlLength = null;
    protected SerializedProperty inset = null;
    protected SerializedProperty insetPercent = null;
    protected SerializedProperty originPoint = null;
    protected SerializedProperty axis = null;

    protected override void InitializeVariables()
        {
        base.InitializeVariables();
        numEdges = serializedObject.FindProperty("numEdges");
        centerToVertexLength = serializedObject.FindProperty("centerToVertexLength");
        offsetAngle = serializedObject.FindProperty("offsetAngle");
        sharpCorners = serializedObject.FindProperty("sharpCorners");
        autoControlLength = serializedObject.FindProperty("autoControlLength");
        inset = serializedObject.FindProperty("inset");
        insetPercent = serializedObject.FindProperty("insetPercent");
        originPoint = serializedObject.FindProperty("originPoint");
        axis = serializedObject.FindProperty("axis");
        }

    protected override void ShowGeneratorSettings(GUIStyle headerStyle)
        {
        GUILayout.BeginVertical(EditorStyles.helpBox);
            {
            EditorGUI.indentLevel++;
            ShowPath(headerStyle);
            DrawUILine();
            ShowPolygonGenerationSettings(headerStyle);
            DrawUILine();
            ShowLimits(headerStyle);
            DrawUILine();
            ShowTime(headerStyle);
            DrawUILine();
            ShowCollider(headerStyle);
            }
        GUILayout.EndVertical();
        }


    protected virtual void ShowPolygonGenerationSettings(GUIStyle headerStyle)
        {
        polygonGenerationFoldOut = EditorGUILayout.Foldout(polygonGenerationFoldOut, "Polygon Generation Settings", true, headerStyle);
        if (polygonGenerationFoldOut)
            {
            EditorGUILayout.PropertyField(numEdges);
            EditorGUILayout.PropertyField(centerToVertexLength);
            EditorGUILayout.PropertyField(offsetAngle);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(sharpCorners);
            EditorGUILayout.PropertyField(autoControlLength);
            EditorGUILayout.EndHorizontal();
            DrawUILine();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(inset);
            EditorGUILayout.PropertyField(insetPercent);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(originPoint);
            EditorGUILayout.PropertyField(axis);
            }
        }


    protected override void GenerationButtons()
        {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate polygon"))
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
        GUILayout.EndVertical();
        }

    }
