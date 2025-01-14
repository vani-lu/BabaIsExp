using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

    [Serializable]
    public class BlockListWrapper
    {
        public int timestamp = 0;
        public Vector2Int size;
        public List<Block> blocks;

        public async Task Save(string path)
        {
            var json = JsonUtility.ToJson(this, false);
            using (StreamWriter file = new StreamWriter(path, append: true))
            {
                await file.WriteLineAsync(json);
            }
        }
    }

}
