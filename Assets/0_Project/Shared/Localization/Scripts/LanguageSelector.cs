using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TechArtistLeadTest.Localization
{
    public class LanguageSelector : MonoBehaviour, IPointerClickHandler
    {
        public Image flagImage;
        public Sprite englishFlag;
        public Sprite spanishFlag;

        public void ToggleLanguage()
        {
            if (LocalizationManager.Instance == null) return;

            Language current = LocalizationManager.Instance.CurrentLanguage;
            Language next = current == Language.English ? Language.Spanish : Language.English;
            
            LocalizationManager.Instance.SetLanguage(next);
            UpdateFlagSprite(next);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ToggleLanguage();
        }

        private void UpdateFlagSprite(Language lang)
        {
            if (flagImage != null)
            {
                flagImage.sprite = lang == Language.English ? englishFlag : spanishFlag;
            }
        }
        
        private void Start()
        {
            if (LocalizationManager.Instance != null)
            {
                UpdateFlagSprite(LocalizationManager.Instance.CurrentLanguage);
            }
        }
    }
}
