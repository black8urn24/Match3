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
        Orange,
        Yellow,
        Wild,
        Collectable,
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

    public enum PoolObjectsType
    {
        PieceClearEffect,
        SingleBreakableTileEffect,
        DoubleBreakableTileEffect,
        BombClearEffect,
        MusicAudioSource,
        BonusAudioSource,
        SFXAudioSource,
        WinAudioSource,
        LooseAudioSource,
        ScoreStarAudioSource,
        ClockTickAudioSource,
        None
    }

    public enum BombType
    {
        None,
        Color,
        Adjacent,
        Coloumn,
        Row
    }

    public enum CollectableType
    {
        ClearedAtBottom,
        ClearedByBomb,
        None
    }

    public enum PieceColor
    {
        Blue,
        Cyan,
        Green,
        Indigo,
        Magenta,
        Red,
        Orange,
        Yellow,
        None
    }
}