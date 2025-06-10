using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final.Migrations
{
    /// <inheritdoc />
    public partial class updatecomm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Projects_ProjectId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Comments_CommentId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Projects_ProjectId",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "GmailAcc",
                table: "Users",
                newName: "gmailAcc");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Likes",
                newName: "projectId");

            migrationBuilder.RenameColumn(
                name: "CommentId",
                table: "Likes",
                newName: "commentId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_ProjectId",
                table: "Likes",
                newName: "IX_Likes_projectId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_CommentId",
                table: "Likes",
                newName: "IX_Likes_commentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Projects_ProjectId",
                table: "Comments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Comments_commentId",
                table: "Likes",
                column: "commentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Projects_projectId",
                table: "Likes",
                column: "projectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Projects_ProjectId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Comments_commentId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Projects_projectId",
                table: "Likes");

            migrationBuilder.RenameColumn(
                name: "gmailAcc",
                table: "Users",
                newName: "GmailAcc");

            migrationBuilder.RenameColumn(
                name: "projectId",
                table: "Likes",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "commentId",
                table: "Likes",
                newName: "CommentId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_projectId",
                table: "Likes",
                newName: "IX_Likes_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_commentId",
                table: "Likes",
                newName: "IX_Likes_CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Projects_ProjectId",
                table: "Comments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Comments_CommentId",
                table: "Likes",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Projects_ProjectId",
                table: "Likes",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
