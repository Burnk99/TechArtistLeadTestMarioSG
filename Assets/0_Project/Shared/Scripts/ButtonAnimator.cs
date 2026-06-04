using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace TechArtistLeadTest.UI
{
    public class ButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Animation Settings")]
        public float scaleFactor = 0.92f;
        public float animationDuration = 0.15f;
        public Ease easeDown = Ease.OutQuad;
        public Ease easeUp = Ease.OutQuad;
        
        private Vector3 _originalScale;
        private UnityEngine.UI.Selectable _selectable;
        
        private void Awake()
        {
            _originalScale = transform.localScale;
            _selectable = GetComponent<UnityEngine.UI.Selectable>();
        }
        
        private void OnDisable()
        {
            // Reset scale if object gets disabled while pressed to avoid staying shrunk
            transform.localScale = _originalScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_selectable != null && !_selectable.interactable) return;
            
            // Kill any active tweens on the transform's scale before starting a new one
            transform.DOKill(false);
            transform.DOScale(_originalScale * scaleFactor, animationDuration)
                     .SetEase(easeDown)
                     .SetUpdate(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_selectable != null && !_selectable.interactable) return;

            transform.DOKill(false);
            transform.DOScale(_originalScale, animationDuration)
                     .SetEase(easeUp)
                     .SetUpdate(true);
        }
    }
}
