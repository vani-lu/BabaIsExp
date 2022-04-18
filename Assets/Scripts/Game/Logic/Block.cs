using UnityEngine;
using System;

namespace Gfen.Game.Logic
{
    [Serializable]
    public class Block
    {
        public int entityType;

        public Vector2Int position;

        public Direction direction;

        public Block(int type, Vector2Int pos, Direction dir){

            entityType = type;
            position = pos;
            direction = dir;
            
        }
    }
}
