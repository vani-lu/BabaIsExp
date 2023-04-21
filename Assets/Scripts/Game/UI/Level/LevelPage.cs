using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gfen.Game.UI
{
    public class LevelPage : UIPage
    {
        public Button backButton;

        public Text chapterNameText;

        public GameObject tutorialIntro;

        public GameObject trainingIntro;
        
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
            tutorialIntro.SetActive(false);
            trainingIntro.SetActive(false);
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

            // Toggle text discription
            if (m_currentChapterIndex == 0){
                tutorialIntro.SetActive(true);
            }
            else if (m_currentChapterIndex == 1){
                trainingIntro.SetActive(true);
            }
            
        }

        private void OnBackButtonClicked()
        {
            m_gameManager.uiManager.HidePage();
        }
    }
}
