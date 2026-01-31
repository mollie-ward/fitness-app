using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;
using FitnessApp.Infrastructure.Persistence;

namespace FitnessApp.Infrastructure.Persistence.SeedData;

/// <summary>
/// Provides comprehensive seed data for exercises and related entities
/// </summary>
public static class ExerciseSeedData
{
    public static async Task SeedAllAsync(ApplicationDbContext context)
    {
        // Seed tags first
        await SeedMuscleGroupsAsync(context);
        await SeedEquipmentAsync(context);
        await SeedContraindicationsAsync(context);
        
        // Then exercises
        await SeedExercisesAsync(context);
    }

    private static async Task SeedMuscleGroupsAsync(ApplicationDbContext context)
    {
        var muscleGroups = new[]
        {
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000001"), Name = "Quadriceps", Category = "Lower Body", Description = "Front thigh muscles", CreatedAt = DateTime.UtcNow },
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000002"), Name = "Hamstrings", Category = "Lower Body", Description = "Back thigh muscles", CreatedAt = DateTime.UtcNow },
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000003"), Name = "Glutes", Category = "Lower Body", Description = "Gluteal muscles", CreatedAt = DateTime.UtcNow },
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000004"), Name = "Calves", Category = "Lower Body", Description = "Calf muscles", CreatedAt = DateTime.UtcNow },
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000005"), Name = "Chest", Category = "Upper Body", Description = "Pectoral muscles", CreatedAt = DateTime.UtcNow },
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000006"), Name = "Back", Category = "Upper Body", Description = "Latissimus dorsi and upper back", CreatedAt = DateTime.UtcNow },
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000007"), Name = "Shoulders", Category = "Upper Body", Description = "Deltoid muscles", CreatedAt = DateTime.UtcNow },
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000008"), Name = "Biceps", Category = "Upper Body", Description = "Front arm muscles", CreatedAt = DateTime.UtcNow },
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000009"), Name = "Triceps", Category = "Upper Body", Description = "Back arm muscles", CreatedAt = DateTime.UtcNow },
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000010"), Name = "Core", Category = "Core", Description = "Abdominal and oblique muscles", CreatedAt = DateTime.UtcNow },
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000011"), Name = "Lower Back", Category = "Core", Description = "Erector spinae", CreatedAt = DateTime.UtcNow },
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000012"), Name = "Full Body", Category = "Full Body", Description = "Multiple muscle groups", CreatedAt = DateTime.UtcNow },
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000013"), Name = "Forearms", Category = "Upper Body", Description = "Forearm muscles and grip", CreatedAt = DateTime.UtcNow },
            new MuscleGroup { Id = new Guid("10000000-0000-0000-0000-000000000014"), Name = "Hip Flexors", Category = "Lower Body", Description = "Hip flexor muscles", CreatedAt = DateTime.UtcNow }
        };
        await context.MuscleGroups.AddRangeAsync(muscleGroups);
    }

    private static async Task SeedEquipmentAsync(ApplicationDbContext context)
    {
        var equipment = new[]
        {
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000001"), Name = "Bodyweight", Description = "No equipment required", CreatedAt = DateTime.UtcNow },
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000002"), Name = "Barbell", Description = "Standard barbell", CreatedAt = DateTime.UtcNow },
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000003"), Name = "Dumbbells", Description = "Pair of dumbbells", CreatedAt = DateTime.UtcNow },
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000004"), Name = "Kettlebell", Description = "Kettlebell", CreatedAt = DateTime.UtcNow },
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000005"), Name = "Ski Erg", Description = "Ski ergometer machine", CreatedAt = DateTime.UtcNow },
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000006"), Name = "Rowing Machine", Description = "Indoor rowing machine", CreatedAt = DateTime.UtcNow },
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000007"), Name = "Sled", Description = "Weighted sled for pushing/pulling", CreatedAt = DateTime.UtcNow },
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000008"), Name = "Sandbag", Description = "Training sandbag", CreatedAt = DateTime.UtcNow },
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000009"), Name = "Wall Ball", Description = "Medicine ball for wall throws", CreatedAt = DateTime.UtcNow },
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000010"), Name = "Pull-up Bar", Description = "Pull-up or chin-up bar", CreatedAt = DateTime.UtcNow },
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000011"), Name = "Bench", Description = "Weight bench", CreatedAt = DateTime.UtcNow },
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000012"), Name = "Squat Rack", Description = "Squat or power rack", CreatedAt = DateTime.UtcNow },
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000013"), Name = "Resistance Bands", Description = "Elastic resistance bands", CreatedAt = DateTime.UtcNow },
            new Equipment { Id = new Guid("20000000-0000-0000-0000-000000000014"), Name = "Track", Description = "Running track or measured course", CreatedAt = DateTime.UtcNow }
        };
        await context.Equipment.AddRangeAsync(equipment);
    }

