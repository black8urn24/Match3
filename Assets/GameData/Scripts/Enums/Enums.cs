using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Enums
{
    public enum GamePieceType
    {
        Blue,
        Cyan,
        Green,
        Indigo,
        Magenta,
        Red,
        Teal,
        Yellow,
        None
    }

    public enum GamePieceInterpolationType
    {
        Linear,
        EaseIn,
        EaseOut,
        SmoothStep,
        SmootherStep
    }

    public enum TileType
    {
        Normal,
        Obstacle,
        Breakable
    }
}