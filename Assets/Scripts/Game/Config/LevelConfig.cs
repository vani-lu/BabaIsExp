using System;
using UnityEngine;

namespace Gfen.Game.Config
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "babaisyou/LevelConfig", order = 0)]
    public class LevelConfig : ScriptableObject
    {
        public string levelName;

        [TextArea]
        public string hintText = "";

        public MapConfig map;
    }
}
