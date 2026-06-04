#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TMPro;
using TechArtistLeadTest.Localization;
using UnityEngine.UI;

public class ApplyLocalization
{
    public static void Run()
    {
        // 1. SettingsLine
        string linePath = "Assets/0_Project/Shared/SettingsPopup/Prefabs/SettingsLine.prefab";
        GameObject linePrefab = PrefabUtility.LoadPrefabContents(linePath);
        if (linePrefab != null)
        {
            var text = linePrefab.GetComponentInChildren<TMP_Text>(true);
            if (text != null && text.GetComponent<LocalizedText>() == null)
            {
                text.gameObject.AddComponent<LocalizedText>();
            }
            PrefabUtility.SaveAsPrefabAsset(linePrefab, linePath);
            PrefabUtility.UnloadPrefabContents(linePrefab);
            Debug.Log("Added LocalizedText to SettingsLine");
        }

        // 2. SettingsPopup
        string popupPath = "Assets/0_Project/Shared/SettingsPopup/Prefabs/SettingsPopup.prefab";
        GameObject popupPrefab = PrefabUtility.LoadPrefabContents(popupPath);
        if (popupPrefab != null)
        {
            var titleText = popupPrefab.transform.Find("Title")?.GetComponent<TMP_Text>();
            if (titleText != null && titleText.GetComponent<LocalizedText>() == null)
            {
                var loc = titleText.gameObject.AddComponent<LocalizedText>();
                loc.localizationKey = "SETTINGS_TITLE";
            }
            
            var lines = popupPrefab.GetComponentsInChildren<LocalizedText>(true);
            foreach (var line in lines)
            {
                if (line.transform.parent == null) continue;
                string name = line.transform.parent.name;
                if (name == "SettingsLine_Music") line.localizationKey = "SETTINGS_MUSIC";
                else if (name == "SettingsLine_Privacy") line.localizationKey = "SETTINGS_PRIVACY";
                else if (name == "SettingsLine_Support") line.localizationKey = "SETTINGS_SUPPORT";
                else if (name == "SettingsLine_Language") line.localizationKey = "SETTINGS_LANGUAGE";
                else if (name == "SettingsLine_Notification") line.localizationKey = "SETTINGS_NOTIFICATIONS";
                else if (name == "SettingsLine_Sound") line.localizationKey = "SETTINGS_SOUND";
                else if (name == "SettingsLine_Vibration") line.localizationKey = "SETTINGS_VIBRATION";
                else if (name == "SettingsLine_T&C") line.localizationKey = "SETTINGS_TC";
            }

            var langBtn = popupPrefab.transform.Find("SettingsLine_Language/Toggle");
            if (langBtn != null && langBtn.GetComponent<LanguageSelector>() == null)
            {
                var langSelector = langBtn.gameObject.AddComponent<LanguageSelector>();
                langSelector.flagImage = langBtn.GetComponent<Image>();
                langSelector.englishFlag = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/0_Project/Shared/SettingsPopup/Assets/Sprites/UKUSFlag_Icon.png");
                langSelector.spanishFlag = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/0_Project/Shared/SettingsPopup/Assets/Sprites/ESFlag_Icon.png");
                
                var btn = langBtn.GetComponent<Button>();
                if (btn != null)
                {
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, new UnityEngine.Events.UnityAction(langSelector.ToggleLanguage));
                }
            }

            PrefabUtility.SaveAsPrefabAsset(popupPrefab, popupPath);
            PrefabUtility.UnloadPrefabContents(popupPrefab);
            Debug.Log("Added LocalizedText to SettingsPopup overrides");
        }
        else
        {
            Debug.LogError("Could not find SettingsPopup.prefab");
        }
    }
}
#endif
