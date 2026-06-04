using UnityEngine;
using TMPro;

namespace TechArtistLeadTest.UI
{

/// <summary>
/// Animates each character of a TextMeshPro text in a sine wave pattern,
/// creating a floating/bouncing effect. Configurable values are exposed
/// in the Inspector so artists can tune the animation without touching code.
/// </summary>
public class SineWaveTextAnimation : MonoBehaviour
{
    [Header("Wave Settings")]
    // [REFACTOR] Replaced const fields with [SerializeField] so artists and designers
    // can tune these values directly from the Inspector without modifying code.
    [SerializeField] private float _amplitude = 5f;    // How far up/down each letter travels
    [SerializeField] private float _frequency = 2f;    // Speed of the wave oscillation
    [SerializeField] private float _waveOffset = 0.2f; // Phase offset between consecutive characters

    private TMP_Text _textMesh;
    private Vector3[] _originalVertices;

    void Start()
    {
        // [NOTE] A null-check is added here to prevent a NullReferenceException
        // if this script is placed on a GameObject that has no TMP_Text component.
        _textMesh = GetComponent<TMP_Text>();
        if (_textMesh == null)
        {
            Debug.LogError($"[SineWaveTextAnimation] No TMP_Text component found on '{gameObject.name}'. Disabling script.", this);
            enabled = false;
            return;
        }

        _textMesh.ForceMeshUpdate();
        // Capture the original, rest-position vertex data once at startup.
        // All per-frame animation offsets are calculated relative to these base positions.
        _originalVertices = _textMesh.mesh.vertices;
    }

    void Update()
    {
        AnimateText();
    }

    void AnimateText()
    {
        // [OPTIMISATION NOTE] ForceMeshUpdate() forces a full TMP mesh rebuild every frame,
        // which is expensive. For production, this should be replaced with:
        //   _textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices)
        // to update only the vertex buffer, avoiding the costly full layout pass.
        _textMesh.ForceMeshUpdate();
        var mesh = _textMesh.mesh;
        var vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            // Each character in TMP is composed of 4 vertices (a quad).
            // Integer division groups all 4 verts of the same character to the same wave phase.
            int charIndex = i / 4;
            float wave = Mathf.Sin(Time.time * _frequency + charIndex * _waveOffset);
            vertices[i].y = _originalVertices[i].y + wave * _amplitude;
        }

        // Write the modified vertex positions back to the mesh and push it to the renderer.
        mesh.vertices = vertices;
        _textMesh.canvasRenderer.SetMesh(mesh);
    }
} // end class SineWaveTextAnimation
} // namespace TechArtistLeadTest.UI
