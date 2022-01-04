using UnityEngine;
using UnityEngine.UI;

namespace Gfen.Game.UI
{
    public class GamePlayPage : UIPage
    {
        public Button pauseButton;

        public Button quitButton;

        public GameObject quitGameObject;

        public GameObject joystickGameObject;

        public GameObject waitGameObject;

        public GameObject undoGameObject;

        public GameObject redoGameObject;

        private void Awake()
        {
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnEnable() 
        {
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
            if (m_gameManager.CurrentChapterIndex == 2 && m_gameManager.CurrentLevelIndex > 0){
                quitGameObject.SetActive(true);
            }
            else {
                quitGameObject.SetActive(false);
            }
        }

        private void OnPauseButtonClicked()
        {
            m_gameManager.PauseGame();
        }

        public void OnQuitButtonClicked()
        {
            m_gameManager.PauseGame();
            m_gameManager.uiManager.HidePage();
            m_gameManager.uiManager.ShowPage<InGameQuitPage>();
        }
    }
}