    private static async Task SeedContraindicationsAsync(ApplicationDbContext context)
    {
        var contraindications = new[]
        {
            new Contraindication { Id = new Guid("30000000-0000-0000-0000-000000000001"), InjuryType = "Shoulder", MovementRestriction = "Overhead", Description = "Shoulder injuries limiting overhead movement", CreatedAt = DateTime.UtcNow },
            new Contraindication { Id = new Guid("30000000-0000-0000-0000-000000000002"), InjuryType = "Knee", MovementRestriction = "Impact", Description = "Knee injuries sensitive to impact", CreatedAt = DateTime.UtcNow },
            new Contraindication { Id = new Guid("30000000-0000-0000-0000-000000000003"), InjuryType = "Lower Back", MovementRestriction = "Heavy Load", Description = "Lower back issues limiting heavy loading", CreatedAt = DateTime.UtcNow },
            new Contraindication { Id = new Guid("30000000-0000-0000-0000-000000000004"), InjuryType = "Ankle", MovementRestriction = "Jumping", Description = "Ankle injuries preventing jumping movements", CreatedAt = DateTime.UtcNow },
            new Contraindication { Id = new Guid("30000000-0000-0000-0000-000000000005"), InjuryType = "Wrist", MovementRestriction = "Weight Bearing", Description = "Wrist injuries limiting weight bearing", CreatedAt = DateTime.UtcNow }
        };
        await context.Contraindications.AddRangeAsync(contraindications);
    }

    private static async Task SeedExercisesAsync(ApplicationDbContext context)
    {
        var exercises = new List<Exercise>();
        
        // HYROX Exercises (30+)
        exercises.AddRange(GetHyroxExercises());
        
        // Running Exercises (25+)
        exercises.AddRange(GetRunningExercises());
        
        // Strength Exercises (50+)
        exercises.AddRange(GetStrengthExercises());
        
        await context.Exercises.AddRangeAsync(exercises);
    }

