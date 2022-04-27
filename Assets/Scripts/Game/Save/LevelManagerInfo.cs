using System;
using Gfen.Game.Common;

namespace Gfen.Game.Logic{
    
    [Serializable]
    public class LevelManagerInfo
    {
        public SerializableDictionaryOfIntAndChapterInfo chapterInfoDict = new SerializableDictionaryOfIntAndChapterInfo();

        public SerializableDictionaryOfIntAndChapterTimerInfo chapterTimerInfoDict = new SerializableDictionaryOfIntAndChapterTimerInfo();
    }

    // Key: Chapter Index
    // Value: Level Information
    [Serializable]
    public class SerializableDictionaryOfIntAndChapterInfo : SerializableDictionary<int, ChapterInfo> { }

    [Serializable]
    public  class SerializableDictionaryOfIntAndChapterTimerInfo : SerializableDictionary<int, ChapterTimerInfo> { }

    [Serializable]
    public class ChapterInfo
    {
        // Key: Level Index
        // Value: Level Passed
        public SerializableDictionaryOfIntAndInt levelInfoDict = new SerializableDictionaryOfIntAndInt();
    }

    [Serializable]
    public class ChapterTimerInfo
    {
        // Key: Level Index
        // Value: Time remaining
        public SerializableDictionaryOfIntAndFloat levelTimerInfoDict = new SerializableDictionaryOfIntAndFloat();
    }
}
