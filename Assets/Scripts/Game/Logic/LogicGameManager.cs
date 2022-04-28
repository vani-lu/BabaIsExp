using System;
using System.Collections.Generic;
using System.IO;
using Gfen.Game.Config;
using UnityEngine;

namespace Gfen.Game.Logic
{
    public class LogicGameManager
    {
        public Action<bool> GameEnd;

        private GameManager m_gameManager;

        public GameManager GameManager { get { return m_gameManager; } }

        private RuleAnalyzer m_ruleAnalyzer;

        private AttirbuteHandler m_attributeHandler;

        public AttirbuteHandler AttributeHandler { get { return m_attributeHandler; } }

        private string m_mapPath;

        private MapConfig m_mapConfig;

        private List<Block>[,] m_map;

        public List<Block>[,] Map { get { return m_map; } }

        private Stack<Stack<Command>> m_tickCommandsStack = new Stack<Stack<Command>>();

        private Stack<Stack<Command>> m_redoCommandsStack = new Stack<Stack<Command>>();

        public LogicGameManager(GameManager gameManager)
        {
            m_gameManager = gameManager;

            m_ruleAnalyzer = new RuleAnalyzer(this);
            m_attributeHandler = new AttirbuteHandler(this);

            if (!Directory.Exists("./Exports")){
                 Directory.CreateDirectory("./Exports");
            }

            m_mapPath = "./Exports/" + "map_" + m_gameManager.Date + "_" + m_gameManager.User + ".json";

            if (!File.Exists(m_mapPath)){
                File.Create(m_mapPath).Dispose();
            }

        }
    
        public void StartGame(MapConfig mapConfig)
        {
            m_mapConfig = mapConfig;

            StartGameCore();
        }

        public void StopGame()
        {
            StopGameCore();
        }

        public void RestartGame()
        {
            StopGameCore();
            StartGameCore();
        }

        private void StartGameCore()
        {
            m_map = new List<Block>[m_mapConfig.size.x, m_mapConfig.size.y];
            for (var i = 0; i < m_mapConfig.size.x; i++)
            {
                for (var j = 0; j < m_mapConfig.size.y; j++)
                {
                    m_map[i, j] = new List<Block>();
                }
            }

            foreach (MapBlock mapBlockConfig in m_mapConfig.blocks)
            {
                AddBlock(new Block(mapBlockConfig.entityType, mapBlockConfig.position, mapBlockConfig.direction));
            }

            m_gameManager.SolutionDataManager.HandleBonusMap();
            m_attributeHandler.RefreshAttributes();
            m_ruleAnalyzer.Apply(null);

        }

        private void StopGameCore()
        {
            m_attributeHandler.Clear();

            m_tickCommandsStack.Clear();
            m_redoCommandsStack.Clear();

            m_ruleAnalyzer.Clear();
        }

        public int Undo()
        {
            if (m_tickCommandsStack.Count <= 0)
            {
                return 0;
            }

            var redoCommands = new Stack<Command>();

            var lastTickCommands = m_tickCommandsStack.Pop();

            int numCommands = lastTickCommands.Count;

            while ( lastTickCommands.Count > 0)
            {
                var command = lastTickCommands.Pop();
                command.Undo();

                redoCommands.Push(command);
            }

            m_redoCommandsStack.Push(redoCommands);
            m_attributeHandler.RefreshAttributes();
            m_ruleAnalyzer.Refresh();

            return numCommands;
        }

        public int Redo()
        {
            if (m_redoCommandsStack.Count <= 0)
            {
                return 0;
            }

            var tickCommands = new Stack<Command>();

            var redoCommands = m_redoCommandsStack.Pop();

            int numCommands = redoCommands.Count;

            while ( redoCommands.Count > 0)
            {
                var command = redoCommands.Pop();
                command.Perform();

                tickCommands.Push(command);
            }

            m_tickCommandsStack.Push(tickCommands);

            m_attributeHandler.RefreshAttributes();

            m_ruleAnalyzer.Refresh();

            return numCommands;
        }

