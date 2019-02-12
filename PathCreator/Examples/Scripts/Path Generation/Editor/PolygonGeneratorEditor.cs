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
    protected SerializedProperty insetEveryNthPoint = null;
    protected SerializedProperty divideEdges = null;
    protected SerializedProperty divisionsPerEdge = null;
    protected SerializedProperty insetPercent = null;
    protected SerializedProperty originPoint = null;
    protected SerializedProperty axis = null;
    public PolygonGenerator polyGenerator;
    protected override void OnEnable()
        {
        polyGenerator = (PolygonGenerator)target;
        InitializeVariables();
        } 

    protected override void InitializeVariables()
        {
        base.InitializeVariables();
        numEdges = serializedObject.FindProperty("numEdges");
        centerToVertexLength = serializedObject.FindProperty("centerToVertexLength");
        offsetAngle = serializedObject.FindProperty("offsetAngle");
        sharpCorners = serializedObject.FindProperty("sharpCorners");
        autoControlLength = serializedObject.FindProperty("autoControlLength");
        inset = serializedObject.FindProperty("inset");
        insetEveryNthPoint = serializedObject.FindProperty("insetEveryNthPoint");
        divideEdges = serializedObject.FindProperty("divideEdges");
        divisionsPerEdge = serializedObject.FindProperty("divisionsPerEdge");
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
            EditorGUILayout.PropertyField(divideEdges);
            EditorGUILayout.EndHorizontal();
            DrawUILine();
            if (polyGenerator.inset)
                {
                EditorGUILayout.PropertyField(insetPercent);
                polyGenerator.insetEveryNthPoint = EditorGUILayout.IntSlider(new GUIContent("Inset Every n-th point"), polyGenerator.insetEveryNthPoint, 1, polyGenerator.GetNumPoints());
                DrawUILine();
                }
            if (polyGenerator.divideEdges)
                {
                polyGenerator.divisionsPerEdge = EditorGUILayout.IntSlider(new GUIContent("Divisions per edge"), polyGenerator.divisionsPerEdge, 1, 20);
                DrawUILine();
                }
            //EditorGUILayout.PropertyField(divisionsPerEdge);
            EditorGUILayout.PropertyField(originPoint);
            EditorGUILayout.PropertyField(axis);
            }
        }


    protected override void GenerationButtons()
        {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate polygon"))
            {
            if (!polyGenerator.hasBeenInitialized)
                {
                polyGenerator.InitializeGenerator();
                }
            polyGenerator.GeneratePoint();
            }
        if (GUILayout.Button("RESET PATH"))
            {
            polyGenerator.Clear();
            }
        GUILayout.EndVertical();
        }

    }
