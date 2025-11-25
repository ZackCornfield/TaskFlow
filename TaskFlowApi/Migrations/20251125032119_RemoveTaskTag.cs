using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlowApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTaskTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Boards_BoardId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "TaskTags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_BoardId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "BoardId",
                table: "Tags");

            migrationBuilder.AddColumn<int>(
                name: "TaskItemId",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TaskItemId",
                table: "Tags",
                column: "TaskItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Tasks_TaskItemId",
                table: "Tags",
                column: "TaskItemId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Tasks_TaskItemId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TaskItemId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TaskItemId",
                table: "Tags");

            migrationBuilder.AddColumn<int>(
                name: "BoardId",
                table: "Tags",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TaskTags",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTags", x => new { x.TaskId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TaskTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskTags_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_BoardId",
                table: "Tags",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTags_TagId",
                table: "TaskTags",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Boards_BoardId",
                table: "Tags",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
