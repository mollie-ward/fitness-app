namespace FitnessApp.Domain.Enums;

/// <summary>
/// Fundamental movement pattern
/// </summary>
public enum MovementPattern
{
    /// <summary>
    /// Pushing movements (e.g., bench press, push-ups)
    /// </summary>
    Push = 0,

    /// <summary>
    /// Pulling movements (e.g., rows, pull-ups)
    /// </summary>
    Pull = 1,

    /// <summary>
    /// Squatting movements (e.g., squats, lunges)
    /// </summary>
    Squat = 2,

    /// <summary>
    /// Hip hinge movements (e.g., deadlifts, RDLs)
    /// </summary>
    Hinge = 3,

    /// <summary>
    /// Carrying movements (e.g., farmer's carry)
    /// </summary>
    Carry = 4,

    /// <summary>
    /// Core stability and anti-rotation
    /// </summary>
    Core = 5,

    /// <summary>
    /// Cardiovascular endurance
    /// </summary>
    Cardio = 6
}
