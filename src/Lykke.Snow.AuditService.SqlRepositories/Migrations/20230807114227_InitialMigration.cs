using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lykke.Snow.AuditService.SqlRepositories.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "audit");

            migrationBuilder.CreateTable(
                name: "Events",
                schema: "audit",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorporateActionsId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ActionTypeDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DataReference = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DataDiff = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events",
                schema: "audit");
        }
    }
}
