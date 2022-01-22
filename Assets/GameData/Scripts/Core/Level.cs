using Match3.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Core
{
    public class Level
    {
        public int id;
        public int boardWidth;
        public int boardHeight;
        public int targetScore;
        public int moves;
        public List<PieceColor> availablePieces = new List<PieceColor>();
        public List<LevelPiece> levelPieces;
    }

    [System.Serializable]
    public class LevelPiece
    {
        public PieceColor pieceColor;
        public int pieceValue;
    }
}