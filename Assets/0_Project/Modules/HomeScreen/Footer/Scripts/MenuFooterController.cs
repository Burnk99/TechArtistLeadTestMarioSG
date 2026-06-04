using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


    public class MenuFooterController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject indicator;
        [SerializeField] private ButtonFooterController startSelected;
        [SerializeField] private List<ButtonFooterController> footerButtons;

        //Internal
        private ButtonFooterController _buttonSelected;
        private GameObject _currentSlot;

        void Awake()
        {
            // [FIX] Hide the indicator initially to prevent a visual glitch where it briefly 
            // flashes at the center (0,0) before the HorizontalLayoutGroup calculates the final button positions.
            if (indicator != null) indicator.SetActive(false);
        }

        System.Collections.IEnumerator Start()
        {
            // [FIX] Wait until the end of the frame to ensure the UI Layout system (HorizontalLayoutGroup)
            // has finished positioning all footer buttons before we try to move the indicator to their transform.position.
            yield return new WaitForEndOfFrame();

            ButtonFooterController targetButton = startSelected;
            
            // If not assigned in inspector, auto-select the middle button (Home)
            if (targetButton == null && footerButtons != null && footerButtons.Count > 0)
            {
                targetButton = footerButtons[footerButtons.Count / 2];
            }

            if (targetButton != null)
            {
                OnButtonClickedEvent(targetButton);
                
                // [FIX] Snap the indicator instantly to the selected button on boot.
                // This prevents the indicator from visibly tweening/sliding from the center of the screen
                // when the scene is first loaded.
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


        private void OnButtonClickedEvent(
            ButtonFooterController buttonClicked)
        {
            if (footerButtons.Contains(buttonClicked))
            {
                // [FIX] Prevent deselection. If the user taps the currently active tab, we exit early.
                // Standard bottom navigation bars should always have one active tab and should not 
                // hide the indicator if tapped again.
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

        private void MoveIndicator()
        {
            if (_buttonSelected == null) return;
            if (_currentSlot == _buttonSelected.gameObject) return;
            _currentSlot = _buttonSelected.gameObject;

            indicator.SetActive(true);
            indicator.transform.DOKill();
            
            float duration = 0.35f;
            float stretchScale = 1.35f; // Reduced from 2.0f for a more subtle stretching effect

            Sequence seq = DOTween.Sequence();
            seq.SetUpdate(true);

            // Move the indicator to the target using a smooth InOut ease
            seq.Append(indicator.transform.DOMoveX(_currentSlot.transform.position.x, duration).SetEase(Ease.InOutQuad));

            // Simultaneously stretch it horizontally during the first half of the movement, then squash back during the second half
            seq.Insert(0, indicator.transform.DOScaleX(stretchScale, duration / 2f).SetEase(Ease.OutQuad));
            seq.Insert(duration / 2f, indicator.transform.DOScaleX(1f, duration / 2f).SetEase(Ease.InQuad));
            
            seq.OnComplete(() =>
            {
                indicator.transform.position = new Vector3(_currentSlot.transform.position.x,
                                                            indicator.transform.position.y,
                                                            indicator.transform.position.z);
                // Ensure scale resets perfectly
                indicator.transform.localScale = Vector3.one;
            });
        }
    }
