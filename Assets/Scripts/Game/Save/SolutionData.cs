using System;
using Gfen.Game.Logic;

namespace Vani.Data
{
    [Serializable]
    public class SolutionData
    {
        public int chapterIndex;

        public int levelIndex;

        public SerializableDictionaryOfIntAndAttribute ruleInfoDict = new SerializableDictionaryOfIntAndAttribute();
    }

}
