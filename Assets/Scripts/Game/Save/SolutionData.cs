using System;
using System.Collections.Generic;
using Gfen.Game.Logic;
using Gfen.Game.Common;

namespace Vani.Data
{
     [Serializable]
    public class SolutionData
    {
        public int chapterIndex = -1;

        public int levelIndex = -1;

        public SerializableDictionaryOfEntityAndAttributeSet ruleInfoDict = new SerializableDictionaryOfEntityAndAttributeSet();

        public SolutionData(int chapter, int level, Dictionary<int, HashSet<AttributeCategory>> dict) {
            chapterIndex = chapter;
            levelIndex = level;
            
            foreach(var item in dict)
            {
                HashSet<AttributeCategory> origin = item.Value;
                List<int> result = new List<int>();
                HashSet<AttributeCategory>.Enumerator em = origin.GetEnumerator();
                while (em.MoveNext()) {
                    result.Add((int)em.Current);
                }
                AttributeSet aSet = new AttributeSet();
                aSet.attributeList = result;
                ruleInfoDict[item.Key] = aSet;
            }
        }
    }

    [Serializable]
    public class SerializableDictionaryOfEntityAndAttributeSet : SerializableDictionary<int, AttributeSet> { }

    [Serializable]
    public class AttributeSet {
        public List<int> attributeList = new List<int>();
    }
}
