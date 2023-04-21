using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace Gfen.Game.UI
{
    public class GameSuccessPage : UIPage
    {
        public Button okButton;

        private void Awake() 
        {
            okButton.onClick.AddListener(OnOkButtonClicked);
        }

        private void Update(){
            if (CrossPlatformInputManager.GetButton("Submit")){
                OnOkButtonClicked();
            }
        }
        
        private void OnOkButtonClicked()
        {
            // on the last level of the last chapter
            if (m_gameManager.CurrentChapterIndex == m_gameManager.gameConfig.chapterConfigs.Length - 1 &&
                m_gameManager.CurrentLevelIndex == m_gameManager.gameConfig.chapterConfigs[m_gameManager.gameConfig.chapterConfigs.Length - 1].levelConfigs.Length - 1)
            {
                m_gameManager.ExitApp();
            }

            m_gameManager.uiManager.HidePage();
            m_gameManager.StopGame();
        }
    }
}
