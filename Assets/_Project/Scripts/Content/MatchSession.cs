using UnityEngine;

namespace BeachVolley.Content
{
    /// <summary>
    /// Carries a pending MatchConfig from the scene that sets it up (the menu, later a
    /// tournament) into the Gameplay scene that consumes it. This is the swappable
    /// "transport" — the one deliberate seam for passing match data across a scene load.
    ///
    /// Static on purpose: minimal, no GameObject, no DontDestroyOnLoad. It auto-clears at
    /// the start of every Play session (see ResetOnPlay), so booting straight into Gameplay
    /// always starts with NO pending config -> the Bootstrap falls back to its own fields.
    /// That keeps the "boot directly into Gameplay to test" workflow intact.
    /// </summary>
    public static class MatchSession
    {
        public static MatchConfig Pending { get; private set; }
        public static bool HasPending => Pending != null;

        public static void Set(MatchConfig config) => Pending = config;
        public static void Clear() => Pending = null;

        // Runs before the first scene loads, on every Play session, regardless of the
        // editor's "Reload Domain" setting. Guarantees a fresh, empty session each run.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnPlay() => Pending = null;
    }
}
