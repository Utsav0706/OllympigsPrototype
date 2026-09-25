using UnityEngine;
using UnityEngine.UI;

// Cartouche, objective line, poem scroll, completion label. No logic, only display:
// StageController decides what to say and when. The restart seal wires itself to
// StageController.Restart through its Button, so it needs nothing here.
public class StageUI : MonoBehaviour
{
    [SerializeField] Text cartouche;
    [SerializeField] GameObject objectiveRoot;
    [SerializeField] Text objective;
    [SerializeField] GameObject scrollRoot;
    [SerializeField] Text scroll;
    [SerializeField] GameObject completionRoot;
    [SerializeField] Text completion;

    public void SetCartouche(string name) => cartouche.text = name;

    // An empty objective hides the whole slip rather than leaving a blank bar.
    public void SetObjective(string line)
    {
        objective.text = line;
        objectiveRoot.SetActive(!string.IsNullOrEmpty(line));
    }

    public void ShowScroll(string line)
    {
        scroll.text = line;
        scrollRoot.SetActive(true);
    }

    public void HideScroll() => scrollRoot.SetActive(false);

    public void ShowCompletion(string caption)
    {
        completion.text = caption;
        completionRoot.SetActive(true);
    }
}
