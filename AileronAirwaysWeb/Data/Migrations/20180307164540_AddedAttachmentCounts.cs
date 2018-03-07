using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AileronAirwaysWeb.Data.Migrations
{
    public partial class AddedAttachmentCounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttachmentsCount",
                table: "TimelineEvents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImagesCount",
                table: "TimelineEvents",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentsCount",
                table: "TimelineEvents");

            migrationBuilder.DropColumn(
                name: "ImagesCount",
                table: "TimelineEvents");
        }
    }
}
