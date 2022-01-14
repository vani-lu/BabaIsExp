using UnityEngine;
using UnityEngine.UI;

namespace Gfen.Game.UI
{
    public class GamePlayPage : UIPage
    {
        public Button pauseButton;

        public Button hintButton;

        public GameObject hintPanel;

        public GameObject hintNote;

        public Text hintText;

        public Text countDown;

        public GameObject joystickGameObject;

        public GameObject waitGameObject;

        public GameObject undoGameObject;

        public GameObject redoGameObject;

        private string m_levelHintText;

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

        private void LateUpdate(){
            if (gameObject.activeSelf) {
                // If in gameplay
                if (!hintButton.gameObject.activeSelf){
                    if (m_gameManager.LevelManager.IsCurrentLevelTimeUp()){
                        hintButton.gameObject.SetActive(true);
                        hintNote.SetActive(true);
                        hintText.text = m_levelHintText;
                    }
                }
            }
        }

        private void SetHintContent(){

            //Hint Content
            int chapterIndex = m_gameManager.CurrentChapterIndex;
            int levelIndex = m_gameManager.CurrentLevelIndex;
            var levelConfig = m_gameManager.gameConfig.chapterConfigs[chapterIndex].levelConfigs[levelIndex];
            m_levelHintText = levelConfig.hintText;

            //Size
            RectTransform rt = hintPanel.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(300, 80);
        }

        private void UpdateCountdownTimer(){
            
        }

        private void OnPauseButtonClicked()
        {
            m_gameManager.PauseGame();
        }

        private void OnHintButtonClicked()
        {
            hintPanel.SetActive(!hintPanel.activeSelf);
            hintNote.SetActive(!hintNote.activeSelf);
        }
    }

}
