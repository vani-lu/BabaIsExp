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

        public void HandleBonusMap(){

            int chapterIndex = m_gameManager.CurrentChapterIndex;
            int levelIndex = m_gameManager.CurrentLevelIndex;

            int bonusChapterIndex = m_gameManager.bonusChapterIndex;

            // Skip other chapters
            if (chapterIndex != bonusChapterIndex) {
                return;
            }
            if (levelIndex == 0) {
                return;
            }

            bool isBagPush = false; // Bag Is Push
            bool isBagHotMelt = false; // Bag Is Melt
            bool isPumpkinPush = false; // Pumpkin Is Push

            foreach (SolutionData item in solutionInfo.sList){
                if (item.chapterIndex == bonusChapterIndex && item.levelIndex < 2){
                    // Check recorded rule set;
                    // If the solution indicator is true, it remains to be true
                    isBagPush = isBagPush || item.ruleInfoDict.GetOrSet(8,()=> new AttributeSet()).attributeList.Contains(4);
                    isBagHotMelt = isBagHotMelt || item.ruleInfoDict.GetOrSet(8,()=> new AttributeSet()).attributeList.Contains(8);
                    isPumpkinPush = isPumpkinPush || item.ruleInfoDict.GetOrSet(4,()=> new AttributeSet()).attributeList.Contains(4);
                }
            }

            if (isBagPush && isBagHotMelt){
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(11,7), Direction.Up));
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(11,6), Direction.Up));
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(12,6), Direction.Up));
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(13,6), Direction.Up));
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(13,7), Direction.Up));
                return;
            }

            if (isBagPush && isPumpkinPush){
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(6,9), Direction.Up));
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(6,8), Direction.Up));
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(7,8), Direction.Up));
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(8,8), Direction.Up));
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(8,9), Direction.Up));
                return;
            }

            if (isBagPush){
                int xStartPoint = 17;
                for (var i = xStartPoint; i < m_logicGameManager.Map.GetLength(0); i++)
                {
                    for (var j = 0; j < m_logicGameManager.Map.GetLength(1); j++)
                    {
                        var mapBlockList = m_logicGameManager.Map[i, j];
                        if (mapBlockList.Count == 0){
                            m_logicGameManager.AddBlock(new Block(8, new Vector2Int(i,j), Direction.Up));
                        }
                    }
                }
            }

            if (isBagHotMelt){
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(10,6), Direction.Up));
            }

            if (isPumpkinPush){
                m_logicGameManager.AddBlock(new Block(4, new Vector2Int(1,10), Direction.Up));
            }

        }

    }


}

