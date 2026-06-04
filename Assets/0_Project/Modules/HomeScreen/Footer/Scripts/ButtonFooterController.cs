using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TechArtistLeadTest.UI.Footer
{
    /// <summary>
    /// Controls the visual and interactive state of a single footer navigation button.
    /// Drives the Animator between Selected, Unselected, and Locked states,
    /// and broadcasts a click event upward to the MenuFooterController.
    /// </summary>
    public class ButtonFooterController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private Button footerBtn;
        [SerializeField] private bool lockOnAwake;

        [Header("Events")]
        public UnityEvent<ButtonFooterController> OnButtonClickedEvent;

        // [REFACTOR] Animator parameter names cached as hashes to avoid fragile magic strings.
        // Using StringToHash prevents typo-silent failures and is faster than string comparisons at runtime.
        private static readonly int LockedHash = Animator.StringToHash("Locked");
        private static readonly int SelectedHash = Animator.StringToHash("Selected");

        // Internal state
        private bool _selected;
        private bool _locked;

        void Awake()
        {
            SetLock(lockOnAwake);
        }

        void Start()
        {
            // [FIX] RemoveAllListeners() is called before AddListener() to prevent duplicate
            // event registrations if this GameObject is disabled and re-enabled at runtime.
            footerBtn.onClick.RemoveAllListeners();
            footerBtn.onClick.AddListener(() =>
            {
                OnButtonClickedEvent?.Invoke(this);
            });
        }

        /// <summary>
        /// Locks or unlocks this button. A locked button is non-interactable
        /// and plays the Locked animation state.
        /// </summary>
        public void SetLock(bool locked)
        {
            _locked = locked;
            footerBtn.interactable = !_locked;

            // [REFACTOR] Using cached hash instead of raw string "Locked"
            animator.SetBool(LockedHash, _locked);
        }

        /// <summary>
        /// Selects or deselects this button, driving the Animator into
        /// the Selected or Unselected state accordingly.
        /// </summary>
        public void SetSelect(bool selected)
        {
            _selected = selected;

            // [REFACTOR] Using cached hash instead of raw string "Selected"
            animator.SetBool(SelectedHash, _selected);
        }
    }
} // namespace TechArtistLeadTest.UI.Footer
