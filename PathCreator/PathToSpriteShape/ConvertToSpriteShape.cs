﻿using UnityEngine;
using PathCreation;
using UnityEngine.U2D;

/// Uses Odin Inspector, simply calls functions from the PathToSpriteShape class via a button in the editor.
[RequireComponent(typeof(PathCreator))]
[RequireComponent(typeof(SpriteShapeController))]
public class ConvertToSpriteShape : MonoBehaviour
    {
    int j = 0;

    /// <summary>
    /// Convert to sprite shape with curved corners.
    /// </summary>
    public void ConvertSmooth()
        {
        Clear();
        j = 0;
        PathCreator pathCreator = GetComponent<PathCreator>();
        pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Mirrored;
        j = PathToSpriteShape.UpdateSpriteShape(GetComponent<SpriteShapeController>(), pathCreator, j);
        }

    /// <summary>
    /// Convert to sprite shape with sharp corners.
    /// </summary>
    public void ConvertSharp()
        {
        Clear();
        j = 0;
        PathCreator pathCreator = GetComponent<PathCreator>();
        pathCreator.bezierPath.ControlPointMode = BezierPath.ControlMode.Mirrored;
        j = PathToSpriteShape.UpdateSpriteShape(GetComponent<SpriteShapeController>(), pathCreator, j, ShapeTangentMode.Broken, true);
        }

    /// <summary>
    /// Clear sprite shape.
    /// </summary>
    public void Clear()
        {
        PathToSpriteShape.Clear(GetComponent<SpriteShapeController>());
        }
    }