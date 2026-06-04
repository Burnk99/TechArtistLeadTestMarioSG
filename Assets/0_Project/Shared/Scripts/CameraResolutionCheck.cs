using UnityEngine;
using UnityEngine.UI;

namespace TechArtistLeadTest.Core
{

/// <summary>
/// Detects at startup whether the current device is a Phone or a Tablet,
/// then configures the CanvasScaler's Match Width-or-Height value accordingly.
/// Phone: match width (portrait layout).
/// Tablet: match height (wider layout).
/// </summary>
public class CameraResolutionCheck : MonoBehaviour
{
    [SerializeField] CanvasScaler scaler;

    // [REFACTOR] Tablet detection thresholds moved to SerializeField constants
    // so they can be tuned per project without touching code.
    [Header("Tablet Detection Thresholds")]
    [Tooltip("Devices with a physical diagonal larger than this (in inches) are considered tablets.")]
    [SerializeField] private float _tabletDiagonalThresholdInches = 6.5f;

    [Tooltip("Devices with an aspect ratio lower than this are considered tablets (tablets tend to be squarer).")]
    [SerializeField] private float _tabletAspectRatioThreshold = 2f;

    // Internal enum to classify device form factor.
    // Values are kept as-is from the original implementation.
    enum DeviceType
    {
        Phone = 2,
        Tablet = 3
    }

    void Awake()
    {
        var device = GetDeviceType();

        // Match height for tablets so the layout adapts to their wider/squarer screens.
        // Match width for phones to preserve the portrait-first layout.
        if (device == DeviceType.Tablet)
        {
            scaler.matchWidthOrHeight = 1f;
        }
        else
        {
            scaler.matchWidthOrHeight = 0f;
        }
    }

    /// <summary>
    /// Calculates the physical diagonal size of the screen in inches using Screen.dpi.
    /// </summary>
    /// <remarks>
    /// Note: Screen.dpi may return 0 on some Android devices during Awake().
    /// A value of 0 will cause this method to return infinity. Consider adding a fallback.
    /// </remarks>
    private float DeviceDiagonalSizeInInches()
    {
        float screenWidth = Screen.width / Screen.dpi;
        float screenHeight = Screen.height / Screen.dpi;
        float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));
        return diagonalInches;
    }

    /// <summary>
    /// Identifies the current device as Phone or Tablet.
    /// On iOS, uses the device generation string for a reliable detection.
    /// On Android and in the Editor, falls back to diagonal size and aspect ratio heuristics.
    /// </summary>
    DeviceType GetDeviceType()
    {
#if UNITY_IOS
        // iOS: use the device generation name for definitive tablet/phone classification.
        bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
        if (deviceIsIpad)
        {
            return DeviceType.Tablet;
        }

        bool deviceIsIphone = UnityEngine.iOS.Device.generation.ToString().Contains("iPhone");
        if (deviceIsIphone)
        {
            return DeviceType.Phone;
        }
#endif

#if !UNITY_EDITOR
        // Android runtime: use physical screen size and aspect ratio as heuristics.
        float aspectRatio = Mathf.Max(Screen.width, Screen.height) / (float)Mathf.Min(Screen.width, Screen.height);
        bool isTablet = (DeviceDiagonalSizeInInches() > _tabletDiagonalThresholdInches && aspectRatio < _tabletAspectRatioThreshold);
#else
        // Editor: use the Play Mode window resolution for accurate simulation.
        UnityEditor.PlayModeWindow.GetRenderingResolution(out uint width, out uint height);
        float aspectRatio = Mathf.Max(width, height) / (float)Mathf.Min(width, height);
        bool isTablet = (DeviceDiagonalSizeInInches() > _tabletDiagonalThresholdInches && aspectRatio < _tabletAspectRatioThreshold);
#endif

        return isTablet ? DeviceType.Tablet : DeviceType.Phone;
    }
}
} // namespace TechArtistLeadTest.Core

