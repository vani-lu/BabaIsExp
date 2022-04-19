using System.IO;
using System.Collections.Generic;
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

        private int m_chapter = 0;

        private int m_level = 0;

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

            if (m_isParsable){
                if (m_chapter > 0 && m_level > 0){
                    GUI.enabled = true;
                }
                else{
                    ShowTip("请输入正整数");
                }
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
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            if (GUILayout.Button("TestFlattenArray", GUILayout.Width(150))){
                FlattenArray();
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

        private void FlattenArray()
        {
            List<int> a1 = new List<int>(){ 1, 2, 3};
            List<int> a2 = new List<int>(){ 2, 2, 3};
            List<int> a3 = new List<int>(){ 3, 2, 3};

            List<int> b1 = new List<int>(){ 4, 5, 6};
            List<int> b2 = new List<int>(){ 5, 5, 6};
            List<int> b3 = new List<int>(){ 6, 5, 6};

            List<int>[,] arbmap = new List<int>[2,3]{
                {a1, a2, a3},
                {b1, b2, b3}
            };

            List<List<int>> listOfLists = Array2List<int>(arbmap);
            // 通过array的宽高可以复原出array
            //List<int> flattenedList = listOfLists.SelectMany(d => d).ToList();
        }
        
        // 错误处理
        private void ShowTip(string tip)
        {
            ShowNotification(new GUIContent(tip));
        }

        public static List<List<T>> Array2List<T>(List<T>[,] array)
        {
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            List<List<T>> ret = new List<List<T>>(width * height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    ret.Add(array[i, j]);
                }
            }
            return ret;
        }

    }
}
