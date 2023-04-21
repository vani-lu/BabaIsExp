using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Vani.UI
{
    public class MenuManager : MonoBehaviour
    {
        public Button playButton;

        public InputField nameInputField;

        private string m_dataPath;

        private string m_userName;

        private string m_loginDate;

        private const string PathInfoKey = "DataPath";

        private const string UserInfoKey = "UserName";

        private const string DateInfoKey = "LoginDate";

        // Start is called before the first frame update
        void Start()
        {
            m_loginDate = DateTime.Now.ToString("yyyyMMddHHmm");
            // m_dataPath = Application.persistentDataPath;
            m_dataPath = "./Save";
            if (!Directory.Exists(m_dataPath)){
                 Directory.CreateDirectory(m_dataPath);
            }
        }

        public void PlayGame()
        {
            // Get user (participant's) name from input
            m_userName = nameInputField.text;

            // Check if user name is empty and longer than 1 character
            if (!string.IsNullOrEmpty(m_userName) && m_userName.Length > 1){
                SaveUserInfo();
                SceneManager.LoadScene("Main");
            }
        }

        private void SaveUserInfo()
        {
                PlayerPrefs.SetString(UserInfoKey, m_userName);
                PlayerPrefs.SetString(DateInfoKey, m_loginDate);
                PlayerPrefs.SetString(PathInfoKey, m_dataPath);
                UpdatePlayerDatabase();
            
        }

        private void UpdatePlayerDatabase(){
            using StreamWriter file = new StreamWriter(m_dataPath + "/participants.csv", append: true);
            file.WriteLine(string.Format("{0},{1}", m_loginDate, m_userName));
        }
    }
}
