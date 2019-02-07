using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;
using UnityEngine.U2D;
using System;

/// <summary>
/// Path generator that generates a path procedurally.
/// </summary>

[RequireComponent(typeof(PathCreator))]
public class PathGenerator : MonoBehaviour
    {
    [Header("Spawn Settings")]
    [Range(0.5f, 5f), Tooltip("Amount of time between spawns")]
    public float timeBetweenSpawns = 2f;
    [Range(0, 20), Tooltip("Number of points to spawn each time")]
    public int numPointsToSpawn = 4;
    [SerializeField, Tooltip("Spawn more points?")]
    private bool spawn = true;

    [Header("Generation Settings")]
    public Vector3 minDistance = new Vector3(5, 2, 5);
    public Vector3 maxDistance = new Vector3(20, 10, 20);
    [Tooltip("Use z-axis")]
    public bool is3D = false;
    [Tooltip("Multiplies y-value with a sign that changes between positive and negative 1.")]
    public bool alternateY = true;
    [Tooltip("Number of points to generate when the script first starts.")]
    [Range(0, 50)]
    public int startPointsNum = 10;
    [Tooltip("Custom positions to add to the start of the path. If empty, doesn't generate any predefined points.")]
    public Transform[] startPoints;

    [Header("Spiral Path")]
    [Tooltip("If on, generates a spiral, sets is3D to true, and alternateY to false.")]
    public bool isSpiral;
    [Tooltip("Width, Height and Depth of the spiral.")]
    public Vector3 spiralLength = Vector3.one * 50;
    [Tooltip("Point the spiral revolves around.")]
    public Vector3 spiralOrigin = Vector3.zero;
    [Tooltip("The axis the spiral is generated on.")]
    public SpiralDirection spiralDir = SpiralDirection.Y;
    [Tooltip("Add the spiral variation?")]
    public bool useSpiralVariation = true;
    [Tooltip("How much to add to the spiral in each axis.")]
    public Vector3 spiralVariation;
    [Tooltip("Should the spiral go clockwise or anti-clockwise?")]
    public bool clockwiseSpiral = true;

    [Header("Collider")]
    public bool generateCollider = true;
    public PhysicsMaterial2D physicsMaterial2D;
    private PathCollider pathCollider;

    [Header("Path")]
    public PathCreator pathCreator;
    public List<Vector3> pathPoints;

    private Vector2 lastPoint;
    private int sign = 1;
    private PathSpace space;

    private float spiralX, spiralY, spiralZ;

    void Start()
        {
        SetPathCollider();
        SetPathCreator();
        StartCoroutine(IGeneratePath());
        }
    private void Update()
        {
        if (Input.GetMouseButtonDown(2))
            {
            spawn = !spawn;
            if (spawn)
                StartCoroutine(IGeneratePath());
            }
        }
    private IEnumerator IGeneratePath()
        {
        WaitForSeconds wait = new WaitForSeconds(timeBetweenSpawns);
        while (spawn)
            {
            for (int i = 0; i < numPointsToSpawn; i++)
                {
                if (isSpiral)
                    {
                    GenerateSpiralPoint();
                    }
                else
                    {
                    GenerateRandomPoint();
                    }
                }
            yield return wait;
            RebuildPath();
            }
        }

    private void GenerateSpiralPoint()
        {
        Vector3[] cyclePoints = new Vector3[4];
        float axisVal;
        float xAddition, yAddition, zAddition;

        if (useSpiralVariation)
            {
            xAddition = spiralVariation.x;
            yAddition = spiralVariation.y;
            zAddition = spiralVariation.z;
            }
        else
            {
            xAddition = 0;
            yAddition = 0;
            zAddition = 0;
            }
        switch (spiralDir) // AXIS
            {
            case SpiralDirection.X:
                    {
                    axisVal = spiralLength.x / 4;
                    if (useSpiralVariation) spiralY += yAddition;
                    cyclePoints[0] = new Vector3(axisVal + spiralX, spiralOrigin.y + (spiralY + yAddition), spiralOrigin.z);
                    if (useSpiralVariation) spiralZ += zAddition;
                    cyclePoints[1] = new Vector3((axisVal * 2) + spiralX, spiralOrigin.y, spiralOrigin.z + (spiralZ + zAddition));
                    if (useSpiralVariation) spiralY += yAddition;
                    cyclePoints[2] = new Vector3((axisVal * 3) + spiralX, spiralOrigin.y + (-spiralY - yAddition), spiralOrigin.z);
                    if (useSpiralVariation) spiralZ += zAddition;
                    cyclePoints[3] = new Vector3((axisVal * 4) + spiralX, spiralOrigin.y, spiralOrigin.z + (-spiralZ - zAddition));
                    spiralX += spiralLength.x;
                    }
                break;
            case SpiralDirection.Y:
                    {
                    axisVal = spiralLength.y / 4;
                    if (useSpiralVariation) spiralX += xAddition;
                    cyclePoints[0] = new Vector3(spiralOrigin.x + (spiralX + xAddition), axisVal + spiralY, spiralOrigin.z);
                    if (useSpiralVariation) spiralZ += zAddition;
                    cyclePoints[1] = new Vector3(spiralOrigin.x, (axisVal * 2) + spiralY, spiralOrigin.z + (spiralZ + zAddition));
                    if (useSpiralVariation) spiralX += xAddition;
                    cyclePoints[2] = new Vector3(spiralOrigin.x + (-spiralX - xAddition), (axisVal * 3) + spiralY, spiralOrigin.z);
                    if (useSpiralVariation) spiralZ += zAddition;
                    cyclePoints[3] = new Vector3(spiralOrigin.x, (axisVal * 4) + spiralY, spiralOrigin.z + (-spiralZ - zAddition));
                    spiralY += spiralLength.y;
                    }
                break;
            case SpiralDirection.Z:
                    {
                    axisVal = spiralLength.z / 4;
                    if (useSpiralVariation) spiralX += xAddition;
                    cyclePoints[0] = new Vector3(spiralOrigin.x + (spiralX + xAddition), spiralOrigin.y, axisVal + spiralZ);
                    if (useSpiralVariation) spiralY += yAddition;
                    cyclePoints[1] = new Vector3(spiralOrigin.x, spiralOrigin.y + (spiralY + yAddition), (axisVal * 2) + spiralZ);
                    if (useSpiralVariation) spiralX += xAddition;
                    cyclePoints[2] = new Vector3(spiralOrigin.x + (-spiralX - xAddition), spiralOrigin.y, (axisVal * 3) + spiralZ);
                    if (useSpiralVariation) spiralY += yAddition;
                    cyclePoints[3] = new Vector3(spiralOrigin.x, spiralOrigin.y + (-spiralY - yAddition), (axisVal * 4) + spiralZ);
                    spiralZ += spiralLength.z;
                    }
                break;
            default:
                break;
            }

        if (clockwiseSpiral)
            {
            pathPoints.Add(cyclePoints[0]);
            pathPoints.Add(cyclePoints[1]);
            pathPoints.Add(cyclePoints[2]);
            pathPoints.Add(cyclePoints[3]);
            lastPoint = cyclePoints[3];
            }
        else
            {
            switch (spiralDir)
                {
                case SpiralDirection.X:
                        {
                        float temp = cyclePoints[0].y;
                        cyclePoints[0] = new Vector3(cyclePoints[0].x, cyclePoints[2].y, cyclePoints[0].z);
                        cyclePoints[2] = new Vector3(cyclePoints[2].x, temp, cyclePoints[2].z);
                        }
                    break;
                case SpiralDirection.Y:
                        {
                        float temp = cyclePoints[0].x;
                        cyclePoints[0] = new Vector3(cyclePoints[2].x, cyclePoints[0].y, cyclePoints[0].z);
                        cyclePoints[2] = new Vector3(temp, cyclePoints[2].y, cyclePoints[2].z);
                        }
                    break;
                case SpiralDirection.Z:
                        {
                        float temp = cyclePoints[0].x;
                        cyclePoints[0] = new Vector3(cyclePoints[2].x, cyclePoints[0].y, cyclePoints[0].z);
                        cyclePoints[2] = new Vector3(temp, cyclePoints[2].y, cyclePoints[2].z);
                        }
                    break;
                default:
                    break;
                }
            pathPoints.Add(cyclePoints[0]);
            pathPoints.Add(cyclePoints[1]);
            pathPoints.Add(cyclePoints[2]);
            pathPoints.Add(cyclePoints[3]);
            lastPoint = cyclePoints[3];
            }

        RebuildPath();
        }

    private void GenerateRandomPoint()
        {
        float x = UnityEngine.Random.Range(minDistance.x, maxDistance.x);
        float y = UnityEngine.Random.Range(minDistance.y, maxDistance.y);
        float z;
        if (alternateY)
            {
            y *= sign;
            sign *= -1;
            }
        if (is3D)
            {
            z = UnityEngine.Random.Range(minDistance.z, maxDistance.z);
            Vector3 newPoint = new Vector3(x, y, z) + (Vector3)lastPoint;
            pathPoints.Add(newPoint);
            lastPoint = newPoint;
            }
        else
            {
            Vector2 newPoint = new Vector2(x, y) + lastPoint;
            pathPoints.Add(newPoint);
            lastPoint = newPoint;
            }

        }

    private void RebuildPath()
        {
        space = is3D ? PathSpace.xyz : PathSpace.xy;
        pathCreator.bezierPath = new BezierPath(pathPoints, false, space);
        pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Mirrored;
        SetPathCollider();
        }

    private void SetPathCreator()
        {
        pathCreator = GetComponent<PathCreator>();
        pathPoints = new List<Vector3>();
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
        if (isSpiral)
            {
            pathPoints.Add(spiralOrigin); // Starting point for the spiral.
            space = PathSpace.xyz;
            alternateY = false;
            is3D = true;
            spiralX = spiralLength.x;
            spiralY = spiralLength.y;
            spiralZ = spiralLength.z;
            GenerateSpiralPoint();
            }
        else
            {
            space = is3D ? PathSpace.xyz : PathSpace.xy;
            pathCreator.bezierPath.Space = space;

            for (int i = 0; i < startPointsNum; i++)
                {
                GenerateRandomPoint();
                }
            lastPoint = pathPoints[pathPoints.Count - 1];
            }


        pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Mirrored;
        pathCreator.bezierPath = new BezierPath(pathPoints, false, space);
        if (generateCollider && pathCollider != null) pathCollider.GenerateCollider();
        }

    private void SetPathCollider()
        {
        if (generateCollider)
            {
            pathCollider = GetComponent<PathCollider>();
            if (pathCollider == null)
                pathCollider = gameObject.AddComponent<PathCollider>();
            if (physicsMaterial2D != null)
                pathCollider.SetPhysicsMaterial(physicsMaterial2D);
            }
        else return;
        }
    }

public enum SpiralDirection
    {
    X, Y, Z
    }
