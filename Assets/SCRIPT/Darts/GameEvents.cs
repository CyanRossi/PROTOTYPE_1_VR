using System;
using UnityEngine;

public enum DartRing
{
    Miss, InnerBull, OuterBull, Single, Double, Triple
}

public static class GameEvents
{
    // Fired when a dart is released
    public static Action<Dart> OnDartThrown;

    // Fired when a dart sticks the board and is scored
    // score = final points added, ring = which ring, sector = 1..20 (0 for bulls), worldPoint = hit pos
    public static Action<int, DartRing, int, Vector3> OnDartScored;

    // Fired after all three darts are stuck and cleared, and a new rack appears
    public static Action OnRackRefilled;

    // UI helpers
    public static Action<int> OnTotalScoreChanged;
    public static Action<int> OnDartsRemainingChanged;
    public static Action<string> OnLastHitText;

    // Pause/settings toggles (optional)
    public static Action<bool> OnPauseToggled;
}
