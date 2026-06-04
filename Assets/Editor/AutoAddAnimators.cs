using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TechArtistLeadTest.UI;

public class AutoAddAnimators
{
    [MenuItem("Tools/Add Button Animators")]
    public static void InjectAnimators()
    {
        try 
        {
            // Inject to all explicit prefabs that we know act as buttons
            string[] specificPrefabs = new string[] 
            {
                "Assets/0_Project/Shared/Prefabs/GenericTextButton.prefab",
                "Assets/0_Project/Shared/Prefabs/GenericIconButton.prefab",
                "Assets/0_Project/Shared/Prefabs/GreenButton.prefab",
                "Assets/0_Project/Shared/SettingsPopup/Prefabs/LightBlueButton.prefab",
                "Assets/0_Project/Modules/HomeScreen/Footer/Prefabs/FooterButton.prefab",
                "Assets/0_Project/Modules/HomeScreen/Header/Prefabs/BuyMoreButton.prefab",
                "Assets/0_Project/Modules/HomeScreen/Header/Prefabs/SettingsButton.prefab",
                "Assets/0_Project/Modules/LevelCompletedScreen/Prefabs/BackToMenuButton.prefab",
                "Assets/0_Project/Modules/LevelCompletedScreen/Prefabs/WatchVideoButton.prefab",
                "Assets/0_Project/Shared/Prefabs/GenericPopup.prefab"
            };

            foreach (var path in specificPrefabs)
            {
                if (!System.IO.File.Exists(path)) continue;

                GameObject p = PrefabUtility.LoadPrefabContents(path);
                if (p != null)
                {
                    bool changed = false;

                    // Find everything that has a Button component
                    var buttons = p.GetComponentsInChildren<Button>(true);
                    foreach (var btn in buttons)
                    {
                        if (btn.GetComponent<ButtonAnimator>() == null)
                        {
                            btn.gameObject.AddComponent<ButtonAnimator>();
                            changed = true;
                        }
                    }

                    if (changed)
                    {
                        PrefabUtility.SaveAsPrefabAsset(p, path);
                        Debug.Log("[Animator] Added ButtonAnimator to buttons in " + path);
                    }
                    PrefabUtility.UnloadPrefabContents(p);
                }
            }

            // Also explicitly add to the SettingsLine Toggle arrow
            string settingsLinePath = "Assets/0_Project/Shared/SettingsPopup/Prefabs/SettingsLine.prefab";
            if (System.IO.File.Exists(settingsLinePath))
            {
                GameObject linePrefab = PrefabUtility.LoadPrefabContents(settingsLinePath);
                var toggleArrow = linePrefab.transform.Find("Toggle");
                if (toggleArrow != null && toggleArrow.GetComponent<ButtonAnimator>() == null)
                {
                    toggleArrow.gameObject.AddComponent<ButtonAnimator>();
                    PrefabUtility.SaveAsPrefabAsset(linePrefab, settingsLinePath);
                    Debug.Log("[Animator] Added ButtonAnimator to SettingsLine Toggle arrow");
                }
                PrefabUtility.UnloadPrefabContents(linePrefab);
            }

            EditorUtility.DisplayDialog("Success", "¡Micro-animaciones (DOTween) inyectadas en todos los botones!", "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[Animator] Failed: " + e.Message);
            EditorUtility.DisplayDialog("Error", "An error occurred: " + e.Message, "OK");
        }
    }
}
