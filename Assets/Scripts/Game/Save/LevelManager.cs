using System.IO;
using Gfen.Game.Utility;
using UnityEngine;

namespace Gfen.Game.Manager
{
    public class LevelManager
    {
        private const string InfoKey = "LevelManagerInfo";

        private const string UserInfoKey = "UserName";

        private const string DateInfoKey = "LoginDate";

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

        // A chapter is avaible when all the previous chapters are passed
        public bool IsChapterAvailable(int chapterIndex)
        {
            // Passed chapters can be revisited
            if (IsChapterPassed(chapterIndex)){
                return true;
            }

            for (int i = chapterIndex - 1; i >= 0; i--){
                if (!IsChapterPassed(i)){

                    Debug.Log("Chapter" + i +  "Not Passed");
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

            for (int i = levelIndex - 1; i >= 0; i--){
                if (!IsLevelPassed(chapterIndex, i)){
                    return false;
                }
            }

            return true;
        }

        public void SetStayChapterIndex(int chapterIndex)
        {
            m_managerInfo.lastStayChapterIndex = chapterIndex;

            SaveInfo();
        }

        public int GetStayChapterIndex()
        {
            return m_managerInfo.lastStayChapterIndex;
        }

        public void PassLevel(int chapterIndex, int levelIndex)
        {
            var chapterInfo = m_managerInfo.chapterInfoDict.GetOrSet(chapterIndex, () => new ChapterInfo());
            chapterInfo.levelInfoDict[levelIndex] = 1;

            SaveInfo();
        }

        public int CountBonus(){
            if (IsLevelPassed(2,1)){
                return 2;
            }
            else if (IsLevelPassed(2,2)){
                return 3;
            }
            return 1;
        }
    }
}
