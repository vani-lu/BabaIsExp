using UnityEngine;
using UnityEngine.UI;
using Vani.Data;

namespace Gfen.Game.UI
{
    public class GamePlayPage : UIPage
    {
        public Button pauseButton;

        public Button hintButton;

        public GameObject hintPanel;

        public GameObject hintNote;

        public Text hintText;

        public GameObject joystickGameObject;

        public GameObject waitGameObject;

        public GameObject undoGameObject;

        public GameObject redoGameObject;

        private int m_chapterIndex;

        private int m_levelIndex;

        private void Awake()
        {
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
            hintButton.onClick.AddListener(OnHintButtonClicked);
        }

        private void OnEnable() 
        {
            hintButton.gameObject.SetActive(false);
            
            hintPanel.SetActive(false);
            hintNote.SetActive(false);

#if MOBILE_INPUT
		    joystickGameObject.SetActive(true);
            waitGameObject.SetActive(true);
            undoGameObject.SetActive(true);
            redoGameObject.SetActive(true);
#else
            joystickGameObject.SetActive(false);
            waitGameObject.SetActive(false);
            undoGameObject.SetActive(false);
            redoGameObject.SetActive(false);
#endif

            SetHintContent();
        }

        private async void LateUpdate(){
            if (gameObject.activeSelf) {
                // in gameplay

                // show hint button if reach time limit
                if (!hintButton.gameObject.activeSelf){
                    if (m_gameManager.LevelManager.IsCurrentLevelTimeUp()){
                        var writeTask = FrameDataUtility.MarkEnableHint(m_gameManager.DataPath + m_gameManager.DataFile, m_chapterIndex, m_levelIndex);
                        hintButton.gameObject.SetActive(true);
                        hintNote.SetActive(true);
                        await writeTask;
                    }
                }
            }
        }

        private void SetHintContent(){

            //Hint Content
            m_chapterIndex = m_gameManager.CurrentChapterIndex;
            m_levelIndex = m_gameManager.CurrentLevelIndex;
            var levelConfig = m_gameManager.gameConfig.chapterConfigs[m_chapterIndex].levelConfigs[m_levelIndex];
            hintText.text = levelConfig.hintText;

            //Size
            RectTransform rt = hintPanel.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, 80);
        }

        private void OnPauseButtonClicked()
        {
            m_gameManager.PauseGame();
        }

        private async void OnHintButtonClicked()
        {
            var writeTask = FrameDataUtility.MarkToggleHint(m_gameManager.DataPath + m_gameManager.DataFile, m_chapterIndex, m_levelIndex);

            hintPanel.SetActive(!hintPanel.activeSelf);
            hintNote.SetActive(!hintNote.activeSelf);

            await writeTask;
        }
    }

}
