using UnityEngine;

namespace Gfen.Game.Logic
{
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
