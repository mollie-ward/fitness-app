namespace FitnessApp.Domain.Enums;

/// <summary>
/// Type of training session
/// </summary>
public enum SessionType
{
    // Running session types
    /// <summary>
    /// Easy conversational pace run
    /// </summary>
    EasyRun = 0,

    /// <summary>
    /// Interval training session
    /// </summary>
    Intervals = 1,

    /// <summary>
    /// Tempo run at lactate threshold
    /// </summary>
    Tempo = 2,

    /// <summary>
    /// Long endurance run
    /// </summary>
    LongRun = 3,

    /// <summary>
    /// Low-intensity recovery run
    /// </summary>
    Recovery = 4,

    // Strength session types
    /// <summary>
    /// Full body strength workout
    /// </summary>
    FullBody = 5,

    /// <summary>
    /// Upper/Lower body split
    /// </summary>
    UpperLower = 6,

    /// <summary>
    /// Push/Pull/Legs split
    /// </summary>
    PushPullLegs = 7,

    // HYROX session types
    /// <summary>
    /// Full HYROX race simulation
    /// </summary>
    RaceSimulation = 8,

    /// <summary>
    /// Practice specific HYROX stations
    /// </summary>
    StationPractice = 9,

    /// <summary>
    /// Transition drills between stations
    /// </summary>
    TransitionDrills = 10,

    /// <summary>
    /// Hybrid conditioning work
    /// </summary>
    HybridConditioning = 11
}
