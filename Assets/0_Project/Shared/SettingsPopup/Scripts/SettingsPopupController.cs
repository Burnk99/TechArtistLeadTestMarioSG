using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;

namespace TechArtistLeadTest.UI
{

/// <summary>
/// Controls the Settings Popup lifecycle: opening, closing, and reacting to
/// the close animation event. The API is self-contained — callers should use
/// Open() and OnCloseButtonClicked() instead of toggling SetActive() externally.
/// </summary>
public class SettingsPopupController : MonoBehaviour
{
    [FormerlySerializedAs("animator")]
    [SerializeField] private Animator _animator;

    [Header("Animation Polish (DOTween)")]
    [SerializeField] private float _animationDuration = 0.35f;
    [SerializeField] private Transform _popupContent;
    [SerializeField] private UnityEngine.UI.Image _backgroundBlocker;

    private void Awake()
    {
        if (_animator != null) _animator.enabled = false;
        
        // Find the visual popup box (Panel) instead of the whole Content container
        if (_popupContent == null)
        {
            Transform contentObj = transform.Find("Content");
            if (contentObj != null)
            {
                _popupContent = contentObj.Find("Panel");
                if (_popupContent == null) _popupContent = contentObj;
            }
            else
            {
                _popupContent = transform;
            }
        }

        // Find the Background correctly (it's inside Content)
        if (_backgroundBlocker == null)
        {
            Transform contentObj = transform.Find("Content");
            Transform bg = contentObj != null ? contentObj.Find("Background") : transform.Find("Background");
            if (bg != null) _backgroundBlocker = bg.GetComponent<UnityEngine.UI.Image>();
        }
    }

    private void OnEnable()
    {
        _popupContent.DOKill();
        _popupContent.localScale = Vector3.zero;
        _popupContent.DOScale(Vector3.one, _animationDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        if (_backgroundBlocker != null)
        {
            _backgroundBlocker.DOKill();
            _backgroundBlocker.color = new Color(0f, 0f, 0f, 0f);
            _backgroundBlocker.DOFade(0.5f, _animationDuration * 0.8f).SetUpdate(true);
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void OnCloseButtonClicked()
    {
        _popupContent.DOKill();
        _popupContent.DOScale(Vector3.zero, _animationDuration * 0.7f)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() => gameObject.SetActive(false));

        if (_backgroundBlocker != null)
        {
            _backgroundBlocker.DOKill();
            _backgroundBlocker.DOFade(0f, _animationDuration * 0.6f).SetUpdate(true);
        }
    }
} // end class SettingsPopupController
} // namespace TechArtistLeadTest.UI
