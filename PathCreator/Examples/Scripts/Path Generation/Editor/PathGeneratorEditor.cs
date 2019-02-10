using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathGenerator),true), CanEditMultipleObjects]
public class PathGeneratorEditor : Editor
{

    #region Fields
    public static bool pathFoldOut = false;
    protected SerializedProperty pathCreator = null;
    protected SerializedProperty pathPoints = null;

    public static bool showSettings = true;
    public static bool settingsFoldOut = false;
    protected SerializedProperty generateMultiplePoints = null;
    protected SerializedProperty pointsToAdd = null;
    protected SerializedProperty is3D = null;
    protected SerializedProperty startPointsNum = null;
    protected SerializedProperty startPoints = null;

    public static bool showLimitsSettings = true;
    public static bool limitsFoldOut = false;
    protected SerializedProperty limitPoints = null;
    protected SerializedProperty maxPointsNum = null;
    protected SerializedProperty removePointsOnReachLimit = null;

    public static bool showGenerateOverTimeSettings = true;
    public static bool timeFoldOut = false;
    protected SerializedProperty generateOverTime = null;
    protected SerializedProperty timeBetweenGenerations = null;
    protected SerializedProperty numPointsToGenerateT = null;

    public static bool showColliderSettings = true;
    public static bool colliderFoldOut = false;
    protected SerializedProperty generate2DCollider = null;
    protected SerializedProperty physicsMaterial2D = null; 
    #endregion

    public Color lineColor = Color.grey;
    public PathGenerator generator;

    protected virtual void OnEnable()
        {
        generator = (PathGenerator)target;
        InitializeVariables();
        }

    protected virtual void InitializeVariables()
        {
        generateMultiplePoints = serializedObject.FindProperty("generateMultiplePoints");
        pointsToAdd = serializedObject.FindProperty("pointsToAdd");
        is3D = serializedObject.FindProperty("is3D");
        startPointsNum = serializedObject.FindProperty("startPointsNum");
        startPoints = serializedObject.FindProperty("startPoints");

        limitPoints = serializedObject.FindProperty("limitPoints");
        maxPointsNum = serializedObject.FindProperty("maxPointsNum");
        removePointsOnReachLimit = serializedObject.FindProperty("removePointsOnReachLimit");

        generateOverTime = serializedObject.FindProperty("generateOverTime");
        timeBetweenGenerations = serializedObject.FindProperty("timeBetweenGenerations");
        numPointsToGenerateT = serializedObject.FindProperty("numPointsToGenerateT");

        generate2DCollider = serializedObject.FindProperty("generate2DCollider");
        physicsMaterial2D = serializedObject.FindProperty("physicsMaterial2D");

        pathCreator = serializedObject.FindProperty("pathCreator");
        pathPoints = serializedObject.FindProperty("pathPoints");
        }

    public override void OnInspectorGUI()
        {
        serializedObject.Update();
        GenerationButtons();
        GUIStyle headerStyle = new GUIStyle(EditorStyles.foldout);
        headerStyle.fontStyle = FontStyle.Bold;
        ShowGeneratorSettings(headerStyle);
        serializedObject.ApplyModifiedProperties();
        }

    protected virtual void ShowGeneratorSettings(GUIStyle headerStyle)
        {
        GUILayout.BeginVertical(EditorStyles.helpBox);
            {
            EditorGUI.indentLevel++;
            ShowPath(headerStyle);
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

    protected virtual void ShowSettings(GUIStyle headerStyle)
        {
        settingsFoldOut = EditorGUILayout.Foldout(settingsFoldOut, "Settings", true, headerStyle);
        if (settingsFoldOut && showSettings)
            {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(generateMultiplePoints);
            EditorGUILayout.PropertyField(is3D, new GUIContent("Is a 3D path?"));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(pointsToAdd, new GUIContent("Number of Points to Add"));
            EditorGUILayout.PropertyField(startPointsNum);
            EditorGUILayout.PropertyField(startPoints, true);
            }
        }

    protected virtual void ShowCollider(GUIStyle headerStyle)
        {
        colliderFoldOut = EditorGUILayout.Foldout(colliderFoldOut, "Collider", true, headerStyle);
        if (colliderFoldOut && showColliderSettings)
            {
            EditorGUILayout.PropertyField(generate2DCollider);
            EditorGUILayout.PropertyField(physicsMaterial2D);
            }
        }

    protected virtual void ShowPath(GUIStyle headerStyle)
        {
        pathFoldOut = EditorGUILayout.Foldout(pathFoldOut, "Path", true, headerStyle);
        if (pathFoldOut)
            {
            EditorGUILayout.PropertyField(pathCreator);
            EditorGUILayout.PropertyField(pathPoints, true);
            }
        }

    protected virtual void ShowTime(GUIStyle headerStyle)
        {
        timeFoldOut = EditorGUILayout.Foldout(timeFoldOut, "Time", true, headerStyle);
        if (timeFoldOut && showGenerateOverTimeSettings)
            {
            EditorGUILayout.PropertyField(generateOverTime, new GUIContent("Generate over time?"));
            EditorGUILayout.PropertyField(timeBetweenGenerations);
            EditorGUILayout.PropertyField(numPointsToGenerateT, new GUIContent("Num Points To Generate"));
            }
        }

    protected virtual void ShowLimits(GUIStyle headerStyle)
        {
        limitsFoldOut = EditorGUILayout.Foldout(limitsFoldOut, "Limits", true, headerStyle);
        if (limitsFoldOut && showLimitsSettings)
            {
            GUILayout.BeginHorizontal(); 
            EditorGUILayout.PropertyField(limitPoints, new GUIContent("Limit point number?"));
            EditorGUILayout.PropertyField(removePointsOnReachLimit, new GUIContent("Remove after limit?"));
            GUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(maxPointsNum);
            }
        }

    protected virtual void GenerationButtons()
        {
        GUILayout.BeginHorizontal();
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
        GUILayout.EndVertical();
        }

    public void DrawUILine(int thickness = 1, int padding = 5)
        {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, lineColor);
        }
    }