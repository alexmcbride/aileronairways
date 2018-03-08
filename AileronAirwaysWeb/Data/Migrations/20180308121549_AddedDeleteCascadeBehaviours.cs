using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AileronAirwaysWeb.Data.Migrations
{
    public partial class AddedDeleteCascadeBehaviours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_TimelineEvents_TimelineEventId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_TimelineEvents_Timelines_TimelineId",
                table: "TimelineEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_TimelineEvents_TimelineEventId",
                table: "Attachments",
                column: "TimelineEventId",
                principalTable: "TimelineEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TimelineEvents_Timelines_TimelineId",
                table: "TimelineEvents",
                column: "TimelineId",
                principalTable: "Timelines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_TimelineEvents_TimelineEventId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_TimelineEvents_Timelines_TimelineId",
                table: "TimelineEvents");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_TimelineEvents_TimelineEventId",
                table: "Attachments",
                column: "TimelineEventId",
                principalTable: "TimelineEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TimelineEvents_Timelines_TimelineId",
                table: "TimelineEvents",
                column: "TimelineId",
                principalTable: "Timelines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
