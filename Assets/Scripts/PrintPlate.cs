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

    [Header("Finished plate")]
    [Tooltip("Paper grain overlay. On the stamp the rough proof grain gives way to a cleaner finished plate.")]
    [SerializeField] SpriteRenderer grain;
    [SerializeField, Range(0f, 1f)] float printedGrainAlpha = 0.04f;

    Vector3 homePosition;
    float proofGrainAlpha;

    void Awake()
    {
        homePosition = ochreRoot.localPosition;
        if (grain != null) proofGrainAlpha = grain.color.a;
        SetOchreVisible(false);
    }

    public void SetOchreVisible(bool visible)
    {
        StopAllCoroutines();                      // a pending settle must not act on a later stamp
        foreach (var sr in ochreLayers) sr.enabled = visible;
        ochreRoot.localPosition = homePosition;
        SetGrain(visible ? printedGrainAlpha : proofGrainAlpha);
    }

    void SetGrain(float alpha)
    {
        if (grain == null) return;
        var c = grain.color; c.a = alpha; grain.color = c;   // a straight swap, never a fade
    }

    public void StampOchre()
    {
        // abrupt reveal: no fade, no lerp, no tween
        StopAllCoroutines();
        ochreRoot.localPosition = homePosition + (Vector3)registrationOffset;
        foreach (var sr in ochreLayers) sr.enabled = true;
        SetGrain(printedGrainAlpha);              // same frame: grain gives way as the colour lands

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
