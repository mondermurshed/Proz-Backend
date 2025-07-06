using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proz_WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addingSomeColumnsiNFeedbackAndLeaveRequestTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "FeedbacksTable",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "CanBeSeen",
                table: "FeedbacksAnswersTable",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "FeedbacksAnswersTable",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "FeedbacksTable");

            migrationBuilder.DropColumn(
                name: "CanBeSeen",
                table: "FeedbacksAnswersTable");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "FeedbacksAnswersTable");
        }
    }
}
