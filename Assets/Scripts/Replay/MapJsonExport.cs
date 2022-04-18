using UnityEngine;
using UnityEditor;
using Gfen.Game.Config;

namespace Gfen.Game.Map
{
    public class MapJsonExportWindow : EditorWindow 
    {
        private const string exportDirectoryKey = "ExportDirectory";

        private const string configPathKey = "ConfigPath";

        private string m_exportDirectory;

        private string m_configPath;

        private int m_chapter;

        private int m_level;

        private bool m_isParsable;

        private Config.MapConfig m_mapconfig;

        private GameConfig m_gameconfig;

        [MenuItem("babaisyou/MapJsonExportWindow")]
        private static void ShowWindow() 
        {
            var window = GetWindow<MapJsonExportWindow>();
            window.titleContent = new GUIContent("MapJsonExport");

            if (EditorPrefs.HasKey(exportDirectoryKey))
            {
                window.m_exportDirectory = EditorPrefs.GetString(exportDirectoryKey, "");
            }
            if (EditorPrefs.HasKey(configPathKey))
            {
                window.m_configPath = EditorPrefs.GetString(configPathKey, "");
                window.m_gameconfig = AssetDatabase.LoadAssetAtPath<GameConfig>(window.m_configPath);
            }

            window.Show();
        }
    
        private void OnGUI() 
        {
            GUILayout.BeginHorizontal();
            m_exportDirectory = EditorGUILayout.TextField(m_exportDirectory);
            if (GUILayout.Button("Export Directory", GUILayout.Width(150)))
            {
                EditorPrefs.SetString(exportDirectoryKey, m_exportDirectory);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            GUILayout.BeginHorizontal();
            m_configPath = EditorGUILayout.TextField(m_configPath);
            if (GUILayout.Button("GameConfig File Path", GUILayout.Width(150)))
            {
                m_gameconfig = AssetDatabase.LoadAssetAtPath<GameConfig>(m_configPath);
                EditorPrefs.SetString(configPathKey, m_configPath);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            GUILayout.BeginHorizontal();

            m_isParsable = false;
            m_isParsable = int.TryParse(EditorGUILayout.TextField(m_chapter.ToString()), out m_chapter);
            m_isParsable = m_isParsable && int.TryParse(EditorGUILayout.TextField(m_level.ToString()), out m_level);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Export Json", GUILayout.Width(150)))
            {
                EditorPrefs.SetString(configPathKey, m_configPath);
                m_mapconfig = m_gameconfig.chapterConfigs[m_chapter].levelConfigs[m_level].map;
                ExportMap();
            }

            GUI.enabled = true;

        }


        private void ExportMap()
        {
            Debug.Log(JsonUtility.ToJson(m_mapconfig));
        }

        // 错误处理
        private void ShowTip(string tip)
        {
            ShowNotification(new GUIContent(tip));
        }
    }
}

