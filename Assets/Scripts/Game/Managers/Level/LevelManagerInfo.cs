using System;
using Gfen.Game.Common;

namespace Gfen.Game.Manager{
    
    [Serializable]
    public class LevelManagerInfo
    {
        public int lastStayChapterIndex = -1;

        public SerializableDictionaryOfIntAndChapterInfo chapterInfoDict = new SerializableDictionaryOfIntAndChapterInfo();
    }

    // Key: Chapter Index
    // Value: Level Information
    [Serializable]
    public class SerializableDictionaryOfIntAndChapterInfo : SerializableDictionary<int, ChapterInfo> { }


    [Serializable]
    public class ChapterInfo
    {
        // Key: Level Index
        // Value: Level Passed
        public SerializableDictionaryOfIntAndInt levelInfoDict = new SerializableDictionaryOfIntAndInt();
    }
}