    private static List<Exercise> GetHyroxExercises()
    {
        return new List<Exercise>
        {
            new Exercise { 
                Id = new Guid("e6f2349c-7594-4607-b487-bd2f67039889"),
                Name = "Ski Erg Steady State",
                Description = "Maintain consistent pace on ski erg",
                Instructions = "Keep steady rhythm, engage core and lats",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 20,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("12b6fcb3-fd23-4e9c-b846-ec76c015be58"),
                Name = "Ski Erg Intervals 30/30",
                Description = "30 seconds hard, 30 seconds easy intervals",
                Instructions = "Explosive pull, controlled recovery",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 15,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("53fb1a83-7362-4e59-bfba-6d9e8abe1847"),
                Name = "Ski Erg Sprint Intervals",
                Description = "High intensity sprint intervals",
                Instructions = "Maximum power output for short bursts",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Maximum,
                ApproximateDuration = 12,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("eecd85f6-2409-45ae-ab86-c66f23405e15"),
                Name = "Ski Erg 1000m Time Trial",
                Description = "1000m for time",
                Instructions = "Consistent pacing, strong finish",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 5,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("d416029a-2040-48ca-b327-c07c16d18768"),
                Name = "Ski Erg Pyramid",
                Description = "Increasing then decreasing intervals",
                Instructions = "Build intensity then taper",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 20,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("889ddcff-ea4e-41d8-82f4-5895d9856bdc"),
                Name = "Ski Erg Distance Challenge",
                Description = "Maximum distance in 10 minutes",
                Instructions = "Sustainable pace throughout",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 10,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("a5e7cc33-8fb1-4455-9a31-cb927fb23f80"),
                Name = "Sled Push",
                Description = "Push weighted sled",
                Instructions = "Low position, drive with legs",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 5,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("77f01e5e-e779-4a7a-8566-c32de8d5a451"),
                Name = "Sled Pull",
                Description = "Pull weighted sled backwards",
                Instructions = "Control movement, engage posterior chain",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 5,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("a3ded17b-a95a-456f-aee3-fd0a1a56a249"),
                Name = "Sled Push-Pull Combo",
                Description = "Alternate push and pull",
                Instructions = "Quick transitions",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 8,
                SessionType = SessionType.TransitionDrills,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("59b2a69a-efe4-4398-bae8-d38bca270587"),
                Name = "Heavy Sled Push",
                Description = "Maximum weight sled push",
                Instructions = "Power and strength focus",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Maximum,
                ApproximateDuration = 3,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("9372fa76-4a12-42e3-8bcd-0b5a910bcc4e"),
                Name = "Sled Sprint",
                Description = "Short distance max speed",
                Instructions = "Explosive power",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Maximum,
                ApproximateDuration = 2,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("2f22c7a8-22f6-4073-9cee-6eb2c9c4fe84"),
                Name = "Burpee Broad Jump",
                Description = "Burpee with broad jump",
                Instructions = "Explosive jump forward, controlled landing",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 8,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("91ec437c-c8af-447d-a7c3-3e0b7b77fab8"),
                Name = "Fast Burpee Broad Jumps",
                Description = "Speed-focused burpee broad jumps",
                Instructions = "Minimize ground contact time",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 6,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("bb95e9c4-c837-4da7-8018-5039a0b31ea9"),
                Name = "Burpee Broad Jump Ladder",
                Description = "Increasing reps ladder",
                Instructions = "Pace management critical",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Maximum,
                ApproximateDuration = 12,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("b1bf7dad-17e6-4303-bd0f-2acfe2e42560"),
                Name = "Burpee Efficiency Drill",
                Description = "Perfect form burpees",
                Instructions = "Quality movement practice",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 10,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("df59ddd1-63bb-43de-bbe8-1f876f46d67b"),
                Name = "Rowing Steady State",
                Description = "Consistent pace rowing",
                Instructions = "Maintain stroke rate and power",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 20,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("5ac8c048-9573-44d8-876d-9f73916b4590"),
                Name = "Rowing Intervals 500m",
                Description = "500m intervals with rest",
                Instructions = "High intensity efforts",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 15,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("f54b45e6-bdf6-4fdf-ad88-7292586b212e"),
                Name = "Rowing Sprint Intervals",
                Description = "Short max effort intervals",
                Instructions = "Maximum power output",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Maximum,
                ApproximateDuration = 12,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("0420236e-87c2-434e-bd8c-268fb07b958d"),
                Name = "Rowing 1000m Time Trial",
                Description = "1000m for time",
                Instructions = "Pacing and endurance",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 5,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("a9d0fb8c-5702-4a63-ba9b-4e17f3882a47"),
                Name = "Rowing Distance Challenge",
                Description = "Maximum meters in 15 minutes",
                Instructions = "Sustainable effort",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 15,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("694e51cf-662b-42cc-ad50-b9bf35b592e8"),
                Name = "Farmers Carry",
                Description = "Carry heavy dumbbells/kettlebells",
                Instructions = "Tight core, upright posture",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 8,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("897eb9bf-6ae4-4d71-b04d-38ba5c9b60ef"),
                Name = "Heavy Farmers Carry",
                Description = "Maximum weight carry",
                Instructions = "Grip and core strength",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 5,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("66c2be9a-374d-44a3-9c02-5047f900ab4f"),
                Name = "Farmers Carry Intervals",
                Description = "Timed intervals with rest",
                Instructions = "Controlled movement",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 12,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("977f019e-4511-4e18-8bb8-dfb7b1d3a386"),
                Name = "Uneven Farmers Carry",
                Description = "Different weights each hand",
                Instructions = "Anti-rotation core work",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 8,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("b0f6f763-43cf-4343-b034-da0b334d3de4"),
                Name = "Sandbag Lunges",
                Description = "Walking lunges with sandbag",
                Instructions = "Control descent, drive through heel",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 10,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("1d5e3ce4-d1e4-4b5f-9fa7-56304ef08e06"),
                Name = "Heavy Sandbag Lunges",
                Description = "Maximum weight lunges",
                Instructions = "Strength and stability",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 8,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("c1e3f35b-aff3-4b12-a936-62d3cc9cf66a"),
                Name = "Sandbag Lunge Intervals",
                Description = "Timed lunge intervals",
                Instructions = "Pace management",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 12,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("b12ad8f8-a040-4749-92dd-670e9186fde1"),
                Name = "Wall Balls",
                Description = "Squat and throw ball to target",
                Instructions = "Full depth squat, explosive throw",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 10,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("920dc092-d367-4191-9176-8e1d58bda088"),
                Name = "Wall Ball Intervals",
                Description = "Timed wall ball intervals",
                Instructions = "Consistent rhythm",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 12,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("021f59ed-1f5d-4f04-9f73-332c88d0af9e"),
                Name = "Heavy Wall Balls",
                Description = "Increased weight wall balls",
                Instructions = "Power and endurance",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 8,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("ac9d0489-810d-45b5-8e03-7b2ee594dd0e"),
                Name = "HYROX Race Simulation",
                Description = "Full race simulation workout",
                Instructions = "Practice pacing and transitions",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Maximum,
                ApproximateDuration = 90,
                SessionType = SessionType.RaceSimulation,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("52d5f83e-f7d1-4009-81af-5272bc220eea"),
                Name = "HYROX Half Simulation",
                Description = "Half race simulation",
                Instructions = "Build race readiness",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 50,
                SessionType = SessionType.RaceSimulation,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("d806d841-e9b1-4b7e-ac72-91d5a0c0978c"),
                Name = "Station Transitions",
                Description = "Practice moving between stations",
                Instructions = "Quick equipment setup and breakdown",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Low,
                ApproximateDuration = 15,
                SessionType = SessionType.TransitionDrills,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("418e4625-65f2-4600-ae18-0fd17240011f"),
                Name = "Hybrid Conditioning",
                Description = "Mix of cardio and strength",
                Instructions = "Builds hybrid fitness",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 40,
                SessionType = SessionType.HybridConditioning,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("a00a2eb7-1760-4c10-80e7-4a05c9bf8bbf"),
                Name = "HYROX Station Circuit",
                Description = "Circuit through all 8 stations",
                Instructions = "Build familiarity with movements",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 45,
                SessionType = SessionType.StationPractice,
                CreatedAt = DateTime.UtcNow
            }
        };
    }

