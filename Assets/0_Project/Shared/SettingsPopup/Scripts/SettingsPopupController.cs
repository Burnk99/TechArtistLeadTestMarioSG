using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using DG.Tweening;

namespace TechArtTeamLeadTest.UI
{

/// <summary>
/// Controls the Settings Popup lifecycle asynchronously via DOTween.
/// Eliminates Animator overhead, Animation Events, and logic coupling.
/// </summary>
public class SettingsPopupController : MonoBehaviour
{
    [FormerlySerializedAs("animator")]
    [SerializeField] private Animator _animator;

    [Header("Animation Polish (DOTween)")]
    [SerializeField] private float _animationDuration = 0.35f;
    [SerializeField] private Transform _popupContent;
    [SerializeField] private Image _backgroundBlocker;

    private void Awake()
    {
        if (_animator != null) _animator.enabled = false;
        
        // 1. Logical Decoupling: Dynamically isolate the Panel and Background 
        // bypassing rigid prefab hierarchies. We use recursive search to find them
        // no matter how the user flattened the hierarchy.
        if (_popupContent == null)
        {
            _popupContent = FindChildRecursive(transform, "Panel");
            if (_popupContent == null) _popupContent = transform; // Fallback
        }

        if (_backgroundBlocker == null)
        {
            Transform bg = FindChildRecursive(transform, "Background");
            if (bg != null) _backgroundBlocker = bg.GetComponent<Image>();
        }
    }

    private Transform FindChildRecursive(Transform parent, string exactName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == exactName) return child;
            Transform found = FindChildRecursive(child, exactName);
            if (found != null) return found;
        }
        return null;
    }

    private void OnEnable()
    {
        _popupContent.DOKill();
        _popupContent.localScale = Vector3.zero;
        _popupContent.DOScale(Vector3.one, _animationDuration)
            .SetEase(Ease.OutBack).SetUpdate(true);

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
        // Callback-driven lifecycle
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
}
}
