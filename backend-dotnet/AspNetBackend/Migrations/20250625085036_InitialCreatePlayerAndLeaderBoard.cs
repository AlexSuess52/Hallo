using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AspNetBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatePlayerAndLeaderBoard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "players");

            migrationBuilder.CreateTable(
                name: "leaderboard",
                schema: "players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    player_name = table.Column<string>(type: "text", nullable: true),
                    total_score = table.Column<long>(type: "bigint", nullable: false),
                    total_time = table.Column<long>(type: "bigint", nullable: false),
                    sessions_played = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leaderboard", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "players",
                schema: "players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    refresh_token = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    refresh_token_expiry_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    user_email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "leaderboard",
                schema: "players");

            migrationBuilder.DropTable(
                name: "players",
                schema: "players");
        }
    }
}
