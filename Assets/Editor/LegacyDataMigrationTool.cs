using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TechArtTeamLeadTest.EditorTools
{
    /// <summary>
    /// Tool created to safely migrate legacy string-based UnityEvents to type-safe methods 
    /// and enforce scene hierarchy rules without breaking prefab or scene GUIDs.
    /// </summary>
    public class LegacyDataMigrationTool : EditorWindow
    {
        [MenuItem("Tools/TechArtTeamLeadTest/Migrate Legacy Data (Hierarchy & Navigation)")]
        public static void ApplyFixes()
        {
            FixNavigationPrefab();
            FixScene("Assets/0_Project/Scenes/HomeScreen.unity", "HomeScreen", "LoadLevelCompletedScreen");
            FixScene("Assets/0_Project/Scenes/LevelCompletedScreen.unity", "LevelCompletedScreen", "LoadHomeScreen");
            AssetDatabase.SaveAssets();
            Debug.Log("Legacy data migration applied successfully!");
        }

        private static void FixNavigationPrefab()
        {
            string prefabPath = "Assets/0_Project/Modules/LevelCompletedScreen/Prefabs/BackToMenuButton.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null)
            {
                // Use GetComponentInChildren to ensure we find the Button even if it's not on the root
                Button btn = prefab.GetComponentInChildren<Button>(true);
                if (btn != null)
                {
                    // Rewire UnityEvent
                    SerializedObject so = new SerializedObject(btn);
                    SerializedProperty persistentCalls = so.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");
                    for (int i = 0; i < persistentCalls.arraySize; i++)
                    {
                        SerializedProperty call = persistentCalls.GetArrayElementAtIndex(i);
                        if (call.FindPropertyRelative("m_MethodName").stringValue == "LoadScene")
                        {
                            call.FindPropertyRelative("m_MethodName").stringValue = "LoadHomeScreen";
                            call.FindPropertyRelative("m_Mode").enumValueIndex = 1; // 1 is Void mode
                        }
                    }
                    so.ApplyModifiedProperties();
                    
                    // PrefabUtility is required in modern Unity to properly save prefab changes to disk
                    PrefabUtility.SavePrefabAsset(prefab);
                }
            }
        }

        private static void FixScene(string scenePath, string sceneName, string correctTargetMethod)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            
            // 1. Scene Hierarchy Grouping
            GameObject root = GameObject.Find("[Scene] " + sceneName);
            if (root == null)
            {
                root = new GameObject("[Scene] " + sceneName);
            }

            // Move all root objects into the new root, EXCEPT the root itself
            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (var go in rootObjects)
            {
                if (go != root)
                {
                    go.transform.SetParent(root.transform);
                }
            }

            // 2. Fix Navigation Controller in Scene
            // Find buttons that might have the old LoadScene method
            Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
            foreach (var btn in buttons)
            {
                // Only modify buttons that belong to the current scene
                if (btn.gameObject.scene == scene)
                {
                    SerializedObject so = new SerializedObject(btn);
                    SerializedProperty persistentCalls = so.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");
                    bool modified = false;
                    for (int i = 0; i < persistentCalls.arraySize; i++)
                    {
                        SerializedProperty call = persistentCalls.GetArrayElementAtIndex(i);
                        if (call.FindPropertyRelative("m_MethodName").stringValue == "LoadScene")
                        {
                            call.FindPropertyRelative("m_MethodName").stringValue = correctTargetMethod;
                            call.FindPropertyRelative("m_Mode").enumValueIndex = 1; // Void mode
                            modified = true;
                        }
                    }
                    if (modified)
                    {
                        so.ApplyModifiedProperties();
                        EditorUtility.SetDirty(btn);
                    }
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }
}
