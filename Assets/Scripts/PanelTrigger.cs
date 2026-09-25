using UnityEngine;

public enum TriggerKind { PanelExit, Arrival }

[RequireComponent(typeof(Collider2D))]
public class PanelTrigger : MonoBehaviour
{
    [SerializeField] TriggerKind kind = TriggerKind.PanelExit;
    [SerializeField] int panelIndex;   // only used for PanelExit

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (kind == TriggerKind.PanelExit)
            StageController.Instance.OnPanelExit(panelIndex);
        else
            StageController.Instance.OnArrivalReached();
    }
}
