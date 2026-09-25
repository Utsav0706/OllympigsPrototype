using UnityEngine;

// Scrolls the rain texture along its slant via material offset. Sprite shaders in the
// URP 2D renderer ignore texture offset, so the rain is a quad with an Unlit material.
// Speed breathes on two out-of-step sine gusts so the fall never reads as a conveyor belt.
[RequireComponent(typeof(Renderer))]
public class RainScroller : MonoBehaviour
{
    [SerializeField] float fallSpeed = 2.4f;        // tiles per second along the streaks
    [SerializeField] float slantDegrees = 15f;      // must match the streaks in Rain.png
    [SerializeField, Range(0f, 0.5f)] float gust = 0.18f;
    [SerializeField] float drift = 0.04f;           // slow sideways sway, tiles per second

    Material mat;
    Vector2 offset;
    Vector2 fallDir;

    void Awake()
    {
        mat = GetComponent<Renderer>().material;    // instance, so the asset is never edited
        float a = slantDegrees * Mathf.Deg2Rad;
        // content moves down-right on screen, so the offset moves up-left in UV space
        fallDir = new Vector2(-Mathf.Sin(a), Mathf.Cos(a));
    }

    void Update()
    {
        float t = Time.time;
        float speed = fallSpeed * (1f + gust * (0.6f * Mathf.Sin(t * 0.7f) + 0.4f * Mathf.Sin(t * 1.9f + 1.3f)));
        offset += fallDir * speed * Time.deltaTime;
        offset.x += drift * Mathf.Sin(t * 0.33f) * Time.deltaTime;
        offset.x = Mathf.Repeat(offset.x, 1f);
        offset.y = Mathf.Repeat(offset.y, 1f);
        mat.mainTextureOffset = offset;
    }

    void OnDestroy()
    {
        if (mat != null) Destroy(mat);
    }
}
