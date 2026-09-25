using UnityEngine;
using UnityEngine.SceneManagement;

public enum StageState { Crossing, Clue, Arrival, Printed }

public class StageController : MonoBehaviour
{
    public static StageController Instance { get; private set; }

    [SerializeField] PlayerController2D player;
    [SerializeField] CameraRig cameraRig;
    [SerializeField] PrintPlate plate;
    [SerializeField] Collider2D exitBlocker;      // panel 02 exit, ON at start
    [SerializeField] Transform[] panelEntryPoints; // 0,1,2

    public StageState State { get; private set; } = StageState.Crossing;
    bool clueFound;
    bool hasPrinted;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        exitBlocker.enabled = true;
        plate.SetOchreVisible(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Restart();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void OnPanelExit(int fromPanel)
    {
        int next = fromPanel + 1;
        if (next >= panelEntryPoints.Length) return;
        if (next == 2 && !clueFound) return;        // gate, belt and braces

        player.WarpTo(panelEntryPoints[next].position);
        cameraRig.CutToPanel(next);

        if (next == 1) State = StageState.Clue;
    }

    public void OnClueFound(string poemLine)
    {
        if (clueFound) return;                      // one-shot
        clueFound = true;
        State = StageState.Arrival;
        exitBlocker.enabled = false;                // unlock the exit
        Debug.Log($"Clue found: {poemLine}");
    }

    public void OnArrivalReached()
    {
        if (hasPrinted || !clueFound) return;       // prevents repeat triggers
        hasPrinted = true;
        State = StageState.Printed;
        plate.StampOchre();
    }

    // Full reload over a hand-written reset: fewer failure modes for a clean second run.
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
