using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpiralGenerator), true)]
public class SpiralGeneratorEditor : PathGeneratorEditor
    {
    public static bool spiralGenerationFoldOut = false;
    protected SerializedProperty spiralLength = null;
    protected SerializedProperty spiralOrigin = null;
    protected SerializedProperty spiralDir = null;
    protected SerializedProperty useSpiralVariation = null;
    protected SerializedProperty spiralVariation = null;
    protected SerializedProperty clockwiseSpiral = null;

    protected override void InitializeVariables()
        {
        base.InitializeVariables();
        spiralLength = serializedObject.FindProperty("spiralLength");
        spiralOrigin = serializedObject.FindProperty("spiralOrigin");
        spiralDir = serializedObject.FindProperty("spiralDir");
        useSpiralVariation = serializedObject.FindProperty("useSpiralVariation");
        spiralVariation = serializedObject.FindProperty("spiralVariation");
        clockwiseSpiral = serializedObject.FindProperty("clockwiseSpiral");
        }

    protected override void ShowGeneratorSettings(GUIStyle headerStyle)
        {
        GUILayout.BeginVertical(EditorStyles.helpBox);
            {
            EditorGUI.indentLevel++;
            ShowPath(headerStyle);
            DrawUILine();
            ShowSpiralGenerationSettings(headerStyle);
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

    // Don't need the is3D here
    protected override void ShowSettings(GUIStyle headerStyle)
        {
        settingsFoldOut = EditorGUILayout.Foldout(settingsFoldOut, "Settings", true, headerStyle);
        if (settingsFoldOut && showSettings)
            {
            EditorGUILayout.PropertyField(generateMultiplePoints);
            EditorGUILayout.PropertyField(pointsToAdd, new GUIContent("Number of Points to Add"));
            EditorGUILayout.PropertyField(startPointsNum);
            EditorGUILayout.PropertyField(startPoints, true);
            }
        }

    protected virtual void ShowSpiralGenerationSettings(GUIStyle headerStyle)
        {
        spiralGenerationFoldOut = EditorGUILayout.Foldout(spiralGenerationFoldOut, "Spiral Generation Settings", true, headerStyle);
        if (spiralGenerationFoldOut)
            {
            EditorGUILayout.PropertyField(spiralLength);
            EditorGUILayout.PropertyField(spiralOrigin);
            EditorGUILayout.PropertyField(spiralDir, new GUIContent("Spiral Direction/Axis"));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useSpiralVariation);
            EditorGUILayout.PropertyField(clockwiseSpiral);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(spiralVariation);
            }
        }
    }
