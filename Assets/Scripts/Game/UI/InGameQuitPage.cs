using UnityEngine.UI;
using UnityEngine;

namespace Gfen.Game.UI
{
    public class InGameQuitPage : UIPage
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
            m_gameManager.ResumeGame();
        }

        private void OnQuitGameButtonClicked()
        {
            m_gameManager.ExitGame();
        }
        
    }
}
