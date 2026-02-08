namespace Core.Constants
{
    /// <summary>
    /// Game-wide constants to replace magic numbers
    /// </summary>
    public static class GameConstants
    {
        #region Physics Detection

        /// <summary>
        /// Default raycast distance for ground detection and tool targeting
        /// </summary>
        public const float DefaultRaycastDistance = 100f;

        /// <summary>
        /// Offset above ground when placing items
        /// </summary>
        public const float GroundOffset = 0.1f;

        /// <summary>
        /// Height offset for raycast origin when detecting ground
        /// </summary>
        public const float RaycastHeightOffset = 1f;

        #endregion

        #region Interaction

        /// <summary>
        /// Default distance for dropping items in front of player
        /// </summary>
        public const float DefaultDropDistance = 2f;

        /// <summary>
        /// Default cooldown time between interactions
        /// </summary>
        public const float DefaultCooldownTime = 0.1f;

        /// <summary>
        /// Default radius for item detection
        /// </summary>
        public const float DefaultDetectionRadius = 2f;

        #endregion

        #region Inventory

        /// <summary>
        /// Default number of inventory slots
        /// </summary>
        public const int DefaultMaxSlots = 10;

        /// <summary>
        /// Default max stack size for stackable items
        /// </summary>
        public const int DefaultMaxStack = 99;

        /// <summary>
        /// Max stack size for tools (non-stackable)
        /// </summary>
        public const int ToolMaxStack = 1;

        #endregion
    }
}
