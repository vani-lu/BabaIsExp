using System.IO;
using System.Threading.Tasks;
using Gfen.Game;
using Gfen.Game.Utility;
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

            Debug.Log("Chapter: " + chapterIndex);

            int volcanoChapterIndex = 2;

            // Skip other chapters
            if (chapterIndex != volcanoChapterIndex) {
                return;
            }
            if (levelIndex == 0) {
                return;
            }

            bool isBagPush = false; // Bag Is Push
            bool isHotMelt = false; // Bag Is Melt
            bool isPumpkinPush = false; // Pumpkin Is Push

            foreach (SolutionData item in solutionInfo.sList){
                if (item.chapterIndex == volcanoChapterIndex){
                    Debug.Log("Check recorded rule set");
                    isBagPush = item.ruleInfoDict.GetOrDefault(8,null).attributeList.Contains(4);
                    isHotMelt = item.ruleInfoDict.GetOrDefault(8,null).attributeList.Contains(8);
                    isPumpkinPush = item.ruleInfoDict.GetOrDefault(4,null).attributeList.Contains(4);
                }
            }

            if (isBagPush){
                Debug.Log("Bag is Push");
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(6,8), Direction.Up));
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(6,7), Direction.Up));
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(7,7), Direction.Up));
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(8,7), Direction.Up));
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(8,8), Direction.Up));
            }

            if (isHotMelt){
                Debug.Log("Bag is Melt");
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(5,6), Direction.Up));
            }

            if (isPumpkinPush){
                Debug.Log("Bag is Melt");
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(0,16), Direction.Up));
            }

        }

    }


}

