namespace LiftControl.Config
{
    /// <summary>Centralized configuration for all adjustable simulation parameters.</summary>
    public static class SimulationConfig
    {
        /// <summary>
        /// Total number of floors in the building.
        /// </summary>
        public static int NumberOfFloors { get; set; } = 10;

        /// <summary>
        /// Number of lift units available in the system.
        /// </summary>
        public static int NumberOfLifts { get; set; } = 4;

        /// <summary>
        /// Time it takes for a lift to travel between two floors (in seconds).
        /// </summary>
        public static int SecondsPerFloor { get; set; } = 10;

        /// <summary>
        /// Time allowed for passengers to board or exit (in seconds).
        /// </summary>
        public static int SecondsForLoading { get; set; } = 10;

        /// <summary>
        /// How often new passenger requests are simulated (in seconds).
        /// </summary>
        public static int RequestIntervalSeconds { get; set; } = 30;

        /// <summary>
        /// Gets or sets the direction penalty weight.
        /// </summary>
        /// <value>
        /// The direction penalty weight.
        /// </value>
        public static int DirectionPenaltyWeight { get; set;} = 10;

        /// <summary>
        /// Gets or sets the batching bonus weight.
        /// </summary>
        /// <value>
        /// The batching bonus weight.
        /// </value>
        public static int BatchingBonusWeight { get; set; } = -5;

        /// <summary>
        /// Gets or sets the load penalty weight.
        /// </summary>
        /// <value>
        /// The load penalty weight.
        /// </value>
        public static int LoadPenaltyWeight { get; set; }

        /// <summary>
        /// Gets or sets the maximum lift load threshold.
        /// </summary>
        /// <value>
        /// The maximum lift load threshold.
        /// </value>
        public static int MaxLiftLoadThreshold { get; set; } = 6;
        /// <summary>
        /// Gets or sets the perfect match bonus weight.
        /// </summary>
        /// <value>
        /// The perfect match bonus weight.
        /// </value>
        public static int PerfectMatchBonusWeight { get; set; } = -15;
        /// <summary>
        /// Gets or sets the idle bonus weight.
        /// </summary>
        /// <value>
        /// The idle bonus weight.
        /// </value>
        public static int IdleBonusWeight { get; set; } = -10;
    }
}