    private static List<Exercise> GetRunningExercises()
    {
        return new List<Exercise>
        {
            new Exercise { 
                Id = new Guid("163d84a8-badb-4041-a2e8-bde0448bd1df"),
                Name = "Easy Run 30min",
                Description = "Easy conversational pace for 30 minutes",
                Instructions = "Maintain comfortable effort, focus on form",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Low,
                ApproximateDuration = 30,
                SessionType = SessionType.EasyRun,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("c5e13d54-1a28-4b04-805f-76ba2b5572bd"),
                Name = "Easy Run 45min",
                Description = "Easy conversational pace for 45 minutes",
                Instructions = "Build aerobic base",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Low,
                ApproximateDuration = 45,
                SessionType = SessionType.EasyRun,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("d900b895-1976-44dc-b9d2-a1e27a1a5455"),
                Name = "Easy Run 60min",
                Description = "Easy conversational pace for 60 minutes",
                Instructions = "Long easy effort",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Low,
                ApproximateDuration = 60,
                SessionType = SessionType.EasyRun,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("4bfdc9dd-4284-417b-a667-51e3edc57b48"),
                Name = "Easy Run 75min",
                Description = "Easy conversational pace for 75 minutes",
                Instructions = "Extended aerobic work",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Low,
                ApproximateDuration = 75,
                SessionType = SessionType.EasyRun,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("c9fd635c-d3f1-4dae-8c2d-270a102e133f"),
                Name = "Easy Run 90min",
                Description = "Easy conversational pace for 90 minutes",
                Instructions = "Long aerobic endurance",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Low,
                ApproximateDuration = 90,
                SessionType = SessionType.EasyRun,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("9ca98ca8-aca0-4821-82d5-8beddec626e8"),
                Name = "400m Intervals",
                Description = "8x400m at 5K pace",
                Instructions = "Fast 400s with equal recovery jog",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 40,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("23920f47-1ffe-4428-93ed-e321410b7533"),
                Name = "800m Intervals",
                Description = "6x800m at 5K pace",
                Instructions = "Controlled hard efforts",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 45,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("d1ca2206-bacc-4227-b730-ca6490ef1b01"),
                Name = "1600m Intervals",
                Description = "4x1600m at 10K pace",
                Instructions = "Sustained threshold efforts",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 50,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("cd718487-4a79-4e71-9090-956fdc5e4da2"),
                Name = "200m Speed Intervals",
                Description = "12x200m sprints",
                Instructions = "Maximum speed work",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Maximum,
                ApproximateDuration = 35,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("af80ce83-6178-44da-863f-d590a77bd892"),
                Name = "1000m Intervals",
                Description = "5x1000m at threshold",
                Instructions = "Race pace practice",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 45,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("59e41080-8211-48f8-a2cc-f3d47189ff71"),
                Name = "Mixed Distance Intervals",
                Description = "Ladder: 400-800-1200-800-400",
                Instructions = "Varied interval training",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 50,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("a3a64fa9-bc60-459f-a09e-3fa4013344cb"),
                Name = "Pyramid Intervals",
                Description = "200-400-600-800-600-400-200",
                Instructions = "Build and descend",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 55,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("b37fd283-0811-41ff-95b0-c40d3421a018"),
                Name = "VO2 Max Intervals",
                Description = "5x3min at VO2 max effort",
                Instructions = "Maximum aerobic capacity",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Maximum,
                ApproximateDuration = 40,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("7bc0db6b-455d-4558-8551-5aa246574fe5"),
                Name = "600m Repeats",
                Description = "8x600m at 5K pace",
                Instructions = "Mid-distance speed work",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 42,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("52082dd5-5707-4912-9e88-acb587b82714"),
                Name = "Tempo Run 20min",
                Description = "20 minutes at lactate threshold",
                Instructions = "Comfortably hard pace",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 30,
                SessionType = SessionType.Tempo,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("1cda4de1-5756-4e58-b845-c25408b8c9ae"),
                Name = "Tempo Run 30min",
                Description = "30 minutes at lactate threshold",
                Instructions = "Sustained threshold effort",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 40,
                SessionType = SessionType.Tempo,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("a95017fa-2b1c-4bb7-b67e-96eb943d58ec"),
                Name = "Tempo Run 40min",
                Description = "40 minutes at lactate threshold",
                Instructions = "Extended threshold work",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 50,
                SessionType = SessionType.Tempo,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("e8e13e2e-dafd-4e99-89dc-ec934559bb01"),
                Name = "Cruise Intervals",
                Description = "3x10min at tempo with 2min rest",
                Instructions = "Broken tempo run",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 40,
                SessionType = SessionType.Tempo,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("3c2a29b9-f20b-4ba8-b9f6-68239466d6cd"),
                Name = "Long Run 90min",
                Description = "90 minute long run",
                Instructions = "Build endurance at easy pace",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Low,
                ApproximateDuration = 90,
                SessionType = SessionType.LongRun,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("2d09cd21-5fb0-4405-bdbf-818af6a96755"),
                Name = "Long Run 120min",
                Description = "2 hour long run",
                Instructions = "Extended endurance training",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Low,
                ApproximateDuration = 120,
                SessionType = SessionType.LongRun,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("b2f576ae-6b19-4758-9e8d-ae1d23f768b0"),
                Name = "Progressive Long Run",
                Description = "Start easy, finish at marathon pace",
                Instructions = "Build strength over distance",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 105,
                SessionType = SessionType.LongRun,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("de6486f0-0632-49e8-880f-658d068b288d"),
                Name = "Fartlek 40min",
                Description = "40 minutes with varied pace surges",
                Instructions = "Unstructured speed play",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 40,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("f4a656c2-c633-42a0-8924-922b15df71ad"),
                Name = "Hill Fartlek",
                Description = "Fartlek on hilly terrain",
                Instructions = "Surges on hills and flats",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 45,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("4ecfc5d9-d2f4-47c3-a7dc-bbec5de2307f"),
                Name = "Kenyan Fartlek",
                Description = "Structured fartlek with pattern",
                Instructions = "Alternating hard and moderate surges",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 35,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("65db989a-2a71-40ef-81d4-bc67e5842e99"),
                Name = "Hill Repeats Short",
                Description = "8x60sec uphill hard",
                Instructions = "Power and strength",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 35,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("87bc87c8-8684-4ce0-86ed-9febacb01e31"),
                Name = "Hill Repeats Long",
                Description = "6x3min uphill",
                Instructions = "Hill endurance",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 45,
                SessionType = SessionType.Intervals,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("219ff9bd-6bfa-4628-a708-f7bde7a59cfa"),
                Name = "Recovery Run 30min",
                Description = "Very easy 30 minute jog",
                Instructions = "Active recovery",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Low,
                ApproximateDuration = 30,
                SessionType = SessionType.Recovery,
                CreatedAt = DateTime.UtcNow
            }
        };
    }

