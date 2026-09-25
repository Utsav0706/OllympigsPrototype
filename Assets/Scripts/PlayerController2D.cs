using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] Transform visual;

    public bool InputEnabled { get; set; } = true;

    Rigidbody2D rb;
    Vector2 input;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    // Update is called once per frame
    void Update()
    {
        if (!InputEnabled) { input = Vector2.zero; return; }

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = Vector2.ClampMagnitude(input, 1f); 

        if (Mathf.Abs(input.x) > 0.01f && visual != null)
            visual.localScale = new Vector3(Mathf.Sign(input.x) * Mathf.Abs(visual.localScale.x),
                                            visual.localScale.y, visual.localScale.z);
    }

    void FixedUpdate() => rb.linearVelocity = input * moveSpeed;

    public void WarpTo(Vector3 worldPos)
    {
        rb.linearVelocity = Vector2.zero;
        rb.position = worldPos;
        transform.position = worldPos;
    }
}
