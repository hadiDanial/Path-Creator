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
        pathPoints.Clear();
        space = is3D ? PathSpace.xyz : PathSpace.xy;
        pathCreator.bezierPath.Space = space;

        for (int i = 0; i < startPointsNum; i++)
            {
            GenerateRandomPoint();
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
                }
            else
                {
                Vector2 newPoint = new Vector2(x, y) + (Vector2)lastPoint;
                pathPoints.Add(newPoint);
                }

            RebuildPath();
            }
        else
            {
            RemovePointsAndGenerate();
            }
        }

    /// <summary>
    /// Removes a number of points from the start of the path and generates new ones in their place, IF removePointsOnReachLimit is true.
    /// </summary>
    /// <param name="numToRemove">The number of points to remove. Defaults to 1.</param>
    protected override void RemovePointsAndGenerate(int numToRemove = 1)
        {
        if (removePointsOnReachLimit)
            {
            for (int i = 0; i < numToRemove; i++)
                {
                pathPoints.RemoveAt(0);
                pathCreator.bezierPath.DeleteSegment(0);
                GenerateRandomPoint();
                }
            }
        else
            {
            Debug.LogWarning(gameObject.name + ": Cannot generate more points. Limit reached, point removal disabled.");
            }
        }
    /// <summary>
    /// Rebuilds the path with the newly generated points.
    /// </summary>
    protected override void RebuildPath()
        {
        lastPoint = pathPoints[pathPoints.Count - 1];
        space = is3D ? PathSpace.xyz : PathSpace.xy;
        pathCreator.bezierPath.AddSegmentToEnd(lastPoint);
        //pathPoints.Add(lastPoint);
        pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Mirrored;
        BuildCollider();
        }
    }
