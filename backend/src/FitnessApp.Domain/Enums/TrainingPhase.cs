namespace FitnessApp.Domain.Enums;

/// <summary>
/// Training periodization phase
/// </summary>
public enum TrainingPhase
{
    /// <summary>
    /// Foundation phase - building base fitness
    /// </summary>
    Foundation = 0,

    /// <summary>
    /// Build phase - increasing volume and intensity
    /// </summary>
    Build = 1,

    /// <summary>
    /// Intensity phase - high-intensity work
    /// </summary>
    Intensity = 2,

    /// <summary>
    /// Peak phase - peak performance before event
    /// </summary>
    Peak = 3,

    /// <summary>
    /// Taper phase - reducing volume before event
    /// </summary>
    Taper = 4,

    /// <summary>
    /// Recovery phase - active recovery
    /// </summary>
    Recovery = 5
}
