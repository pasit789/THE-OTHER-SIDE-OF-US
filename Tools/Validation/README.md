# Hallway puzzle validation

`VerifyHallwayPuzzle.cs` is an Editor-only Play Mode driver kept outside Assets so it does not add test tooling to normal project imports or builds.

To rerun, create an isolated Unity project copy with this project's Assets, Packages and ProjectSettings. Copy this driver into that copy's Assets/Editor directory. Use Unity 6000.6.5f1 with `-batchmode -nographics -projectPath <copy> -executeMethod VerifyHallwayPuzzle.Run -logFile <log>`. Do not add `-quit`: the driver enters Play Mode and calls EditorApplication.Exit with 0 on success or 1 on failure. Never run this driver against an Editor session containing unsaved work.

The test covers the authored hallway puzzle, collision safety, scene travel and session persistence. It aims the third-person interaction ray at real scene colliders; it invokes gameplay methods for puzzle inputs and the real door/fade entry points for travel. It does not emulate physical keyboard events or verify rendered text, sound or visual appearance. `hallway-puzzle-results.txt` records the last passing run (47 assertions).
