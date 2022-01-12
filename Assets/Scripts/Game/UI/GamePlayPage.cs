using UnityEngine;
using UnityEngine.UI;

namespace Gfen.Game.UI
{
    public class GamePlayPage : UIPage
    {
        public Button pauseButton;

        public GameObject hintGameObject;

        public GameObject joystickGameObject;

        public GameObject waitGameObject;

        public GameObject undoGameObject;

        public GameObject redoGameObject;

        private void Awake()
        {
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }

        private void OnEnable() 
        {
            hintGameObject.SetActive(false);
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
        }

        private void LateUpdate(){
            if (gameObject.activeSelf) {
                if (!hintGameObject.activeSelf){
                    if (m_gameManager.LevelManager.IsCurrentLevelTimeUp()){
                        hintGameObject.SetActive(true);
                    }
                }
            }
        }

        private void OnPauseButtonClicked()
        {
            m_gameManager.PauseGame();
        }
    }

}
