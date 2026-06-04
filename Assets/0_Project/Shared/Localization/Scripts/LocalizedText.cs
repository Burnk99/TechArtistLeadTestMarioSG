using UnityEngine;
using TMPro;

namespace TechArtistLeadTest.Localization
{
    /// <summary>
    /// Component that automatically translates a TextMeshPro text element.
    /// It subscribes to the LocalizationManager and updates the text whenever the language changes.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
    {
        [Tooltip("The unique string key used to look up the translation in the dictionary.")]
        public string localizationKey;

        private TMP_Text _textComponent;

        private void Awake()
        {
            _textComponent = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            UpdateText();
        }

        private void OnEnable()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged += UpdateText;
            }
        }

        private void OnDisable()
        {
            if (LocalizationManager.Instance != null)
            {
                LocalizationManager.Instance.OnLanguageChanged -= UpdateText;
            }
        }

        /// <summary>
        /// Fetches the translated string from the LocalizationManager based on the localizationKey
        /// and applies it to the TextMeshPro component.
        /// </summary>
        public void UpdateText()
        {
            if (LocalizationManager.Instance != null && !string.IsNullOrEmpty(localizationKey))
            {
                _textComponent.text = LocalizationManager.Instance.GetTranslation(localizationKey);
            }
        }
    }
}
