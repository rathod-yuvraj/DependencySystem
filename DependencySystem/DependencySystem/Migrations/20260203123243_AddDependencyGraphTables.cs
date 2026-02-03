using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DependencySystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDependencyGraphTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dependencies",
                columns: table => new
                {
                    DependencyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SourceModuleID = table.Column<int>(type: "int", nullable: false),
                    TargetModuleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dependencies", x => x.DependencyID);
                    table.ForeignKey(
                        name: "FK_Dependencies_Modules_SourceModuleID",
                        column: x => x.SourceModuleID,
                        principalTable: "Modules",
                        principalColumn: "ModuleID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dependencies_Modules_TargetModuleID",
                        column: x => x.TargetModuleID,
                        principalTable: "Modules",
                        principalColumn: "ModuleID",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TaskDependencies",
                columns: table => new
                {
                    TaskID = table.Column<int>(type: "int", nullable: false),
                    DependsOnTaskID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDependencies", x => new { x.TaskID, x.DependsOnTaskID });
                    table.ForeignKey(
                        name: "FK_TaskDependencies_Tasks_DependsOnTaskID",
                        column: x => x.DependsOnTaskID,
                        principalTable: "Tasks",
                        principalColumn: "TaskID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskDependencies_Tasks_TaskID",
                        column: x => x.TaskID,
                        principalTable: "Tasks",
                        principalColumn: "TaskID",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Dependencies_SourceModuleID_TargetModuleID",
                table: "Dependencies",
                columns: new[] { "SourceModuleID", "TargetModuleID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dependencies_TargetModuleID",
                table: "Dependencies",
                column: "TargetModuleID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDependencies_DependsOnTaskID",
                table: "TaskDependencies",
                column: "DependsOnTaskID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dependencies");

            migrationBuilder.DropTable(
                name: "TaskDependencies");
        }
    }
}
