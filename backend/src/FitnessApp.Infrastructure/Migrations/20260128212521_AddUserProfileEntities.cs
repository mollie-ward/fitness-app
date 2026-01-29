using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    HyroxLevel = table.Column<int>(type: "integer", nullable: false),
                    RunningLevel = table.Column<int>(type: "integer", nullable: false),
                    StrengthLevel = table.Column<int>(type: "integer", nullable: false),
                    ScheduleAvailability_Monday = table.Column<bool>(type: "boolean", nullable: true),
                    ScheduleAvailability_Tuesday = table.Column<bool>(type: "boolean", nullable: true),
                    ScheduleAvailability_Wednesday = table.Column<bool>(type: "boolean", nullable: true),
                    ScheduleAvailability_Thursday = table.Column<bool>(type: "boolean", nullable: true),
                    ScheduleAvailability_Friday = table.Column<bool>(type: "boolean", nullable: true),
                    ScheduleAvailability_Saturday = table.Column<bool>(type: "boolean", nullable: true),
                    ScheduleAvailability_Sunday = table.Column<bool>(type: "boolean", nullable: true),
                    ScheduleAvailability_MinimumSessionsPerWeek = table.Column<int>(type: "integer", nullable: true),
                    ScheduleAvailability_MaximumSessionsPerWeek = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InjuryLimitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    BodyPart = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    InjuryType = table.Column<int>(type: "integer", nullable: false),
                    ReportedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    MovementRestrictions = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InjuryLimitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InjuryLimitations_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingBackgrounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    HasStructuredTrainingExperience = table.Column<bool>(type: "boolean", nullable: false),
                    PreviousTrainingDetails = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    EquipmentFamiliarity = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TrainingHistoryDetails = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingBackgrounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingBackgrounds_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainingGoals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    GoalType = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainingGoals_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InjuryLimitations_UserProfileId",
                table: "InjuryLimitations",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingBackgrounds_UserProfileId",
                table: "TrainingBackgrounds",
                column: "UserProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingGoals_UserProfileId",
                table: "TrainingGoals",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InjuryLimitations");

            migrationBuilder.DropTable(
                name: "TrainingBackgrounds");

            migrationBuilder.DropTable(
                name: "TrainingGoals");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
