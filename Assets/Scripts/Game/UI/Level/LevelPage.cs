using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gfen.Game.UI
{
    public class LevelPage : UIPage
    {
        public Button backButton;

        public Text chapterNameText;
        
        public Transform levelListRootTransform;

        public LevelCell templateLevelCell;

        private List<LevelCell> m_levelCells = new List<LevelCell>();

        private int m_currentChapterIndex;

        private void Awake() 
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        private void OnEnable()
        {
            if (m_currentChapterIndex == m_gameManager.bonusChapterIndex && m_gameManager.LevelManager.IsChapterPassed(m_gameManager.bonusChapterIndex)){
                m_gameManager.ExitGame();
            }
        }

        public void SetContent(int chapterIndex)
        {
            m_currentChapterIndex = chapterIndex;

            m_gameManager.LevelManager.SetStayChapterIndex(m_currentChapterIndex);

            var chapterConfig = m_gameManager.gameConfig.chapterConfigs[m_currentChapterIndex];

            chapterNameText.text = chapterConfig.chapterName;

            var levelConfigs = chapterConfig.levelConfigs;
            while (m_levelCells.Count < levelConfigs.Length)
            {
                var levelCell = UIUtils.InstantiateUICell(levelListRootTransform, templateLevelCell);
                m_levelCells.Add(levelCell);
            }
            for (var i = 0; i < levelConfigs.Length; i++)
            {
                m_levelCells[i].Show(m_gameManager);
                m_levelCells[i].SetContent(m_currentChapterIndex, i);
            }
            // Hide excess instantiations of cells
            for (var i = levelConfigs.Length; i < m_levelCells.Count; i++)
            {
                m_levelCells[i].Hide();
            }
        }

        private void OnBackButtonClicked()
        {
            m_gameManager.uiManager.HidePage();
        }

        public void OnQuitButtonClicked()
        {
            m_gameManager.uiManager.ShowPage<QuitConfirmPage>();
        }
    }
}
