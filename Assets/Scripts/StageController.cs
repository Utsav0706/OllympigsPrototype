using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum StageState { Crossing, Clue, Arrival, Printed }

public class StageController : MonoBehaviour
{
    public static StageController Instance { get; private set; }

    [SerializeField] PlayerController2D player;
    [SerializeField] CameraRig cameraRig;
    [SerializeField] PrintPlate plate;
    [SerializeField] StageUI ui;
    [SerializeField] Collider2D exitBlocker;      // panel 02 exit, ON at start
    [SerializeField] Transform[] panelEntryPoints; // 0,1,2
    [SerializeField] float cutFreeze = 0.2f;       // seconds of no input after a panel cut
    [Tooltip("Hold Ollypig still once the Ochre prints. Untick only to test that walking out of the arrival trigger and back in cannot stamp twice.")]
    [SerializeField] bool freezeOnPrint = true;

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
        ui.SetCartouche("OLLYPIG SWINE");
        ui.SetObjective("Use WASD to cross to the right side.");
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
        StartCoroutine(FreezeInput(cutFreeze));

        if (next == 1)
        {
            State = StageState.Clue;
            ui.SetObjective("Walk to the note on the shop wall and press E.");
        }
        else if (next == 2)
        {
            ui.HideScroll();                        // keep the arrival panel clear for the stamp
            ui.SetObjective("Step onto the podium.");
        }
    }

    // A beat of stillness after the cut so the eye can read the new panel
    // before she moves, and a held key doesn't carry her straight on.
    IEnumerator FreezeInput(float seconds)
    {
        player.InputEnabled = false;
        yield return new WaitForSeconds(seconds);
        if (State != StageState.Printed) player.InputEnabled = true;
    }

    public void OnClueFound(string poemLine)
    {
        if (clueFound) return;                      // one-shot
        clueFound = true;
        State = StageState.Arrival;
        exitBlocker.enabled = false;                // unlock the exit
        ui.ShowScroll(poemLine);
        ui.SetObjective("The way is open. Keep going right.");
    }

    public void OnArrivalReached()
    {
        if (hasPrinted || !clueFound) return;       // prevents repeat triggers
        hasPrinted = true;
        State = StageState.Printed;
        if (freezeOnPrint) player.InputEnabled = false;   // she stops dead; R or the seal still restart
        plate.StampOchre();
        ui.ShowCompletion("FIRST IMPRESSION  ·  OCHRE");
        ui.SetObjective("Click the seal or press R to print again.");
    }

    // Full reload over a hand-written reset: fewer failure modes for a clean second run.
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
