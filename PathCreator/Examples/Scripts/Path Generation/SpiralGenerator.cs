using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

/// <summary>
/// Generates a spiral path along a given axis.
/// </summary>
public class SpiralGenerator : PathGenerator
    {
    //[Header("Spiral Path")]
    [Tooltip("Width, Height and Depth of the spiral.")]
    public Vector3 spiralLength = Vector3.one * 50;
    [Tooltip("Point the spiral revolves around.")]
    public Vector3 spiralOrigin = Vector3.zero;
    [Tooltip("The axis the spiral is generated on.")]
    public PathAxis spiralDir = PathAxis.Y;
    [Tooltip("Add the spiral variation?")]
    public bool useSpiralVariation = true;
    [Tooltip("How much to add to the spiral on each axis in each cycle.")]
    public Vector3 spiralVariation;
    [Tooltip("Should the spiral go clockwise or anti-clockwise?")]
    public bool clockwiseSpiral = true;

    private float spiralX, spiralY, spiralZ;

    protected override void BuildPath()
        {
        space = is3D ? PathSpace.xyz : PathSpace.xy;
        is3D = true;
        spiralX = spiralLength.x;
        spiralY = spiralLength.y;
        spiralZ = spiralLength.z;
        AddStartPoints();
        pathPoints.Add(spiralOrigin); // Starting point for the spiral.
        GeneratePoint();
        base.BuildPath();
        }

    /// <summary>
    /// Generates 4 points as a single 'cycle' of the spiral.
    /// </summary>
    public override void GeneratePoint()
        {
        if (generateMultiplePoints)
            {
            for (int i = 0; i < pointsToAdd; i++)
                {
                GenerateSpiralCycle();
                }
            }
        else
            {
            GenerateSpiralCycle();
            }
        }

    private void GenerateSpiralCycle()
        {
        if (CanAddPoints())
            {
            Vector3[] cyclePoints = new Vector3[4];
            float axisVal;
            float xAddition, yAddition, zAddition;
            generatedPoints = new List<Vector3>();
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
                case PathAxis.X:
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
                case PathAxis.Y:
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
                case PathAxis.Z:
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
                        {
                        Debug.LogWarning("ERROR! "+ gameObject.name + ": Spiral Direction: Not implemented yet. Please use X, Y, or Z.");
                        return;
                        }
                }

            if (clockwiseSpiral)
                {
                generatedPoints.Add(cyclePoints[0]);
                generatedPoints.Add(cyclePoints[1]);
                generatedPoints.Add(cyclePoints[2]);
                generatedPoints.Add(cyclePoints[3]);
                lastPoint = cyclePoints[3];
                }
            else
                {
                switch (spiralDir)
                    {
                    case PathAxis.X:
                            {
                            float temp = cyclePoints[0].y;
                            cyclePoints[0] = new Vector3(cyclePoints[0].x, cyclePoints[2].y, cyclePoints[0].z);
                            cyclePoints[2] = new Vector3(cyclePoints[2].x, temp, cyclePoints[2].z);
                            }
                        break;
                    case PathAxis.Y:
                            {
                            float temp = cyclePoints[0].x;
                            cyclePoints[0] = new Vector3(cyclePoints[2].x, cyclePoints[0].y, cyclePoints[0].z);
                            cyclePoints[2] = new Vector3(temp, cyclePoints[2].y, cyclePoints[2].z);
                            }
                        break;
                    case PathAxis.Z:
                            {
                            float temp = cyclePoints[0].x;
                            cyclePoints[0] = new Vector3(cyclePoints[2].x, cyclePoints[0].y, cyclePoints[0].z);
                            cyclePoints[2] = new Vector3(temp, cyclePoints[2].y, cyclePoints[2].z);
                            }
                        break;
                    default:
                        break;
                    }
                generatedPoints.Add(cyclePoints[0]);
                generatedPoints.Add(cyclePoints[1]);
                generatedPoints.Add(cyclePoints[2]);
                generatedPoints.Add(cyclePoints[3]);
                lastPoint = cyclePoints[3];
                }

            RebuildPath();
            }

        else
            {
            RemovePointsAndGenerate();
            }
        }

    /// <summary>
    /// Generates points over time.
    /// </summary>
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
            RebuildPath();
            }
        }

    /// <summary>
    /// Removes a number of points from the start of the path and generates a single cycle in their place, IF removePointsOnReachLimit is true.
    /// </summary>
    /// <param name="numToRemove">The number of points to remove. Defaults to 4.</param>
    protected override void RemovePointsAndGenerate(int numToRemove = 4)
        {
        if (removePointsOnReachLimit)
            {
            for (int i = 0; i < numToRemove; i++)
                {
                pathPoints.RemoveAt(0);
                pathCreator.bezierPath.DeleteSegment(0);
                }
            GeneratePoint();
            }
        else
            {
            Debug.LogWarning(gameObject.name + ": Can't add spiral points! Limit reached, and point removal is disabled.");
            }
        }
    /// <summary>
    /// Removes a number of points from the start of the path.
    /// </summary>
    /// <param name="numToRemove">The number of points to remove. Defaults to 4.</param>
    protected override void RemovePoint(int numToRemove = 4)
        {
        if (removePointsOnReachLimit)
            {
            for (int i = 0; i < numToRemove; i++)
                {
                pathPoints.RemoveAt(0);
                pathCreator.bezierPath.DeleteSegment(0);
                }
            }
        }

    /// <summary>
    /// Can you add a (number of) point(s) without going over the maximum?
    /// </summary>
    /// <param name="numToAdd">Number of points to add. Defaults to 4.</param>
    protected override bool CanAddPoints(int numToAdd = 4)
        {
        if (limitPoints)
            {
            return (pathPoints.Count + numToAdd) <= maxPointsNum;
            }
        else
            {
            return true;
            }
        }
    }