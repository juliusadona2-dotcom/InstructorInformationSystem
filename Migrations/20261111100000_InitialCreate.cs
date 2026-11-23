using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstructorInformationSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instructors",
                columns: table => new
                {
                    InstructorId = table.Column<string>(type: "TEXT", maxLength: 7, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Designation = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ContactNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Majorship = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BachelorDegree = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.InstructorId);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    UserAccountId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.UserAccountId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_EmailAddress",
                table: "Instructors",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_Email",
                table: "UserAccounts",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Instructors");

            migrationBuilder.DropTable(
                name: "UserAccounts");
        }
    }
}
