using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AileronAirwaysWeb.Data.Migrations
{
    public partial class RenamedSomeProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagesCount",
                table: "TimelineEvents",
                newName: "AttachmentImagesCount");

            migrationBuilder.RenameColumn(
                name: "AttachmentsCount",
                table: "TimelineEvents",
                newName: "AttachmentFilesCount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AttachmentImagesCount",
                table: "TimelineEvents",
                newName: "ImagesCount");

            migrationBuilder.RenameColumn(
                name: "AttachmentFilesCount",
                table: "TimelineEvents",
                newName: "AttachmentsCount");
        }
    }
}
