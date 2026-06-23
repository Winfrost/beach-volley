using UnityEngine;

namespace BeachVolley.Content
{
    /// <summary>
    /// Carries the active TournamentRun across scene loads (menu -> match -> match -> ...).
    /// Sibling of MatchSession: MatchSession transports one match, TournamentSession transports
    /// the whole run. Static, auto-cleared at the start of every Play session, so booting
    /// straight into Gameplay never has a stale tournament hanging around.
    /// </summary>
    public static class TournamentSession
    {
        public static TournamentRun Active { get; private set; }
        public static bool IsActive => Active != null;

        public static void Set(TournamentRun run) => Active = run;
        public static void Clear() => Active = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnPlay() => Active = null;
    }
}
