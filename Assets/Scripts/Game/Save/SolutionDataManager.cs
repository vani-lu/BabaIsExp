using System.IO;
using System.Threading.Tasks;
using Gfen.Game;
using Gfen.Game.Logic;
using UnityEngine;

namespace Vani.Data
{
    public class SolutionDataManager{

        private const string SolutionKey = "SolutionInfo";

        private GameManager m_gameManager;

        private LogicGameManager m_logicGameManager;

        private SolutionInfo solutionInfo = new SolutionInfo();

        private string m_solutionInfoPath;

        public void Init( GameManager gameManager, LogicGameManager logicGameManager){

            m_gameManager = gameManager;
            m_logicGameManager = logicGameManager;

            m_solutionInfoPath = m_gameManager.DataPath + "/" + SolutionKey + "_" + m_gameManager.Date + "_" + m_gameManager.User + ".json";

            // Get Or Set
            if (!File.Exists(m_solutionInfoPath)){
                File.Create(m_solutionInfoPath);
            }
            else {
                var content = File.ReadAllText(m_solutionInfoPath);
                JsonUtility.FromJsonOverwrite(content, solutionInfo);
            }

        }


        public async Task SaveSuccessRuleset() {

            // Get acitive custom rules from the attribute handler
            SolutionData sData = new SolutionData(  m_gameManager.CurrentChapterIndex, 
                                                    m_gameManager.CurrentLevelIndex,
                                                    m_logicGameManager.AttributeHandler.EntityTypeAttributeDict);
            solutionInfo.Append(sData);

            string content = JsonUtility.ToJson(solutionInfo, true);
            using StreamWriter file = new StreamWriter(m_solutionInfoPath, append: false);
            await file.WriteLineAsync(content);

        }

        public void HandleVolcanoMap(){

            int chapterIndex = m_gameManager.CurrentChapterIndex;
            int levelIndex = m_gameManager.CurrentLevelIndex;

            int volcanoChapterIndex = 2;

            bool isBagPush = false;
            bool isHotMelt = false;
            bool isPumpkinPush = false;

            // Skip other chapters
            if (chapterIndex != volcanoChapterIndex) {
                return;
            }
            if (levelIndex == 0) {
                return;
            }

            foreach (SolutionData item in solutionInfo.sList){
                if (item.chapterIndex != volcanoChapterIndex){
                    continue;
                }
                if (item.levelIndex == 0 ){
                    continue;
                }

            }

        }

    }


}

