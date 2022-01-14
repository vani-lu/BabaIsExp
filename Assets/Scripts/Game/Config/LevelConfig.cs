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
        public string hintText = "巴巴是你是一款具有颠覆性创意的游戏，核心玩法就是通过改变场景的规则来过关。";

        public MapConfig map;
    }
}
