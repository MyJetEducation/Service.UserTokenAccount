using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Service.UserTokenAccount.Postgres.Migrations
{
    public partial class InitialCreateUserTokenAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "education");

            migrationBuilder.CreateTable(
                name: "usertoken_account",
                schema: "education",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usertoken_account", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "usertoken_operation",
                schema: "education",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Movement = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    ProductType = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    Info = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usertoken_operation", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_usertoken_operation_Date",
                schema: "education",
                table: "usertoken_operation",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_usertoken_operation_Movement",
                schema: "education",
                table: "usertoken_operation",
                column: "Movement");

            migrationBuilder.CreateIndex(
                name: "IX_usertoken_operation_ProductType",
                schema: "education",
                table: "usertoken_operation",
                column: "ProductType");

            migrationBuilder.CreateIndex(
                name: "IX_usertoken_operation_Source",
                schema: "education",
                table: "usertoken_operation",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_usertoken_operation_UserId",
                schema: "education",
                table: "usertoken_operation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_usertoken_operation_Value",
                schema: "education",
                table: "usertoken_operation",
                column: "Value");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usertoken_account",
                schema: "education");

            migrationBuilder.DropTable(
                name: "usertoken_operation",
                schema: "education");
        }
    }
}