    private static List<Exercise> GetStrengthExercises()
    {
        return new List<Exercise>
        {
            new Exercise { 
                Id = new Guid("2ded1f41-fc73-4e87-b02f-cdc61ec9f825"),
                Name = "Back Squat",
                Description = "Barbell back squat",
                Instructions = "Bar on upper back, squat to depth, drive through heels",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 30,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("88dbdf51-49e7-4fd6-aec8-6f34d134e748"),
                Name = "Front Squat",
                Description = "Barbell front squat",
                Instructions = "Bar on front delts, upright torso, squat to depth",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 30,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("61c9e638-0007-4afc-8ffa-90092e70f797"),
                Name = "Goblet Squat",
                Description = "Kettlebell goblet squat",
                Instructions = "Hold weight at chest, squat to depth",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 20,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("f9fb22a8-ea0e-45f0-a2b0-8e439070e6c5"),
                Name = "Box Squat",
                Description = "Squat to box",
                Instructions = "Control descent to box, pause, explode up",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 25,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("4d0fcb5d-8c5e-4875-8ff0-b94e96d5ed0c"),
                Name = "Overhead Squat",
                Description = "Squat with barbell overhead",
                Instructions = "Full body mobility and strength",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 30,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("c06fd54f-5a54-40a6-b334-cd9bdf529f84"),
                Name = "Bulgarian Split Squat",
                Description = "Single leg squat with rear foot elevated",
                Instructions = "Unilateral leg strength",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 25,
                SessionType = SessionType.UpperLower,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("60c5ad26-4513-492b-a93b-cc7960dc5f66"),
                Name = "Pistol Squat",
                Description = "Single leg bodyweight squat",
                Instructions = "Balance and unilateral strength",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 20,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("d3a8b87d-0e1a-4241-bff7-9455d699676d"),
                Name = "Pause Squat",
                Description = "Squat with pause at bottom",
                Instructions = "Build strength in bottom position",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 30,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("be4c6aca-b5cf-430e-b1c1-40288ea757ab"),
                Name = "Deadlift",
                Description = "Conventional barbell deadlift",
                Instructions = "Hip hinge, neutral spine, drive through ground",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 30,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("af49a492-73d6-410d-b85e-6cc986fef7b9"),
                Name = "Sumo Deadlift",
                Description = "Wide stance deadlift",
                Instructions = "Wide stance, upright torso",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 30,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("fbe4c16f-b8c6-48cd-86ac-59045f25d085"),
                Name = "Romanian Deadlift",
                Description = "RDL hamstring focused",
                Instructions = "Slight knee bend, push hips back",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 25,
                SessionType = SessionType.UpperLower,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("f8b785d2-088b-4b1f-990e-b6f1186667f6"),
                Name = "Trap Bar Deadlift",
                Description = "Deadlift with trap bar",
                Instructions = "Neutral grip, leg drive",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 25,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("f6f9962a-29ca-4e3e-aa70-9e79b6c0f349"),
                Name = "Single Leg Deadlift",
                Description = "Unilateral RDL",
                Instructions = "Balance and hamstring strength",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 20,
                SessionType = SessionType.UpperLower,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("f90839a2-0b28-475d-9ee9-64db788aee3a"),
                Name = "Deficit Deadlift",
                Description = "Deadlift from elevated platform",
                Instructions = "Increased range of motion",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 30,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("082596c3-a7de-4ae7-a337-a7503dd195b8"),
                Name = "Bench Press",
                Description = "Barbell bench press",
                Instructions = "Retract shoulders, lower to chest, press up",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 30,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("904c4f82-bfeb-4e24-b6c2-c35c5c3e99da"),
                Name = "Incline Bench Press",
                Description = "Incline barbell press",
                Instructions = "Upper chest focus",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 30,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("1578cb0a-b4ec-404a-8e88-5031bf940787"),
                Name = "Dumbbell Bench Press",
                Description = "DB bench press",
                Instructions = "Greater range of motion",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 25,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("0602349a-a6f4-4b4a-8b43-dc713670a9a6"),
                Name = "Close Grip Bench Press",
                Description = "Narrow grip bench press",
                Instructions = "Tricep emphasis",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 25,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("455e8008-1a84-4c79-977c-b51d89abdd25"),
                Name = "Paused Bench Press",
                Description = "Bench with pause on chest",
                Instructions = "Build strength off chest",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 30,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("b1e60175-428a-42e6-936a-cb9f011966cb"),
                Name = "Floor Press",
                Description = "Bench press from floor",
                Instructions = "Limited range pressing",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 25,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("14ee8c66-c096-44a3-8f6b-c7f9cf236f74"),
                Name = "Overhead Press",
                Description = "Standing barbell press",
                Instructions = "Press bar overhead, core tight",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 25,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("0b24a079-912a-4ae8-9cd0-9595df60d8d9"),
                Name = "Push Press",
                Description = "Leg drive overhead press",
                Instructions = "Dip and drive",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 25,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("775e757f-e801-41d5-8a46-69b51487ddd7"),
                Name = "Seated Overhead Press",
                Description = "Seated barbell press",
                Instructions = "Isolate shoulders",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 25,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("4d306215-58c6-43f7-9611-c9bf1efb1ff1"),
                Name = "Dumbbell Shoulder Press",
                Description = "Seated DB press",
                Instructions = "Independent arm movement",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 20,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("1380e6d8-f103-48ef-846b-a298e1432583"),
                Name = "Arnold Press",
                Description = "DB press with rotation",
                Instructions = "Full shoulder development",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 20,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("17e523fa-1f08-4e37-8b4f-45d50a9ed730"),
                Name = "Barbell Row",
                Description = "Bent over barbell row",
                Instructions = "Hip hinge, pull to sternum",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 25,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("4e502323-ec1b-4bf2-aa69-dcd924771dd0"),
                Name = "Dumbbell Row",
                Description = "Single arm DB row",
                Instructions = "Unilateral back work",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 20,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("e2060c15-73fe-4615-bba7-95bf729a2e66"),
                Name = "Cable Row",
                Description = "Seated cable row",
                Instructions = "Constant tension rowing",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 20,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("efbb863c-665c-4a33-957c-ce98595ac095"),
                Name = "T-Bar Row",
                Description = "T-bar landmine row",
                Instructions = "Thick grip rowing",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 25,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("37d87c85-de91-44ac-b21e-e9408d29a8bf"),
                Name = "Inverted Row",
                Description = "Bodyweight horizontal row",
                Instructions = "Bodyweight pulling",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 15,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("9306fd42-0767-4bf4-9875-5393bf22d506"),
                Name = "Chest Supported Row",
                Description = "Incline bench DB row",
                Instructions = "Isolate back",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 20,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("d3212fb7-a56b-441e-a151-f59d50c0bc1d"),
                Name = "Pull-up",
                Description = "Overhand grip pull-up",
                Instructions = "Pull chin over bar",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 20,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("5d28bca3-c650-4ad6-a143-5f437f218526"),
                Name = "Chin-up",
                Description = "Underhand grip pull-up",
                Instructions = "Bicep emphasis",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 20,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("8841496f-8563-48c2-870e-3d05159c0020"),
                Name = "Weighted Pull-up",
                Description = "Pull-up with added weight",
                Instructions = "Progressive overload",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 20,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("3a130254-e3e1-4bd6-995b-5a05c7d64ab8"),
                Name = "Neutral Grip Pull-up",
                Description = "Parallel grip pull-up",
                Instructions = "Joint-friendly variation",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 20,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("13c8ba7a-f28d-4b36-9ad7-5de0bc31fcfa"),
                Name = "Walking Lunge",
                Description = "Forward walking lunges",
                Instructions = "Step forward, lower to 90 degrees",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 20,
                SessionType = SessionType.UpperLower,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("d0d53ea6-9579-4a00-b247-66ec23c34a64"),
                Name = "Reverse Lunge",
                Description = "Step back lunges",
                Instructions = "Knee-friendly variation",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 20,
                SessionType = SessionType.UpperLower,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("6d10d432-9816-4664-aebf-b6292d09e431"),
                Name = "Dumbbell Lunge",
                Description = "Lunges with DBs",
                Instructions = "Added resistance",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 25,
                SessionType = SessionType.UpperLower,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("66991bb8-19d5-422d-978a-fe86b6cb01fa"),
                Name = "Jumping Lunge",
                Description = "Explosive alternating lunges",
                Instructions = "Power and conditioning",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 15,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("77e8ad07-fc68-4fa3-a587-f8ed7f53e4be"),
                Name = "Plank",
                Description = "Front plank hold",
                Instructions = "Maintain straight line, engage core",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Low,
                ApproximateDuration = 10,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("d7767331-baba-455f-a698-b7bef060ec8d"),
                Name = "Side Plank",
                Description = "Side plank hold",
                Instructions = "Anti-lateral flexion",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Low,
                ApproximateDuration = 10,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("5451b462-fa6d-408c-be58-46195553c527"),
                Name = "Dead Bug",
                Description = "Dead bug core exercise",
                Instructions = "Anti-extension control",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Low,
                ApproximateDuration = 10,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("ebc96432-04c4-4774-a8db-80527694a27b"),
                Name = "Pallof Press",
                Description = "Cable anti-rotation press",
                Instructions = "Resist rotation",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 15,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("63695dfc-214a-4f69-8900-09819469caa2"),
                Name = "Ab Wheel Rollout",
                Description = "Ab wheel rollout",
                Instructions = "Core stability and strength",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 15,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("4a9e38bf-3b16-4536-aa4b-51ca8f23ac7d"),
                Name = "Hanging Leg Raise",
                Description = "Hanging knee/leg raise",
                Instructions = "Lower ab strength",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 15,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("d1df4a95-2bd4-422c-ab9e-e853cbfd33f4"),
                Name = "Russian Twist",
                Description = "Weighted trunk rotation",
                Instructions = "Rotational core strength",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 12,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("710eaa70-cdd1-442d-81ea-4e865a772f26"),
                Name = "Dip",
                Description = "Parallel bar dips",
                Instructions = "Lower to 90 degrees, press up",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.Moderate,
                ApproximateDuration = 15,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("be29f0eb-767f-426e-b95b-7f233d90983d"),
                Name = "Weighted Dip",
                Description = "Dips with added weight",
                Instructions = "Progressive overload",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 15,
                SessionType = SessionType.PushPullLegs,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("f2a3856a-a4bd-4f10-877f-be7b250658cd"),
                Name = "Power Clean",
                Description = "Barbell power clean",
                Instructions = "Explosive hip extension, catch in quarter squat",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 30,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("a8ac6b72-aac6-4678-8313-950c6ada821f"),
                Name = "Hang Clean",
                Description = "Clean from hang position",
                Instructions = "Hip power and timing",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High,
                ApproximateDuration = 25,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("a66cb626-d9cf-4f96-b680-543e2d65e82e"),
                Name = "Snatch",
                Description = "Full barbell snatch",
                Instructions = "Technical Olympic lift",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Maximum,
                ApproximateDuration = 35,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            },
            new Exercise { 
                Id = new Guid("845ade57-d6bb-427c-ab0a-d7a3ced1e48b"),
                Name = "Clean and Jerk",
                Description = "Full clean and jerk",
                Instructions = "Complete Olympic lift movement",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.Maximum,
                ApproximateDuration = 35,
                SessionType = SessionType.FullBody,
                CreatedAt = DateTime.UtcNow
            }
        };
    }
}
