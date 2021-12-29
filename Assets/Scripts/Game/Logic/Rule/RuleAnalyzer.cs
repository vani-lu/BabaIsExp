using System.Collections.Generic;
using UnityEngine;

namespace Gfen.Game.Logic
{
    public class RuleAnalyzer
    {
        public LogicGameManager m_logicGameManager;

        /*  Rule structure: SUBJECT IS COMPLEMENT
            Legitimate subject words: entity type OR entity category
            Legitimate predicative complements: attribute words */
        private readonly RuleCategory[] m_subjectCategories = new RuleCategory[] { RuleCategory.EntityType, RuleCategory.EntityCategory };
        private readonly RuleCategory[] m_complementCategories = new RuleCategory[] { RuleCategory.Attribute };

        private List<Rule> m_currentRules = new List<Rule>();

        private List<Rule> m_cachedRules = new List<Rule>();

        private List<Block> m_cachedSubjectRuleBlocks = new List<Block>();

        private List<Block> m_cachedComplementRuleBlocks = new List<Block>();

        private List<Block> m_cachedIsKeyWordRuleBlocks = new List<Block>();

        public RuleAnalyzer(LogicGameManager logicGameManager)
        {
            m_logicGameManager = logicGameManager;
        }

        public void Refresh()
        {
            RefreshCurrentRules();

            foreach (var rule in m_currentRules)
            {
                rule.ApplyPersistent();
            }
        }

        public void Apply(Stack<Command> tickCommands)
        {
            RefreshCurrentRules();

            foreach (var rule in m_currentRules)
            {
                rule.ApplyPersistent();
                rule.ApplyAction(tickCommands);
            }
        }

        public void Clear()
        {
            m_currentRules.Clear();
        }

        private void FindIsKeyWordRuleBlocks(List<Block> isBlocks)
        {
            var map = m_logicGameManager.Map;

            var mapXLength = map.GetLength(0);
            var mapYLength = map.GetLength(1);

            for (var i = 0; i < mapXLength; i++)
            {
                for (var j = 0; j < mapYLength; j++)
                {
                    foreach (var block in map[i, j])
                    {
                        if (IsTargetKeyWordRuleBlock(block, KeyWordCategory.Is))
                        {
                            isBlocks.Add(block);
                        }
                    }
                }
            }
        }

        private bool IsTargetKeyWordRuleBlock(Block block, KeyWordCategory keyWordCategory)
        {
            var gameConfig = m_logicGameManager.GameManager.gameConfig;

            var entityConfig = gameConfig.GetEntityConfig(block.entityType);
            if (entityConfig.category != EntityCategory.Rule)
            {
                return false;
            }

            if (entityConfig.ruleCategory != RuleCategory.KeyWord)
            {
                return false;
            }

            if (entityConfig.keyWordCategoryForRule != keyWordCategory)
            {
                return false;
            }

            return true;
        }

        private bool IsTargetRuleBlock(Block block, RuleCategory ruleCategory)
        {
            var gameConfig = m_logicGameManager.GameManager.gameConfig;

            var entityConfig = gameConfig.GetEntityConfig(block.entityType);
            if (entityConfig.category != EntityCategory.Rule)
            {
                return false;
            }

            if (entityConfig.ruleCategory != ruleCategory)
            {
                return false;
            }

            return true;
        }

        private void RefreshCurrentRules()
        {
            FindIsKeyWordRuleBlocks(m_cachedIsKeyWordRuleBlocks);

            FindDirectionRules(Vector2Int.right, m_cachedIsKeyWordRuleBlocks, m_cachedRules);
            FindDirectionRules(Vector2Int.down, m_cachedIsKeyWordRuleBlocks, m_cachedRules);

            m_currentRules.Clear();
            m_currentRules.AddRange(m_cachedRules);

            m_cachedIsKeyWordRuleBlocks.Clear();
            m_cachedRules.Clear();
        }

        private void FindDirectionRules(Vector2Int direction, List<Block> isKeyWordRuleBlocks, List<Rule> resultRules)
        {
            var configSet = m_logicGameManager.GameManager.gameConfig;

            foreach (var isKeyWordRuleBlock in isKeyWordRuleBlocks) // Is: Linking verb
            {
                // Backward: Subject
                FindAdjacentRuleBlocks(isKeyWordRuleBlock.position, Vector2Int.zero - direction, m_subjectCategories, m_cachedSubjectRuleBlocks);
                // Forward: Predicative complement
                FindAdjacentRuleBlocks(isKeyWordRuleBlock.position, direction, m_complementCategories, m_cachedComplementRuleBlocks);

                foreach (var subjectRuleBlock in m_cachedSubjectRuleBlocks)
                {
                    var subjectEntityConfig = configSet.GetEntityConfig(subjectRuleBlock.entityType);

                    foreach (var complementRuleBlock in m_cachedComplementRuleBlocks)
                    {
                        var complementEntityConfig = configSet.GetEntityConfig(complementRuleBlock.entityType);

                        if (subjectEntityConfig.ruleCategory == RuleCategory.EntityType)
                        {
                            if (complementEntityConfig.ruleCategory == RuleCategory.Attribute)
                            {
                                resultRules.Add(new EntityTypeIsAttributeRule(m_logicGameManager, subjectEntityConfig.entityTypeForRule, complementEntityConfig.attributeCategoryForRule));
                            }
                        }
                        else if (subjectEntityConfig.ruleCategory == RuleCategory.EntityCategory)
                        {
                            if (complementEntityConfig.ruleCategory == RuleCategory.Attribute)
                            {
                                resultRules.Add(new EntityCategoryIsAttributeRule(m_logicGameManager, subjectEntityConfig.entityCategoryForRule, complementEntityConfig.attributeCategoryForRule));
                            }
                        }
                    }
                }

                m_cachedSubjectRuleBlocks.Clear();
                m_cachedComplementRuleBlocks.Clear();
            }
        }

        private void FindAdjacentRuleBlocks(Vector2Int originPosition, Vector2Int direction, RuleCategory[] targetRuleCategories, List<Block> resultBlocks)
        {
            var position = originPosition + direction;
            
            while (m_logicGameManager.InMap(position))
            {
                var mapBlocks = m_logicGameManager.Map[position.x, position.y];

                // Enable this assignment if loop for AND keyword
                //var hasTargetRuleBlock = false;
                foreach (var block in mapBlocks)
                {
                    var hasTargetRuleCategory = false;
                    foreach (var targetRuleCategory in targetRuleCategories)
                    {
                        if (IsTargetRuleBlock(block, targetRuleCategory))
                        {
                            hasTargetRuleCategory = true;
                            break;
                        }
                    }
                    if (hasTargetRuleCategory)
                    {
                        //hasTargetRuleBlock = true;
                        resultBlocks.Add(block);
                    }
                }

                break; // stop the nearest position, delete this break if enable AND keyword

                { // Loop for AND
            
                // if (!hasTargetRuleBlock)
                // {
                //     break;
                // }

                // position += direction;

                // if (!m_logicGameManager.InMap(position))
                // {
                //     break;
                // }


                // mapBlocks = m_logicGameManager.Map[position.x, position.y];

                // var hasAndKeyWordRuleBlock = false;
                // foreach (var block in mapBlocks)
                // {
                //     if (IsTargetKeyWordRuleBlock(block, KeyWordCategory.And))
                //     {
                //         hasAndKeyWordRuleBlock = true;
                //     }
                // }

                // if (!hasAndKeyWordRuleBlock)
                // {
                //     break;
                // }

                // position += direction;
                }
        
            }
        }
    }
}