        public int Tick(OperationType operationType)
        {
            m_redoCommandsStack.Clear();

            var tickCommands = new Stack<Command>();

            m_attributeHandler.HandleAttributeYou(operationType, tickCommands);// Also handles Push and Pull, Open and Shut
            m_attributeHandler.HandleAttributeMove(tickCommands);

            m_attributeHandler.RefreshAttributes();
            m_ruleAnalyzer.Refresh();

            m_attributeHandler.HandleAttributeDefeat(tickCommands);
            m_attributeHandler.HandleAttributeSink(tickCommands);
            m_attributeHandler.HandleAttributeHotAndMelt(tickCommands);

            m_attributeHandler.RefreshAttributes();
            m_ruleAnalyzer.Apply(tickCommands);

            int numCommands = tickCommands.Count;

            if (tickCommands.Count > 0)
            {
                m_tickCommandsStack.Push(tickCommands);
            }

            CheckGameResult();

            return numCommands;
        }

        private async void CheckGameResult()
        {
            var gameResult = GetGameResult();
            if (gameResult == GameResult.Success)
            {
                var saveTask = m_gameManager.SolutionDataManager.SaveSuccessRuleset();

                GameEnd?.Invoke(true);
                await saveTask;
            }
            else if (gameResult == GameResult.Defeat)
            {
                GameEnd?.Invoke(false);
            }
        }

        private GameResult GetGameResult()
        {
            // Check Success
            if (ForeachMapPosition(position =>
            {
                if (HasAttribute(position, AttributeCategory.You))
                {
                    return HasAttribute(position, AttributeCategory.Win);
                }
                return false;
            })){
                return GameResult.Success;
            }
            

            // Check Defeat
            bool youExist = ForeachMapPosition(position =>
            {
                return HasAttribute(position, AttributeCategory.You);
            });
            if (!youExist){
                return GameResult.Defeat;
            }

            // If none of the conditions apply
            return GameResult.Uncertain;
            
        }

        public bool ForeachMapPosition(Func<Vector2Int, bool> positionHandler)
        {
            var mapXLength = m_map.GetLength(0);
            var mapYLength = m_map.GetLength(1);

            for (var i = 0; i < mapXLength; i++)
            {
                for (var j = 0; j < mapYLength; j++)
                {
                    if (positionHandler(new Vector2Int(i, j)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool InMap(Vector2Int position)
        {
            return position.x >= 0 && position.x < m_map.GetLength(0) && position.y >= 0 && position.y < m_map.GetLength(1);
        }

        public void MoveBlock(Block block, Vector2Int position)
        {
            RemoveBlock(block);
            block.position = position;
            AddBlock(block);
        }

        public void RemoveBlock(Block block)
        {
            var mapBlockList = m_map[block.position.x, block.position.y];
            var targetIndex = mapBlockList.IndexOf(block);
            if (targetIndex >= 0)
            {
                mapBlockList.RemoveAt(targetIndex);
            }
        }

        public void AddBlock(Block block)
        {
            var mapBlockList = m_map[block.position.x, block.position.y];
            mapBlockList.Add(block);
        }

        public void ConverseBlockEntityType(Block block, int targetEntityType)
        {
            block.entityType = targetEntityType;
        }

        public bool HasAttribute(Vector2Int position, AttributeCategory attributeCategory)
        {
            var blocks = m_map[position.x, position.y];
            foreach (var block in blocks)
            {
                if (m_attributeHandler.HasAttribute(block, attributeCategory))
                {
                    return true;
                }
            }

            return false;
        }

        public async void BlockListMap2BlockList()
        {
            var blockList = new BlockListWrapper(){
                size = new Vector2Int(0, 0),
                blocks = new List<Block>()
            };

            if (m_map != null)
            {
                var mapXLength = m_map.GetLength(0);
                var mapYLength = m_map.GetLength(1);
                blockList.size = new Vector2Int(mapXLength, mapYLength);

                for (var i = 0; i < mapXLength; i++)
                {
                    for (var j = 0; j < mapYLength; j++)
                    {
                        var mapblockList = m_map[i, j];
                        if (mapblockList.Count > 0)
                        {
                            blockList.blocks.AddRange(mapblockList);
                        }
                    }
                }
            }

            var saveTask = blockList.Save(m_mapPath);
            await saveTask;
            
        }

    }
}
