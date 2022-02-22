using System.IO;
using System.Linq;
using Gfen.Game.Utility;
using UnityEngine;

namespace Gfen.Game.Manager
{
    public class LevelManager
    {
        private const string InfoKey = "LevelManagerInfo";

        private GameManager m_gameManager;

        private LevelManagerInfo m_managerInfo = new LevelManagerInfo();

        private string m_levelInfoPath;

        public void Init(GameManager gameManager)
        {
            m_gameManager = gameManager;

            m_levelInfoPath = m_gameManager.DataPath + "/" + InfoKey + "_" + m_gameManager.Date + "_" + m_gameManager.User + ".json";

            // Get 0r Set
            if (File.Exists(m_levelInfoPath)){
                LoadInfo();
            }
            else {
                SetStayChapterIndex(-1);
                SaveInfo();
            }

        }

        private void LoadInfo()
        {
            var content = File.ReadAllText(m_levelInfoPath);
            JsonUtility.FromJsonOverwrite(content, m_managerInfo);
        }

        private void SaveInfo()
        {
            var content = JsonUtility.ToJson(m_managerInfo, true);
            File.WriteAllText(m_levelInfoPath, content);
        }

        public bool IsChapterPassed(int chapterIndex)
        {
            var chapterInfoDict = m_managerInfo.chapterInfoDict.GetOrDefault(chapterIndex, null);
            if (chapterInfoDict == null)
            {
                return false;
            }

            var levelConfigs = m_gameManager.gameConfig.chapterConfigs[chapterIndex].levelConfigs;
            for (var i = 0; i < levelConfigs.Length; i++)
            {
                if (chapterInfoDict.levelInfoDict.GetOrDefault(i, 0) == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsChapterAvailable(int chapterIndex)
        {
            // Passed chapters can be revisited
            if (IsChapterPassed(chapterIndex)){
                return true;
            }

            for (int i = chapterIndex - 1; i >= 0; i--){
                if (!IsChapterPassed(i)){
                    return false;
                }
            }

            return true;
        }

        public bool IsLevelPassed(int chapterIndex, int levelIndex)
        {
            var chapterInfoDict = m_managerInfo.chapterInfoDict.GetOrDefault(chapterIndex, null);
            if (chapterInfoDict == null)
            {
                return false;
            }

            return chapterInfoDict.levelInfoDict.GetOrDefault(levelIndex, 0) > 0;
        }

        // A level is avaible when all the previous levels are passed
        public bool IsLevelAvailable(int chapterIndex, int levelIndex)
        {
            // Passed levels can be revisited
            if (IsLevelPassed(chapterIndex, levelIndex)){
                // Challenge levels can only be visited once 
                if (chapterIndex == 2){
                    return false;
                }
                return true;
            }

            // first level of each chapter is unlocked 
            // when the previous chapter is passed
            if (chapterIndex > 0 && levelIndex == 0){
                if (IsChapterPassed(chapterIndex - 1)){
                    return true;
                }
                return false;
            }

            for (int i = levelIndex - 1; i >= 0; i--){
                if (!IsLevelPassed(chapterIndex, i)){
                    return false;
                }
            }

            return true;
        }

        public void SetStayChapterIndex(int chapterIndex)
        {
            PlayerPrefs.SetInt("LastStayChapterIndex", chapterIndex);
        }

        public int GetStayChapterIndex()
        {
            return PlayerPrefs.GetInt("LastStayChapterIndex", -1);
        }

        public void PassLevel(int chapterIndex, int levelIndex)
        {
            var chapterInfo = m_managerInfo.chapterInfoDict.GetOrSet(chapterIndex, () => new ChapterInfo());
            chapterInfo.levelInfoDict[levelIndex] = 1;

            SaveInfo();
        }

        public int CountBonus(){
            if (IsLevelPassed(2,2)){
                return 3;
            }
            else if (IsLevelPassed(2,1)){
                return 2;
            }
            else if (IsLevelPassed(2,0)){
                return 1;
            }
            return 0;
        }

        public float GetTimeSpent(int chapterIndex, int levelIndex)
        {
            var chapterTimerInfo = m_managerInfo.chapterTimerInfoDict.GetOrSet(chapterIndex, () => new ChapterTimerInfo());
            return chapterTimerInfo.levelTimerInfoDict.GetOrSet(levelIndex, () => 0f);
        }

        public void SetTimeSpent()
        {   
            int chapterIndex = m_gameManager.CurrentChapterIndex;
            int levelIndex = m_gameManager.CurrentLevelIndex;
            float t = m_gameManager.CurrentLevelElapsedTime;

            var chapterTimerInfo = m_managerInfo.chapterTimerInfoDict.GetOrSet(chapterIndex, () => new ChapterTimerInfo());
            chapterTimerInfo.levelTimerInfoDict[levelIndex] = t;

            SaveInfo();
        }

        public bool IsCurrentLevelTimeUp()
        {
            float t = m_gameManager.CurrentLevelElapsedTime;
            int chapterIndex = m_gameManager.CurrentChapterIndex;

            if (chapterIndex == m_gameManager.bonusChapterIndex){
                return false;
            }

            float[ ] levelLimits = {5f * 60f, 10f * 60f}; // seconds

            return t > levelLimits[chapterIndex];
        }

        public int BonusChapterTimeLeft(){

            int tLimit = 30; //minutes

            int chapterIndex = m_gameManager.CurrentChapterIndex;
            int levelIndex = m_gameManager.CurrentLevelIndex;

            var chapterTimerInfo = m_managerInfo.chapterTimerInfoDict.GetOrSet(chapterIndex, () => new ChapterTimerInfo());
            float sumT;
            if (chapterIndex == m_gameManager.bonusChapterIndex){
                sumT = chapterTimerInfo.levelTimerInfoDict.Where(x => x.Key != levelIndex).Sum(x => x.Value);
                sumT += m_gameManager.CurrentLevelElapsedTime;
            }
            else{
                sumT = chapterTimerInfo.levelTimerInfoDict.Sum(x => x.Value);
            }

            return tLimit - Mathf.FloorToInt(sumT/60f);
        }        
    }
}
