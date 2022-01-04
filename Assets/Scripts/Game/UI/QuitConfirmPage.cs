using UnityEngine.UI;
using UnityEngine;

namespace Gfen.Game.UI
{
    public class QuitConfirmPage : UIPage
    {
        public Button closeButton;

        public Button confirmQuitButton;

        private void Awake() 
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            confirmQuitButton.onClick.AddListener(OnQuitGameButtonClicked);
        }
        
        private void OnCloseButtonClicked()
        {
            m_gameManager.uiManager.HidePage();
        }

        private void OnQuitGameButtonClicked()
        {
            m_gameManager.QuitGame();
        }
        
    }
}
