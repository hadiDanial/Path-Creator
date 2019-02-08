using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

/// <summary>
/// Abstract path generator class.
/// </summary>

[RequireComponent(typeof(PathCreator))]
public abstract class PathGenerator : MonoBehaviour
    {
    [Header("Limits")]
    [SerializeField, Tooltip("Limit maximum number of points?")]
    protected bool limitPointsNum = true;
    [Range(10, 1000), Tooltip("Maximum number of points to generate, if limitPointsNum is on.")]
    public int maxPointsNum = 250;
    [SerializeField, Tooltip("Remove points from the start of the path when the limit is reached?")]
    protected bool removePointsOnReachLimit = true;

    [Header("Settings")]
    [Tooltip("Generate more than one point at a time?")]
    public bool generateMultiplePoints = true;
    [Tooltip("Number of points to add in GeneratePoint"), Range(1, 100)]
    public int pointsToAdd = 5;
    [Tooltip("Use z-axis")]
    public bool is3D = false;
    [Tooltip("Number of points to generate when the script first starts.")]
    [Range(0, 50)]
    public int startPointsNum = 10;
    [Tooltip("Custom positions to add to the start of the path. If empty, doesn't generate any predefined points.")]
    public Transform[] startPoints;

    [Header("Generation Over Time")] // In the IGeneratePath coroutine
    [SerializeField, Tooltip("Generate more points over time?")]
    protected bool generateOverTime = false;
    [Range(0.5f, 5f), Tooltip("Amount of time between generations")]
    public float timeBetweenGenerations = 2f;
    [Range(0, 20), Tooltip("Number of points to generate each time")]
    public int numPointsToGenerateT = 4;

    [Header("Collider")]
    public bool generate2DCollider = true;
    public PhysicsMaterial2D physicsMaterial2D;
    private PathCollider pathCollider;

    [Header("Path")]
    public PathCreator pathCreator;
    public List<Vector3> pathPoints;

    protected Vector3 generatedPoint;           // If only one point it to be generated, store it here.
    protected List<Vector3> generatedPoints;    // If more than one point is generated, store newly generated points here
    protected Vector3 lastPoint;
    protected int sign = 1;
    protected PathSpace space;
    public bool hasBeenInitialized = false;
    protected virtual void Start()
        {
        InitializeGenerator();
        if (generateOverTime)
            StartCoroutine(IGeneratePath());
        }

    public virtual void InitializeGenerator()
        {
        SetPathCollider();
        lastPoint = transform.position;
        pathCreator = GetComponent<PathCreator>();
        pathPoints = new List<Vector3>();
        BuildPath();
        hasBeenInitialized = true;
        }

    private void Update()
        {
        if (Input.GetMouseButtonDown(2)) // Toggle generation over time on and off.
            {
            generateOverTime = !generateOverTime;
            Debug.Log("Generate over time toggled: " + generateOverTime);
            if (generateOverTime)
                StartCoroutine(IGeneratePath());
            }
        }

    /// <summary>
    /// Generates points over time.
    /// </summary>
    protected virtual IEnumerator IGeneratePath()
        {
        WaitForSeconds wait = new WaitForSeconds(timeBetweenGenerations);
        while (generateOverTime)
            {
            for (int i = 0; i < numPointsToGenerateT; i++)
                {
                if (CanAddPoints())
                    GeneratePoint();
                else
                    RemovePointsAndGenerate();
                }
            yield return wait;
            RebuildPath();
            }
        }

    /// <summary>
    /// Removes a number of points from the start of the path and generates new ones in their place, IF removePointsOnReachLimit is true.
    /// </summary>
    /// <param name="numToRemove">The number of points to remove. Defaults to 1.</param>
    protected virtual void RemovePointsAndGenerate(int numToRemove = 1)
        {
        if (removePointsOnReachLimit)
            {
            for (int i = 0; i < numToRemove; i++)
                {
                pathPoints.RemoveAt(0);
                pathCreator.bezierPath.DeleteSegment(0);
                GeneratePoint();
                }
            }
        else
            {
            Debug.LogWarning(gameObject.name + ": Cannot generate more points. Limit reached, point removal disabled.");
            }
        }
    /// <summary>
    /// Removes a number of points from the start of the path.
    /// </summary>
    /// <param name="numToRemove">The number of points to remove. Defaults to 1.</param>
    protected virtual void RemovePoint(int numToRemove = 1)
        {
        for (int i = 0; i < numToRemove; i++)
            {
            pathPoints.RemoveAt(0);
            pathCreator.bezierPath.DeleteSegment(0);
            }    
        }
    public virtual void Clear()
        {
        pathPoints.Clear();
        
        hasBeenInitialized = false;
        InitializeGenerator();
        }
    /// <summary>
    /// Can you add a (number of) point(s) without going over the maximum?
    /// </summary>
    /// <param name="numToAdd">Number of points to add. Defaults to 1.</param>
    protected virtual bool CanAddPoints(int numToAdd = 1)
        {
        if (limitPointsNum)
            {
            return (pathPoints.Count + numToAdd) <= maxPointsNum;
            }
        else
            {
            return true;
            }
        }

    /// <summary>
    /// Generates a point for the path.
    /// </summary>
    public abstract void GeneratePoint();

    /// <summary>
    /// Rebuilds the path with the newly generated points.
    /// </summary>
    protected virtual void RebuildPath()
        {
        space = is3D ? PathSpace.xyz : PathSpace.xy;
        if (generatedPoints == null || generatedPoints.Count == 0)
            {
            pathCreator.bezierPath = new BezierPath(pathPoints, false, space); //err.. why new? why not return here?
            }
        else
            {
            foreach (Vector3 p in generatedPoints)
                {
                pathCreator.bezierPath.AddSegmentToEnd(p);
                pathPoints.Add(p);
                }
            }
        pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Mirrored;
        BuildCollider();
        generatedPoints.Clear();
        }

    /// <summary>
    /// Builds a path from the path points list. If the list is empty or less than two, builds a path with two points instead.
    /// </summary>
    protected virtual void BuildPath()
        {
        if (pathPoints.Count < 2)
            {
            pathPoints = new List<Vector3>();
            pathPoints.Add(Vector3.zero);
            pathPoints.Add(Vector3.one);
            }
        pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Mirrored;
        pathCreator.bezierPath = new BezierPath(pathPoints, false, space);
        BuildCollider();
        }

    protected void BuildCollider()
        {
        if (generate2DCollider && pathCollider != null) pathCollider.GenerateCollider();
        }

    /// <summary>
    /// Adds the points defined in the startPoints array to the start of the path.
    /// </summary>
    protected void AddStartPoints()
        {
        if (startPoints != null && startPoints.Length > 0)
            {
            for (int i = 0; i < startPoints.Length; i++)
                {
                if (startPoints[i] != null)
                    {
                    pathPoints.Add(startPoints[i].position);
                    lastPoint = startPoints[i].position;
                    }
                }
            }
        }

    protected void SetPathCollider()
        {
        if (generate2DCollider)
            {
            pathCollider = GetComponent<PathCollider>();
            if (pathCollider == null)
                pathCollider = gameObject.AddComponent<PathCollider>();
            if (physicsMaterial2D != null)
                pathCollider.SetPhysicsMaterial2D(physicsMaterial2D);
            }
        else return;
        }
    }


/* /// <summary>
    /// Rebuilds the path with the newly generated points.
    /// </summary>
    protected virtual void RebuildPath()
        {
        space = is3D ? PathSpace.xyz : PathSpace.xy;
        if(generatedPoint != null)
            {
            pathCreator.bezierPath.AddSegmentToEnd(generatedPoint);
            pathPoints.Add(generatedPoint);
            generatedPoint = new Vector3();
            }
        else if (generatedPoints == null || generatedPoints.Count == 0)
            {
            pathCreator.bezierPath = new BezierPath(pathPoints, false, space);
            }
        else
            {
            foreach (Vector3 p in generatedPoints)
                {
                pathCreator.bezierPath.AddSegmentToEnd(p);
                pathPoints.Add(p);
                }
            generatedPoints.Clear();
            }
        pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Mirrored;
        BuildCollider();
        }
*/