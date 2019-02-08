using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

/// <summary>
/// Generates a random 2D or 3D path, with respect to the last point and the min/max distances.
/// </summary>
public class RandomPathGenerator : PathGenerator
{
    [Header("Generation Settings")]
    public Vector3 minDistance = new Vector3(5, 2, 5);
    public Vector3 maxDistance = new Vector3(20, 10, 20);
    [Tooltip("Multiplies y-value with a sign that changes between positive and negative 1.")]
    public bool alternateY = true;

    protected override IEnumerator IGeneratePath()
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
            }
        }
    protected override void BuildPath()
        {
        space = is3D ? PathSpace.xyz : PathSpace.xy;
        pathCreator.bezierPath.Space = space;

        for (int i = 0; i < startPointsNum; i++)
            {
            GeneratePoint();
            }
        lastPoint = pathPoints[pathPoints.Count - 1];
        base.BuildPath();
        }

    /// <summary>
    /// Generates a random 2D or 3D point to add to the path, with respect to the last point and the min/max distances.
    /// </summary>
    public override void GeneratePoint()
        {
        if (generateMultiplePoints)
            {
            for (int i = 0; i < pointsToAdd; i++)
                {
                GenerateRandomPoint();
                }
            }
        else
            GenerateRandomPoint();

        }

    private void GenerateRandomPoint()
        {
        if (CanAddPoints())
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
                Vector2 newPoint = new Vector2(x, y) + (Vector2)lastPoint;
                pathPoints.Add(newPoint);
                lastPoint = newPoint;
                }

            RebuildPath();
            }
        else
            {
            RemovePointsAndGenerate();
            }
        }

    /// <summary>
    /// Rebuilds the path with the newly generated points.
    /// </summary>
    protected override void RebuildPath()
        {
        space = is3D ? PathSpace.xyz : PathSpace.xy;
        pathCreator.bezierPath.AddSegmentToEnd(lastPoint);
        //pathPoints.Add(lastPoint);
        pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Mirrored;
        BuildCollider();
        }
    }
