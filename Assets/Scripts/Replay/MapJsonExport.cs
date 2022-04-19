using System.IO;
using System.Threading.Tasks;
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
                if (!Directory.Exists(m_exportDirectory)){
                    Directory.CreateDirectory(m_exportDirectory);
                }
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

            if (m_isParsable){
                GUI.enabled = true;
            }
            else {
                ShowTip("请输入正整数");
            }

            if (GUILayout.Button("Export Json", GUILayout.Width(150)))
            {
                EditorPrefs.SetString(configPathKey, m_configPath);
                m_mapconfig = m_gameconfig.chapterConfigs[m_chapter-1].levelConfigs[m_level-1].map;
                ExportMap();
            }


        }

        private void ExportMap()
        {
            // Debug.Log(JsonUtility.ToJson(m_mapconfig));
            string content = JsonUtility.ToJson(m_mapconfig, true);
            string fileDirectory = m_exportDirectory + "map_chp_" + m_chapter.ToString() 
                                    + "_lvl_" + m_level.ToString() + ".json";

            if (!File.Exists(fileDirectory)){
                File.Create(fileDirectory).Dispose();
            }

            using StreamWriter file = new StreamWriter(fileDirectory, append: false);
            file.WriteLine(content);
        }

        // 错误处理
        private void ShowTip(string tip)
        {
            ShowNotification(new GUIContent(tip));
        }
    }
}
