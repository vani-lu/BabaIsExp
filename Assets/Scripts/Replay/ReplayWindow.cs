using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Gfen.Game
{
    public class ReplayWindow : EditorWindow 
    {
        private const string UserInfoKey = "UserName";

        private const string DateInfoKey = "LoginDate";

        private string m_user;

        private string m_date;

        private int m_condition;

        private string m_scenePath;

        private ReplayManager m_replayManager;


        [MenuItem("babaisyou/ReplayWindow")]
        private static void ShowWindow() 
        {
            var window = GetWindow<ReplayWindow>();
            window.titleContent = new GUIContent("Replay");

            if (PlayerPrefs.HasKey(UserInfoKey))
            {
                window.m_user = PlayerPrefs.GetString(UserInfoKey, "");
            }
            if (PlayerPrefs.HasKey(DateInfoKey))
            {
                window.m_date = PlayerPrefs.GetString(DateInfoKey, "");
            }

            window.Show();
        }
    
        private void OnGUI() 
        {
            GUILayout.BeginHorizontal();
            m_date = EditorGUILayout.TextField(m_date, GUILayout.Width(100));
            m_user = EditorGUILayout.TextField(m_user, GUILayout.Width(50));
            m_condition = EditorGUILayout.IntField(m_condition, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("OK", GUILayout.Width(100)))
            {
                PlayerPrefs.SetString(UserInfoKey, m_user);
                PlayerPrefs.SetString(DateInfoKey, m_date);
                switch(m_condition){
                    case 1:
                        m_scenePath = "Assets/Scenes/Main_NN.unity";
                        break;
                    case 2:
                        m_scenePath = "Assets/Scenes/Main_NF.unity";
                        break;
                    case 3:
                        m_scenePath = "Assets/Scenes/Main_FN.unity";
                        break;
                    case 4:
                        m_scenePath = "Assets/Scenes/Main_FF.unity";
                        break;
                    default:
                        ShowTip("请输入1-4");
                        break;
                }
            }

            if (GUILayout.Button("Start", GUILayout.Width(100)))
            {
                EditorSceneManager.OpenScene(m_scenePath);
                EditorApplication.isPlaying = true;
            }

            GUILayout.EndHorizontal();

        }

        private void ShowTip(string tip)
        {
            ShowNotification(new GUIContent(tip));
        }

    }
}
