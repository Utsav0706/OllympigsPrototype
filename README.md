# Ollympigs — The Crossing (one-day prototype)

Ollypig Swine crosses a rain-soaked Shibuya scramble, finds a note on a shop front, and reaches a
winners' podium — where the blue woodblock proof finally takes its first colour: a single Ochre impression.

## Run it

- **Unity 6000.0.84f1** (Unity 6), Universal Render Pipeline 2D.
- Open **`Assets/Scenes/OllympigsStage.unity`** and press **Play**. It is also scene 0 in Build Settings.
- Best viewed with the Game view at **16:9 / 1920 × 1080** (the framing and the print offset are tuned for it).
- Active Input Handling is set to **Both**; no extra setup is needed.

## Controls

| Key | Action |
|---|---|
| **W A S D** / arrow keys | Move |
| **E** | Read the note (when the red **E** prompt shows) |
| **R** or click the red **seal** (top right) | Restart the stage |

## What is implemented

Tokyo, five days before the Games: the cartouche reads **OLLYPIG SWINE · TOKYO · 5 DAYS TO THE GAMES**
throughout, and each step's instruction appears in a scroll beneath it.

Three sequential comic panels, cut between with hard camera cuts:

1. **01 · The Crossing** — a top-down Shibuya scramble. The umbrella crowd walls off every line except the
   lower-left → upper-right arm of the X, so a first-time player naturally walks the diagonal.
2. **02 · The Clue** — a shop front. The exit is locked until Ollypig reads the note (**E**); a poem scroll
   opens and the gate unlocks.
3. **03 · Arrival** — stepping onto the winners' podium stamps the **Ochre** layer once, abruptly and slightly
   out of register. She freezes, a plate caption appears, and **R** / the seal starts a clean second run.

State flow: `Crossing → Clue → Arrival → Printed`. Restart returns everything to the blue Aizuri base.

## Implementation choices

- **Layered sprites, no custom shaders.** Every panel is a stack with fixed sorting orders:

  | Order | Layer |
  |---|---|
  | 0 | Washi paper |
  | 10–12 | Aizuri base (road, skyline, pavements, crossing, shop, podium) |
  | 20–21 | Ochre twins — renderers **off** until the stamp |
  | 30–35 | Sumi keylines (umbrellas, linework, pedestrians) and the print frame |
  | 49–50 | Ollypig's shadow and Ollypig |
  | 55 / 60 / 100 | rain / interact prompt / paper grain |

- **Keylines print above the Ochre.** Linework for the podium and shop is extracted into its own sprites on
  the keyline layer, so the registration offset can never cover a line (verified: 100 % of keyline pixels
  survive a 0–2 px shift).
- **The print is an abrupt renderer enable** — offset `OchreRoot`, then enable every Ochre renderer in the
  same frame. No fade, lerp or tween anywhere. In that same frame the paper grain drops from 10 % to 4 %, so
  the rough proof gives way to a crisper finished plate.
- **Registration offset (0.02, −0.02)** — about 2 screen pixels at 1080p. Tested against 0.015–0.03:
  below 0.02 it vanishes in places, above 0.025 it reads as a drop shadow. Read as a **permanent
  misregistration** by default; untick `Retain Offset` on the PrintPlate for an offset that snaps into
  register after a beat. Right-click the PrintPlate component in Play mode for *Stamp now* / *Reset to blue*.
- **Single-fire guards.** `hasPrinted` in `OnArrivalReached`, `used` + `clueFound` for the clue, input frozen
  on print. The exit is locked twice: the blocker collider and a `clueFound` check in `OnPanelExit`.
- **Restart reloads the scene.** A hand-written reset would be tidier but has more failure modes, and a clean
  second run is an acceptance criterion. The only static (`StageController.Instance`) is reload-safe.
- **Colour is earned.** Before the clear, colour appears only on Ollypig (Safflower) and on active information
  (Beni Vermillion: the interact prompt and the restart seal). Ochre appears only on the stamp and its caption chip.
- **Closed palette, one gradient, no drop shadows.** Eight hexes (Moss Green reserved, unused). The only
  gradient is the bokashi at the sky edge; post-processing is off, and nothing casts a shadow.
- **Print UI.** Name in a cartouche, instructions and the poem in scrolls with rolled ends, restart as an
  outlined seal, and the completion label set as a plate caption in the paper margin.
- **Rain** is a quad with a URP Unlit material scrolled by `mainTextureOffset`, because URP 2D sprite shaders
  ignore texture offset. Two out-of-step gusts keep it from looking mechanical.
- **Scripts** (`Assets/Scripts`): `StageController` (state, gating, single-fire), `PlayerController2D`,
  `PrintPlate` (the stamp), `ClueInteractable`, `PanelTrigger`, `StageUI` (display only), `CameraRig`
  (hard cuts), `RainScroller`.

### Assumptions

- **Art Direction Bible v1 was not available.** Everything is built to the palette and composition rules in
  the brief.
- **The crossing is a square scramble with a 45° X** (following a Shibuya reference) rather than a single
  crossing at ~25°; the diagonal arm is still the organising line of panel 1.
- **Ollypig is drawn in flat side view** in every panel, including the top-down crossing, as the brief asks.

## Time spent

About **7 h 10 min** by commit history (first commit 08:28, last 15:39 on 25 Sep 2026), against a 12-hour target.

## Known issues

- Crossing panel 1 takes about **5 seconds** at 3.5 units/s, below the 8–12 s guide; slowing Ollypig made
  her feel sluggish, so the length was left to the layout.
- In panels 2 and 3 Ollypig can walk up over the skyline and shop art — the walls are at the panel edge,
  not at the pavement line.
- Side-view pedestrians in panels 2 and 3 are decoration only and have no collision.
- UI text uses Unity's built-in font (TextMesh Pro essentials were not imported).
- The restart seal's click area is its square bounds, so the transparent corners also click.
- The registration offset is tuned for 1080p; at much lower resolutions the misprint becomes very thin.
- Movement uses the legacy Input Manager; gamepad works only through its default axes.

## Next step with another day

- The **Silver tier**: a second Moss Green impression on a repeat clear, reusing the PrintPlate pattern.
- A two-frame walk cycle for Ollypig and a characterful open-licence display font for the cartouche.

## Recording

60–90 s capture of one complete run plus a restart: *add link here.*

## Credits

- **Original work:** all game code, and all art — paper and grain, scramble crossing, pavements, umbrellas,
  pedestrians, city skyline and bokashi, street markings, shop front, note, podium, Ochre and keyline layers,
  Ollypig, print frames, UI plates, scrolls, seal and rain — created for this project, generated
  procedurally from the closed palette.
- **Placeholders:** all of the art above is **placeholder quality**, built to the palette and composition
  rules to prove the pipeline, not final illustration. Ollypig in particular is a rough stand-in for the
  character in the Art Direction Bible. `Assets/Art/Placeholder_Square.png` is the grey-box square and is
  used only to tint the flat road and street blocks.
- **Reused / licensed:** Unity 6 engine and packages (Universal RP, Input System, uGUI) under the Unity
  licence; UI text uses Unity's built-in LegacyRuntime font. No third-party art, audio or fonts.
- **AI assistance:** Anthropic's Claude (Claude Code) was used throughout: writing and reviewing the C#
  scripts, generating the art assets from code, editing scene and asset files, testing by rendering and code
  tracing, and drafting this README. Design direction, references and final decisions were mine.
