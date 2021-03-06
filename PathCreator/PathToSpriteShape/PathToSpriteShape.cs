﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using PathCreation;
using System;

public static class PathToSpriteShape
{
    const float SCALE = 0.35f;//4.25f;

    /// <summary>
    /// Updates a sprite shape.
    /// </summary>
    /// <param name="controller">Sprite Shape Controller</param>
    /// <param name="pathCreator">Path Creator</param>
    /// <param name="j">Index of last bezier path segment updated</param>
    public static int UpdateSpriteShape(SpriteShapeController controller, PathCreator pathCreator, int j, ShapeTangentMode tangentMode = ShapeTangentMode.Continuous, bool sharpCorners = false)
        {
        Spline spline = controller.spline;
        spline.isOpenEnded = !pathCreator.path.isClosedLoop;
        int numSegments = pathCreator.bezierPath.NumSegments;
        int index = spline.GetPointCount();
        Vector3[] segment;
        if(index == 2 || index == 0)
            {
            spline.Clear();
            index = 0;
            segment = pathCreator.bezierPath.GetPointsInSegment(0);
            InsertPointBezier(spline, segment[0], index);
            index++;
            InsertPointBezier(spline, segment[3], index);
            index++;
            }
        for (int i = j + 1; i < numSegments; i++)
            {
            if(!(i==numSegments-1 && !spline.isOpenEnded))
                {
                segment = pathCreator.bezierPath.GetPointsInSegment(i);
                InsertPointBezier(spline, segment[3], index);
                index++;
                j = i;
                }
            }

        if (!sharpCorners)
            {
            spline.SetTangentMode(0, ShapeTangentMode.Continuous);
            spline.SetRightTangent(0, pathCreator.path.anchorTangents[0] * SCALE);

            int anchorT = 1;
            for (int i = 1; i < spline.GetPointCount() - 1; i++)
                {
                spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                spline.SetLeftTangent(i, -pathCreator.path.anchorTangents[anchorT] * SCALE);
                anchorT++;
                spline.SetRightTangent(i, pathCreator.path.anchorTangents[anchorT] * SCALE);
                anchorT++;

                }
            spline.SetLeftTangent(spline.GetPointCount() - 1, -pathCreator.path.anchorTangents[pathCreator.path.anchorTangents.Length - 1] * SCALE);
            if (!spline.isOpenEnded)
                {
                spline.SetLeftTangent(0, -pathCreator.path.anchorTangents[0] * SCALE);
                spline.SetTangentMode(0, ShapeTangentMode.Continuous);
                spline.SetRightTangent(spline.GetPointCount() - 1, pathCreator.path.anchorTangents[pathCreator.path.anchorTangents.Length - 1] * SCALE);
                spline.SetTangentMode(spline.GetPointCount() - 1, ShapeTangentMode.Continuous);
                }

            }
        else
            {
            for (int i = 0; i < spline.GetPointCount() - 1; i++)
                {
                spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                spline.SetLeftTangent(i, Vector3.zero);
                spline.SetRightTangent(i, Vector3.zero);
                }
            }
        
        return j;
        }

    internal static void Clear(SpriteShapeController spriteShapeController)
        {
        spriteShapeController.spline.Clear(); 
        }

    public static void RemovePoints()
        {
        throw new System.NotImplementedException();
        }
    private static void InsertPointBezier(Spline spline, Vector3 point, int index)
        {
        spline.InsertPointAt(index, point);
        spline.SetTangentMode(index, ShapeTangentMode.Continuous);
        }

    }
