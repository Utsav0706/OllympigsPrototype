using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [SerializeField] Transform[] panelAnchors; // 0,1,2 — panel centres

    // Hard cut, no easing: the comic-panel jump is the point.
    public void CutToPanel(int index)
    {
        if (index < 0 || index >= panelAnchors.Length) return;

        Vector3 anchor = panelAnchors[index].position;
        transform.position = new Vector3(anchor.x, anchor.y, transform.position.z);
    }
}
