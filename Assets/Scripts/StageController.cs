using UnityEngine;

public enum StageState { Crossing, Clue, Arrival, Printed }

public class StageController : MonoBehaviour
{
    public static StageController Instance { get; private set; }

    [SerializeField] PlayerController2D player;
    [SerializeField] Collider2D exitBlocker;      // panel 02 exit, ON at start
    [SerializeField] Transform[] panelEntryPoints; // 0,1,2

    public StageState State { get; private set; } = StageState.Crossing;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void OnPanelExit(int fromPanel)
    {
        int next = fromPanel + 1;
        if (next >= panelEntryPoints.Length) return;

        player.WarpTo(panelEntryPoints[next].position);

        if (next == 1) State = StageState.Clue;
    }

    public void OnClueFound(string poemLine)
    {
    }

    public void OnArrivalReached()
    {
    }

    public void Restart()
    {
    }
}
