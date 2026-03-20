using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class ClassScheduling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CompanyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Details = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    StudioRoomId = table.Column<Guid>(type: "TEXT", nullable: false),
                    InstructorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DanceStyleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LevelId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: false),
                    Recurrence_DayOfWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    Recurrence_StartTime = table.Column<string>(type: "TEXT", nullable: false),
                    Recurrence_EndTime = table.Column<string>(type: "TEXT", nullable: false),
                    Recurrence_RecurrenceStartDate = table.Column<string>(type: "TEXT", nullable: false),
                    Recurrence_RecurrenceEndDate = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_DanceStyles_DanceStyleId",
                        column: x => x.DanceStyleId,
                        principalTable: "DanceStyles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Classes_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Classes_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Classes_StudioRooms_StudioRoomId",
                        column: x => x.StudioRoomId,
                        principalTable: "StudioRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClassSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CompanyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClassId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<string>(type: "TEXT", nullable: false),
                    StartTime = table.Column<string>(type: "TEXT", nullable: false),
                    EndTime = table.Column<string>(type: "TEXT", nullable: false),
                    StudioRoomId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsCancelled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CancellationReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsException = table.Column<bool>(type: "INTEGER", nullable: false),
                    Details = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassSchedules_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassSchedules_StudioRooms_StudioRoomId",
                        column: x => x.StudioRoomId,
                        principalTable: "StudioRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_CompanyId",
                table: "Classes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_DanceStyleId",
                table: "Classes",
                column: "DanceStyleId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_InstructorId",
                table: "Classes",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_LevelId",
                table: "Classes",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_StudioRoomId",
                table: "Classes",
                column: "StudioRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_ClassId",
                table: "ClassSchedules",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_CompanyId",
                table: "ClassSchedules",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_StudioRoomId_Date",
                table: "ClassSchedules",
                columns: new[] { "StudioRoomId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassSchedules");

            migrationBuilder.DropTable(
                name: "Classes");
        }
    }
}
