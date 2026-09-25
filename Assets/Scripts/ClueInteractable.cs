using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ClueInteractable : MonoBehaviour
{
    [SerializeField, TextArea] string poemLine = "The rain keeps the family's time.";
    [SerializeField] GameObject promptVisual;

    bool inRange;
    bool used;

    void Start() => promptVisual.SetActive(false);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (used || !other.CompareTag("Player")) return;
        inRange = true;
        promptVisual.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        inRange = false;
        promptVisual.SetActive(false);
    }

    void Update()
    {
        if (!inRange || used) return;
        if (!Input.GetKeyDown(KeyCode.E)) return;

        used = true;
        promptVisual.SetActive(false);
        StageController.Instance.OnClueFound(poemLine);
    }
}
