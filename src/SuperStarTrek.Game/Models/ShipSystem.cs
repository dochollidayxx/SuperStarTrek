namespace SuperStarTrek.Game.Models
{
    /// <summary>
    /// Represents the 8 ship systems that can be damaged in Super Star Trek.
    /// Matches the original BASIC implementation's D(8) array.
    /// </summary>
    public enum ShipSystem
    {
        /// <summary>
        /// Warp Engines - System 1 in original BASIC
        /// </summary>
        WarpEngines = 1,

        /// <summary>
        /// Short Range Sensors - System 2 in original BASIC
        /// </summary>
        ShortRangeSensors = 2,

        /// <summary>
        /// Long Range Sensors - System 3 in original BASIC
        /// </summary>
        LongRangeSensors = 3,

        /// <summary>
        /// Phaser Control - System 4 in original BASIC
        /// </summary>
        PhaserControl = 4,

        /// <summary>
        /// Photon Tubes - System 5 in original BASIC
        /// </summary>
        PhotonTubes = 5,

        /// <summary>
        /// Damage Control - System 6 in original BASIC
        /// </summary>
        DamageControl = 6,

        /// <summary>
        /// Shield Control - System 7 in original BASIC
        /// </summary>
        ShieldControl = 7,

        /// <summary>
        /// Library Computer - System 8 in original BASIC
        /// </summary>
        LibraryComputer = 8
    }

    /// <summary>
    /// Extensions for the ShipSystem enum to provide game-specific functionality
    /// </summary>
    public static class ShipSystemExtensions
    {
        /// <summary>
        /// Gets the display name for a ship system as shown in the original game
        /// </summary>
        public static string GetDisplayName(this ShipSystem system)
        {
            return system switch
            {
                ShipSystem.WarpEngines => "WARP ENGINES",
                ShipSystem.ShortRangeSensors => "SHORT RANGE SENSORS",
                ShipSystem.LongRangeSensors => "LONG RANGE SENSORS",
                ShipSystem.PhaserControl => "PHASER CONTROL",
                ShipSystem.PhotonTubes => "PHOTON TUBES",
                ShipSystem.DamageControl => "DAMAGE CONTROL",
                ShipSystem.ShieldControl => "SHIELD CONTROL",
                ShipSystem.LibraryComputer => "LIBRARY-COMPUTER",
                _ => throw new ArgumentOutOfRangeException(nameof(system))
            };
        }

        /// <summary>
        /// Gets all ship systems in the order they appear in the original game
        /// </summary>
        public static IEnumerable<ShipSystem> GetAllSystems()
        {
            return Enum.GetValues<ShipSystem>().OrderBy(s => (int)s);
        }
    }
}
