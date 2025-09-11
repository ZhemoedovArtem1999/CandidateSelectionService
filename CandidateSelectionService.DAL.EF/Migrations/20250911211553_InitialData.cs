using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CandidateSelectionService.DAL.EF.Migrations
{
    /// <inheritdoc />
    public partial class InitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "hr");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.EnsureSchema(
                name: "verification");

            migrationBuilder.CreateTable(
                name: "data_candidate",
                schema: "hr",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lastname = table.Column<string>(type: "text", nullable: false),
                    firstname = table.Column<string>(type: "text", nullable: false),
                    middlename = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: false),
                    date_birth = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("data_candidate_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "social_network_type",
                schema: "hr",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("social_network_type_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lastname = table.Column<string>(type: "text", nullable: false),
                    firstname = table.Column<string>(type: "text", nullable: false),
                    middlename = table.Column<string>(type: "text", nullable: false),
                    login = table.Column<string>(type: "text", nullable: false),
                    salt = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "verification",
                schema: "verification",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_name = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    search_data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("verification_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "work_schedule",
                schema: "hr",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("work_schedule_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "social_network",
                schema: "hr",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data_candidate_id = table.Column<int>(type: "integer", nullable: false),
                    lastname = table.Column<string>(type: "text", nullable: false),
                    firstname = table.Column<string>(type: "text", nullable: false),
                    type_id = table.Column<int>(type: "integer", nullable: false),
                    date_added = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("social_network_pkey", x => x.id);
                    table.ForeignKey(
                        name: "social_network_data_candidate_fkey",
                        column: x => x.data_candidate_id,
                        principalSchema: "hr",
                        principalTable: "data_candidate",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "social_network_social_type_fkey",
                        column: x => x.type_id,
                        principalSchema: "hr",
                        principalTable: "social_network_type",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "refresh_token",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    replaced_by_token = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("refresh_token_pkey", x => x.id);
                    table.ForeignKey(
                        name: "refresh_token_user_id_fkey",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "verification_event",
                schema: "verification",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "text", nullable: false),
                    verification_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("verification_event_pkey", x => x.id);
                    table.ForeignKey(
                        name: "verification_event_verification_id_fkey",
                        column: x => x.verification_id,
                        principalSchema: "verification",
                        principalTable: "verification",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "candidate",
                schema: "hr",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data_id = table.Column<int>(type: "integer", nullable: false),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_user_id = table.Column<int>(type: "integer", nullable: false),
                    work_schedule_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("candidate_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_candidate_data_candidate_data_id",
                        column: x => x.data_id,
                        principalSchema: "hr",
                        principalTable: "data_candidate",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "candidate_work_schedule_id_fkey",
                        column: x => x.work_schedule_id,
                        principalSchema: "hr",
                        principalTable: "work_schedule",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "employee",
                schema: "hr",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    data_id = table.Column<int>(type: "integer", nullable: false),
                    employment_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    work_schedule_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_pkey", x => x.id);
                    table.ForeignKey(
                        name: "FK_employee_data_candidate_data_id",
                        column: x => x.data_id,
                        principalSchema: "hr",
                        principalTable: "data_candidate",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "employee_work_schedule_id_fkey",
                        column: x => x.work_schedule_id,
                        principalSchema: "hr",
                        principalTable: "work_schedule",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "verification_event_result",
                schema: "verification",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    verification_event_id = table.Column<int>(type: "integer", nullable: false),
                    entity_id = table.Column<int>(type: "integer", nullable: false),
                    lastname = table.Column<string>(type: "text", nullable: false),
                    firstname = table.Column<string>(type: "text", nullable: false),
                    middlename = table.Column<string>(type: "text", nullable: false),
                    date_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    phone = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: false),
                    work_schedule = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("verification_event_result_pkey", x => x.id);
                    table.ForeignKey(
                        name: "verification_event_result_verification_event_id_fkey",
                        column: x => x.verification_event_id,
                        principalSchema: "verification",
                        principalTable: "verification_event",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_candidate_data_id",
                schema: "hr",
                table: "candidate",
                column: "data_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_candidate_work_schedule_id",
                schema: "hr",
                table: "candidate",
                column: "work_schedule_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_data_id",
                schema: "hr",
                table: "employee",
                column: "data_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_work_schedule_id",
                schema: "hr",
                table: "employee",
                column: "work_schedule_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_user_id",
                schema: "auth",
                table: "refresh_token",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_social_network_data_candidate_id",
                schema: "hr",
                table: "social_network",
                column: "data_candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_social_network_type_id",
                schema: "hr",
                table: "social_network",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "IX_verification_event_verification_id",
                schema: "verification",
                table: "verification_event",
                column: "verification_id");

            migrationBuilder.CreateIndex(
                name: "IX_verification_event_result_verification_event_id",
                schema: "verification",
                table: "verification_event_result",
                column: "verification_event_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "candidate",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "employee",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "refresh_token",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "social_network",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "verification_event_result",
                schema: "verification");

            migrationBuilder.DropTable(
                name: "work_schedule",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "user",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "data_candidate",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "social_network_type",
                schema: "hr");

            migrationBuilder.DropTable(
                name: "verification_event",
                schema: "verification");

            migrationBuilder.DropTable(
                name: "verification",
                schema: "verification");
        }
    }
}
