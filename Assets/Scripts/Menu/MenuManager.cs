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

        public InputField conditionInputField;

        private string m_dataPath;

        private string m_userName;

        private string m_loginDate;

        private int m_conditionIndex;

        private const string PathInfoKey = "DataPath";

        private const string UserInfoKey = "UserName";

        private const string DateInfoKey = "LoginDate";

        private const string ConditionInfoKey = "Condition";

        // Start is called before the first frame update
        void Start()
        {
            m_loginDate = DateTime.Now.ToString("yyyyMMdd");
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
            string str = conditionInputField.text;
            bool convertible = Int32.TryParse(str, out m_conditionIndex);

            if (!string.IsNullOrEmpty(m_userName) && convertible){
                SaveUserInfo();
                SceneManager.LoadScene(m_conditionIndex);
            }
        }

        private void SaveUserInfo()
        {
            string lastUserName = PlayerPrefs.GetString(UserInfoKey);
            string lastLoginDate = PlayerPrefs.GetString(DateInfoKey);
            if (lastUserName == m_userName && lastLoginDate == m_loginDate){
                // Continuing last session?
                m_conditionIndex = PlayerPrefs.GetInt(ConditionInfoKey);
            }
            else {
                // A new user?
                PlayerPrefs.SetString(UserInfoKey, m_userName);
                PlayerPrefs.SetString(DateInfoKey, m_loginDate);
                PlayerPrefs.SetString(PathInfoKey, m_dataPath);
                PlayerPrefs.SetInt(ConditionInfoKey, m_conditionIndex);
                // SetAndSaveCondition();
                UpdatePlayerDatabase();
            }
            
        }

        private void SetAndSaveCondition()
        {
            System.Random rnd = new System.Random();
            m_conditionIndex = rnd.Next(1,5);
            PlayerPrefs.SetInt(ConditionInfoKey, m_conditionIndex);
        }

        private void UpdatePlayerDatabase(){
            using StreamWriter file = new StreamWriter(m_dataPath + "/participants.csv", append: true);
            file.WriteLine(string.Format("{0},{1},{2:d}", m_loginDate, m_userName, m_conditionIndex));
        }
    }
}
