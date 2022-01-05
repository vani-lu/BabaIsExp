using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gfen.Game.UI
{
    public class LevelPage : UIPage
    {
        public Button backButton;

        public Button quitButton;

        public Text chapterNameText;
        
        public Transform levelListRootTransform;

        public LevelCell templateLevelCell;

        public GameObject bonusIntro;

        public GameObject immatureQuit;

        private List<LevelCell> m_levelCells = new List<LevelCell>();

        private int m_currentChapterIndex;

        private void Awake() 
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnEnable()
        {
            if (m_currentChapterIndex == 2 && m_gameManager.LevelManager.IsLevelPassed(2, 2)){
                m_gameManager.QuitGame();
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

            // Toggle text discription
            if (m_currentChapterIndex == 2){
                bonusIntro.SetActive(true);
                if (m_gameManager.LevelManager.IsLevelPassed(m_currentChapterIndex, 0)){
                    immatureQuit.SetActive(true);
                }
            }
            else {
                 bonusIntro.SetActive(false);
                 immatureQuit.SetActive(false);
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
