using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

namespace TechArtistLeadTest.UI
{
    /// <summary>
    /// Autonomous component that handles visual toggling (Sprite swapping & Animation)
    /// and exposes a UnityEvent for programmers to hook into logic (Audio, Haptics, etc).
    /// </summary>
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class UIToggle : MonoBehaviour
    {
        [Header("Visual States")]
        [SerializeField] private Sprite _toggleOnSprite;
        [SerializeField] private Sprite _toggleOffSprite;
        [SerializeField] private bool _isOn = true;

        [Header("Events")]
        public UnityEvent<bool> OnValueChanged;

        private Button _button;
        private Image _image;

        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (_isOn != value)
                {
                    _isOn = value;
                    UpdateVisuals(animate: true);
                    OnValueChanged?.Invoke(_isOn);
                }
            }
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();

            _button.onClick.AddListener(ToggleState);
        }

        private void OnEnable()
        {
            UpdateVisuals(animate: false);
        }

        private void ToggleState()
        {
            IsOn = !IsOn;
        }

        private void UpdateVisuals(bool animate)
        {
            if (_image == null || _toggleOnSprite == null || _toggleOffSprite == null) return;

            // Swap sprite
            _image.sprite = _isOn ? _toggleOnSprite : _toggleOffSprite;

            // Optional: DOTween Juice if requested
            if (animate)
            {
                transform.DOKill();
                transform.localScale = Vector3.one;
                transform.DOPunchScale(new Vector3(0.15f, 0.15f, 0), 0.2f, 5, 0.5f).SetUpdate(true);
            }
        }

        private void OnDestroy()
        {
            if (_button != null) _button.onClick.RemoveListener(ToggleState);
        }
    }
}
