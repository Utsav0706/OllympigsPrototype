using System.Collections;
using UnityEngine;

public class PrintPlate : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] ochreLayers;   // every ochre block in the stage
    [SerializeField] Transform ochreRoot;            // offset is applied here

    [Header("Registration")]
    [Tooltip("100 PPU: 1px = 0.01 world units")]
    [SerializeField] Vector2 registrationOffset = new Vector2(0.02f, -0.02f);   // ~2 screen px at 1080p: a misprint, not a shadow
    [Tooltip("True = permanent misregistration. False = offset settles after a beat.")]
    [SerializeField] bool retainOffset = true;
    [Tooltip("Seconds before the offset snaps back into register, when Retain Offset is off.")]
    [SerializeField, Min(0f)] float settleDelay = 0.09f;

    Vector3 homePosition;

    void Awake()
    {
        homePosition = ochreRoot.localPosition;
        SetOchreVisible(false);
    }

    public void SetOchreVisible(bool visible)
    {
        StopAllCoroutines();                      // a pending settle must not act on a later stamp
        foreach (var sr in ochreLayers) sr.enabled = visible;
        ochreRoot.localPosition = homePosition;
    }

    public void StampOchre()
    {
        // abrupt reveal: no fade, no lerp, no tween
        StopAllCoroutines();
        ochreRoot.localPosition = homePosition + (Vector3)registrationOffset;
        foreach (var sr in ochreLayers) sr.enabled = true;

        if (!retainOffset) StartCoroutine(SettleRegistration());
    }

    IEnumerator SettleRegistration()
    {
        yield return new WaitForSeconds(settleDelay);
        ochreRoot.localPosition = homePosition;   // snap, never Lerp
    }

    // Inspector test hooks (right-click the component header, in Play mode): flip Retain Offset,
    // then Reset and Stamp again to compare permanent misregistration with the settle.
    [ContextMenu("Test: Stamp now")]
    void TestStamp() { if (Application.isPlaying) StampOchre(); }

    [ContextMenu("Test: Reset to blue")]
    void TestReset() { if (Application.isPlaying) SetOchreVisible(false); }
}
