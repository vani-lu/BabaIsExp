using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace Gfen.Game.UI
{
    public class QuitConfirmPage : UIPage
    {
        public Button confirmQuitButton;

        private void Awake() 
        {
            confirmQuitButton.onClick.AddListener(OnQuitGameButtonClicked);
        }

        private void Update(){
            if (CrossPlatformInputManager.GetButton("Submit")){
                OnQuitGameButtonClicked();
            }
        }

        private void OnQuitGameButtonClicked()
        {
            m_gameManager.ExitApp();
        }
        
    }
}
