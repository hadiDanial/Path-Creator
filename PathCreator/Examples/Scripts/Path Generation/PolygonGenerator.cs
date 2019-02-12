using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PolygonGenerator : PathGenerator
    {
    [Range(3, 20), Tooltip("Number of edges the polygon will have.")]
    public int numEdges = 4;
    [Range(0.1f, 20), Tooltip("Distance between the center of the polygon and the edge of each vertex.")]
    public float centerToVertexLength= 1;
    //[Range(0.1f, 20), Tooltip("Length of each edge.")]
    //public float edgeLength = 4;
    [Range(0, 360), Tooltip("Rotate shape by offset angle.")]
    public float offsetAngle = 45f;
    [Tooltip("Should the polygon have sharp corners?")]
    public bool sharpCorners = true;
    [Range(0,3), Tooltip("Value the length of control points will be multiplied by. 0.35f is good for circles. The lower this value is, the sharper the edges. A value of 0 is the same as having sharp corners turned on.")]
    public float autoControlLength = 0.35f;
    [Tooltip("Add insets?")]
    public bool inset = false;
    [Tooltip("Divide the edges? If true, adds points on each edge.")]
    public bool divideEdges = false;
    [Range(2, 20), Tooltip("Number of points to add per edge.")]
    public int divisionsPerEdge = 2;
    [Range(0, 1), Tooltip("How far in should the inset go?")]
    public float insetPercent = 0;
    [Range(2,10), Tooltip("Inset is applied to every n-th point. For example, is set to 2, then every 2nd point will be inset. If 3, then every 3rd point, etc...")]
    public int insetEveryNthPoint = 2;
    [Tooltip("Origin point the shape is built around")]
    public Vector3 originPoint = Vector3.zero;
    [Tooltip("The axis the polygon is aligned to.")]
    public PathAxis axis = PathAxis.Y;

    private float[] angles;
    private Vector2[] verticesDir;
    private int numPoints;
    /// <summary>
    /// Generates polygon
    /// </summary>
    public override void GeneratePoint()
        {
        BuildPath();
        }

    protected override void Start()
        {
        InitializeGenerator();
        }

    public override void InitializeGenerator()
        {
        SetPathCollider();
        generateOverTime = false;
        is3D = false;
        pathCreator = GetComponent<PathCreator>();
        pathPoints = new List<Vector3>();
        hasBeenInitialized = true;
        InitVars();
        }

    // Initialize required variables
    private void InitVars()
        {
        SetNumPoints();
        angles = new float[numEdges];
        verticesDir = new Vector2[numPoints];
        }

    private void SetNumPoints()
        {
        if (numEdges < 3)
            {
            numEdges = 3;
            }
        if (divideEdges && divisionsPerEdge > 0)
            {
            numPoints = numEdges * divisionsPerEdge;
            }
        else
            {
            numPoints = numEdges;
            }
        }

    protected override void BuildPath()
        {

        //if (inset)
        //    {
        //    numPoints *= 2;
        //    }
        InitVars();
        Clear();

        float anglePerEdge = 360 / numEdges;

        for (int i = 0; i < numEdges; i++)
            {
            angles[i] = (i * anglePerEdge) + offsetAngle;
            //print(angles[i]);
            }
        pathPoints = new List<Vector3>();

        if (divideEdges)
            {
            for (int i = 0; i < numEdges; i++)
                {
                float x, y;              
                x = centerToVertexLength * Mathf.Cos(angles[i] * Mathf.Deg2Rad);
                y = centerToVertexLength * Mathf.Sin(angles[i] * Mathf.Deg2Rad);
                verticesDir[i * divisionsPerEdge] = new Vector2(x, y).normalized;
                }
            int indexCounter = 1;
            for (int i = 0; i < numEdges; i++)
                {
                int currentIndex = (i * divisionsPerEdge), nextIndex = ((i * divisionsPerEdge) + divisionsPerEdge) % verticesDir.Length;
                Vector2 dir = (verticesDir[nextIndex] - verticesDir[currentIndex]).normalized;
                float distance = Vector2.Distance(verticesDir[nextIndex], verticesDir[currentIndex]);
                //print("Direction: " + dir + ", distance: " + distance);
                for (int j = 0; j < divisionsPerEdge - 1; j++)
                    {
                    int mod = (j+1) % divisionsPerEdge;
                    //print("Index: " + indexCounter + ", Mod: " + mod);
                    verticesDir[indexCounter] = verticesDir[currentIndex] + (dir * (distance * ((float)mod / divisionsPerEdge)));
                    indexCounter++;
                    }
                indexCounter++;
                }
            for (int i = 0; i < numPoints; i++)
                {
                if (inset && (i + 1) % insetEveryNthPoint == 0) // if (inset && i % 2 == 1)
                    {
                    verticesDir[i] *= insetPercent;
                    }
                }
            }
       else
            {
            for (int i = 0; i < numPoints; i++)
                {
                float x, y;
                x = centerToVertexLength * Mathf.Cos(angles[i] * Mathf.Deg2Rad);
                y = centerToVertexLength * Mathf.Sin(angles[i] * Mathf.Deg2Rad);
                verticesDir[i] = new Vector2(x, y).normalized;
                if (inset && (i + 1) % insetEveryNthPoint == 0) // if (inset && i % 2 == 1)
                    {
                    verticesDir[i] *= insetPercent;
                    }
                }           
            }

        switch (axis)
            {
            case PathAxis.X:
                    {
                    for (int i = 0; i < verticesDir.Length; i++)
                        {
                        pathPoints.Add(new Vector3(0,verticesDir[i].x,verticesDir[i].y) * centerToVertexLength + originPoint);
                        }
                    }
                break;
            case PathAxis.Y:
                    {
                    for (int i = 0; i < verticesDir.Length; i++)
                        {
                        pathPoints.Add(new Vector3(verticesDir[i].y, 0, verticesDir[i].x) * centerToVertexLength + originPoint);
                        }
                    }
                break;
            case PathAxis.Z:
                    {
                    for (int i = 0; i < verticesDir.Length; i++)
                        {
                        pathPoints.Add(new Vector3(verticesDir[i].y, verticesDir[i].x, 0) * centerToVertexLength + originPoint);
                        }
                    }
                break;
            case PathAxis.Custom:
                    {
                    for (int i = 0; i < verticesDir.Length; i++)
                        {
                        pathPoints.Add(new Vector3(0, verticesDir[i].x, verticesDir[i].y) * centerToVertexLength + originPoint);
                        }
                    Debug.LogWarning("Custom Axis has not been implemented yet. Defaulting to X.");
                    }
                break;
            default:
                    {
                    for (int i = 0; i < verticesDir.Length; i++)
                        {
                        pathPoints.Add(new Vector3(0, verticesDir[i].x, verticesDir[i].y) * centerToVertexLength + originPoint);
                        }
                    }
                break;
            }

        pathCreator.bezierPath = new BezierPath(pathPoints, true, space);
        if (sharpCorners)
            {
            pathCreator.bezierPath.AutoControlLength = 0;
            pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Free;
            }
        else
            {
            pathCreator.bezierPath.AutoControlLength = autoControlLength;
            pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Aligned;
            }
        //BuildCollider();
        }

    public int GetNumPoints()
        {
        SetNumPoints();
        return numPoints;
        }
    }
