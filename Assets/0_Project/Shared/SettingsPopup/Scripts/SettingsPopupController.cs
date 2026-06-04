using UnityEngine;
using UnityEngine.Serialization;

namespace TechArtistLeadTest.UI
{

/// <summary>
/// Controls the Settings Popup lifecycle: opening, closing, and reacting to
/// the close animation event. The API is self-contained — callers should use
/// Open() and OnCloseButtonClicked() instead of toggling SetActive() externally.
/// </summary>
public class SettingsPopupController : MonoBehaviour
{
    // [FormerlySerializedAs] tells Unity to remap the old serialized field name "animator"
    // to the new name "_animator", preserving all existing Prefab references after the rename.
    [FormerlySerializedAs("animator")]
    [SerializeField] private Animator _animator;

    // [REFACTOR] Animator trigger name cached as a hash to avoid magic strings.
    // This prevents typo-silent failures and is faster at runtime than string lookups.
    private static readonly int CloseHash = Animator.StringToHash("Close");

    /// <summary>
    /// Opens the popup. Activates the GameObject so the Animator can run
    /// its entrance state automatically on enable.
    /// </summary>
    public void Open()
    {
        // [FIX] Previously, opening was handled externally via SetActive(true), 
        // creating an implicit contract that callers had to know about.
        // Centralising it here makes the API explicit and self-contained.
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Triggers the close animation. The GameObject is deactivated at the end
    /// of the animation via the OnClosedAnimationCompleted event (called by Animation Event).
    /// </summary>
    public void OnCloseButtonClicked()
    {
        // [REFACTOR] Using cached hash instead of raw string "Close"
        _animator.SetTrigger(CloseHash);
    }

    /// <summary>
    /// Called via Animation Event at the end of the close animation.
    /// Deactivates the GameObject to return it to its pooled/hidden state.
    /// </summary>
    public void OnClosedAnimationCompleted()
    {
        gameObject.SetActive(false);
    }
} // end class SettingsPopupController
} // namespace TechArtistLeadTest.UI
