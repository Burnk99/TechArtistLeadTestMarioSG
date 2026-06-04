using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TechArtistLeadTest.UI.Footer
{
    /// <summary>
    /// Manages the state of the footer navigation bar.
    /// Listens to click events from all ButtonFooterControllers, updates their
    /// selected/deselected states, and animates the sliding indicator between tabs.
    /// </summary>
    public class MenuFooterController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject indicator;
        [SerializeField] private ButtonFooterController startSelected;
        [SerializeField] private List<ButtonFooterController> footerButtons;

        [Header("Animation Settings")]
        // [REFACTOR] Magic numbers exposed to Inspector so designers can tune them.
        [SerializeField] private float _indicatorSmoothTime = 0.08f; // Lower is faster. Gives a fast start, smooth stop.

        // Internal state
        private ButtonFooterController _buttonSelected;
        private GameObject _currentSlot;
        private float _indicatorVelocity;

        IEnumerator Start()
        {
            // Wait for HorizontalLayoutGroup to position the buttons before snapping
            yield return new WaitForEndOfFrame();

            ButtonFooterController targetButton = startSelected;
            
            // [FIX] Auto-select the middle button (Home) if none is assigned in the Inspector
            if (targetButton == null && footerButtons != null && footerButtons.Count > 0)
            {
                targetButton = footerButtons[footerButtons.Count / 2];
            }

            if (targetButton != null)
            {
                OnButtonClickedEvent(targetButton);

                // Snap indicator instantly
                if (_currentSlot != null)
                {
                    indicator.transform.DOKill();
                    indicator.transform.position = new Vector3(_currentSlot.transform.position.x,
                                                               indicator.transform.position.y,
                                                               indicator.transform.position.z);
                }
            }
            else
            {
                indicator.SetActive(false);
            }
        }

        void OnEnable()
        {
            foreach (var btn in footerButtons)
            {
                btn.OnButtonClickedEvent.AddListener(OnButtonClickedEvent);
            }
        }

        void OnDisable()
        {
            foreach (var btn in footerButtons)
            {
                btn.OnButtonClickedEvent.RemoveListener(OnButtonClickedEvent);
            }
        }

        void Update()
        {
            if (indicator.activeInHierarchy && _currentSlot != null)
            {
                // [FIX] Mathf.SmoothDamp perfectly tracks moving targets.
                // Since the HorizontalLayoutGroup shifts the buttons slightly when they animate, 
                // a static tween misses the target. SmoothDamp guarantees we always arrive exactly
                // at the button's final position with a "fast start, smooth slow down" ease.
                float currentX = indicator.transform.position.x;
                float targetX = _currentSlot.transform.position.x;
                
                float newX = Mathf.SmoothDamp(currentX, targetX, ref _indicatorVelocity, _indicatorSmoothTime);
                
                indicator.transform.position = new Vector3(newX,
                                                           indicator.transform.position.y,
                                                           indicator.transform.position.z);
            }
        }


        private void OnButtonClickedEvent(ButtonFooterController buttonClicked)
        {
            if (footerButtons.Contains(buttonClicked))
            {
                // [REFACTOR] Prevent deselection. A bottom navigation bar should always
                // have one active tab and shouldn't hide the indicator if tapped again.
                if (_buttonSelected == buttonClicked)
                {
                    return;
                }

                _buttonSelected = buttonClicked;

                foreach (var btn in footerButtons)
                {
                    btn.SetSelect(_buttonSelected == btn);
                }

                MoveIndicator();
            }
        }

        /// <summary>
        /// Animates the selection indicator to slide to the exact visual center of the newly selected button.
        /// </summary>
        private void MoveIndicator()
        {
            if (_buttonSelected == null) return;

            if (_currentSlot == _buttonSelected.gameObject) return;

            _currentSlot = _buttonSelected.gameObject;

            indicator.SetActive(true);
            
            // Note: The actual animation is now handled dynamically in Update() using SmoothDamp.
            // This prevents the "snap bounce" bug where the LayoutGroup shifts the button's position
            // mid-animation. SmoothDamp automatically adjusts its trajectory on the fly.
        }
    }
} // namespace TechArtistLeadTest.UI.Footer
