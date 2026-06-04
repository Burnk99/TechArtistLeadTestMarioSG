using System;
using System.Collections.Generic;
using UnityEngine;

namespace TechArtistLeadTest.Localization
{
    /// <summary>
    /// Represents the supported languages in the application.
    /// </summary>
    public enum Language { English, Spanish }

    /// <summary>
    /// Data structure holding the translation strings for a specific key.
    /// Used for JSON serialization.
    /// </summary>
    [Serializable]
    public class TranslationData
    {
        public string key;
        public string en;
        public string es;
    }

    /// <summary>
    /// Wrapper class for the list of translations. Required for Unity's JsonUtility to deserialize JSON arrays.
    /// </summary>
    [Serializable]
    public class LocalizationDictionary
    {
        public List<TranslationData> translations;
    }

    /// <summary>
    /// Singleton manager responsible for loading localization data from JSON,
    /// storing the active language, and providing translation lookups.
    /// Notifies subscribed listeners when the language is changed.
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        private static LocalizationManager _instance;
        private static bool _applicationIsQuitting = false;

        public static LocalizationManager Instance 
        { 
            get 
            {
                if (_applicationIsQuitting) return null;

                if (_instance == null)
                {
                    var go = new GameObject("LocalizationManager");
                    _instance = go.AddComponent<LocalizationManager>();
                }
                return _instance;
            } 
            private set { _instance = value; } 
        }

        public Language CurrentLanguage { get; private set; } = Language.English;
        public Action OnLanguageChanged;

        private Dictionary<string, TranslationData> _dictionary = new Dictionary<string, TranslationData>();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadDictionary();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _applicationIsQuitting = true;
            }
        }

        private void LoadDictionary()
        {
            TextAsset jsonText = Resources.Load<TextAsset>("localization");
            if (jsonText != null)
            {
                LocalizationDictionary dict = JsonUtility.FromJson<LocalizationDictionary>(jsonText.text);
                foreach (var translation in dict.translations)
                {
                    _dictionary[translation.key] = translation;
                }
            }
            else
            {
                Debug.LogError("LocalizationManager: Could not find Resources/localization.json");
            }
        }

        public void SetLanguage(Language lang)
        {
            CurrentLanguage = lang;
            OnLanguageChanged?.Invoke();
        }

        public string GetTranslation(string key)
        {
            if (_dictionary.TryGetValue(key, out var data))
            {
                return CurrentLanguage == Language.English ? data.en : data.es;
            }
            return key; // Fallback to key if not found
        }
    }
}
