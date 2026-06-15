namespace BeachVolley.Gameplay
{
    /// <summary>
    /// Abstraction over a source of player intent. Implemented by keyboard, AI, and
    /// (later) touch. PlayerController drives movement from whatever source is attached,
    /// so the same physics/jump code serves human and CPU players alike.
    /// </summary>
    public interface IPlayerInput
    {
        /// <summary>Called once per frame by the controller (from Update) to refresh intent.</summary>
        void Tick();

        /// <summary>Horizontal intent in [-1, 1].</summary>
        float Horizontal { get; }

        /// <summary>True while the jump action is held (drives variable jump height).</summary>
        bool JumpHeld { get; }

        /// <summary>Returns true ONCE if a jump was requested since the last call, then clears it.</summary>
        bool ConsumeJumpPressed();
    }
}
