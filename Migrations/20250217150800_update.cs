using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_ReceiverId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_SenderId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_CommentOwnerId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Users_LikeOwnerId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatNums",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_MessageOwnerId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Projects_ProjectId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_OwnerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_SysActions_Users_UserId",
                table: "SysActions");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "userName");

            migrationBuilder.RenameColumn(
                name: "Transcript",
                table: "Users",
                newName: "transcript");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Users",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "PasswordResetVerified",
                table: "Users",
                newName: "passwordResetVerified");

            migrationBuilder.RenameColumn(
                name: "PasswordResetExpires",
                table: "Users",
                newName: "passwordResetExpires");

            migrationBuilder.RenameColumn(
                name: "PasswordResetCode",
                table: "Users",
                newName: "passwordResetCode");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "Users",
                newName: "lastModified");

            migrationBuilder.RenameColumn(
                name: "IsBlocked",
                table: "Users",
                newName: "isBlocked");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Users",
                newName: "image");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Users",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Users",
                newName: "creationTime");

            migrationBuilder.RenameColumn(
                name: "Bio",
                table: "Users",
                newName: "bio");

            migrationBuilder.RenameColumn(
                name: "Age",
                table: "Users",
                newName: "age");

            migrationBuilder.RenameColumn(
                name: "Active",
                table: "Users",
                newName: "active");

            migrationBuilder.RenameColumn(
                name: "Address_Details",
                table: "Users",
                newName: "addresses_details");

            migrationBuilder.RenameColumn(
                name: "Address_Country",
                table: "Users",
                newName: "addresses_Country");

            migrationBuilder.RenameColumn(
                name: "Address_CityOrTown",
                table: "Users",
                newName: "addresses_cityOrTown");

            migrationBuilder.RenameColumn(
                name: "GmailAcc",
                table: "Users",
                newName: "Gmail_Acc");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Tokens",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Expires",
                table: "Tokens",
                newName: "expires");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "SysActions",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "SysActions",
                newName: "lastModified");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "SysActions",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "SysActions",
                newName: "creationTime");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "SysActions",
                newName: "action");

            migrationBuilder.RenameIndex(
                name: "IX_SysActions_UserId",
                table: "SysActions",
                newName: "IX_SysActions_userId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Projects",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "ProjectName",
                table: "Projects",
                newName: "projectName");

            migrationBuilder.RenameColumn(
                name: "Pdf",
                table: "Projects",
                newName: "pdf");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Projects",
                newName: "ownerId");

            migrationBuilder.RenameColumn(
                name: "NumberOfUsersAddedThisProjectToFavoriteList",
                table: "Projects",
                newName: "numberOfUsersAddedThisProjectToFavoriteList");

            migrationBuilder.RenameColumn(
                name: "NumberOfLikes",
                table: "Projects",
                newName: "numberOfLikes");

            migrationBuilder.RenameColumn(
                name: "NumberOfComments",
                table: "Projects",
                newName: "numberOfComments");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "Projects",
                newName: "lastModified");

            migrationBuilder.RenameColumn(
                name: "Faculty",
                table: "Projects",
                newName: "faculty");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Projects",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Projects",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Projects",
                newName: "creationTime");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Projects",
                newName: "category");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_OwnerId",
                table: "Projects",
                newName: "IX_Projects_ownerId");

            migrationBuilder.RenameColumn(
                name: "ProjectOwner",
                table: "Notifications",
                newName: "projectOwner");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Notifications",
                newName: "projectId");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "Notifications",
                newName: "lastModified");

            migrationBuilder.RenameColumn(
                name: "EventOwner",
                table: "Notifications",
                newName: "eventOwner");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Notifications",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Notifications",
                newName: "creationTime");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Notifications",
                newName: "content");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ProjectId",
                table: "Notifications",
                newName: "IX_Notifications_projectId");

            migrationBuilder.RenameColumn(
                name: "TheMessage",
                table: "Messages",
                newName: "theMessage");

            migrationBuilder.RenameColumn(
                name: "MessageOwnerId",
                table: "Messages",
                newName: "messageOwnerId");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Messages",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "ChatNums",
                table: "Messages",
                newName: "chatNums");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_MessageOwnerId",
                table: "Messages",
                newName: "IX_Messages_messageOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ChatNums",
                table: "Messages",
                newName: "IX_Messages_chatNums");

            migrationBuilder.RenameColumn(
                name: "LikeOwnerId",
                table: "Likes",
                newName: "likeOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_LikeOwnerId",
                table: "Likes",
                newName: "IX_Likes_likeOwnerId");

            migrationBuilder.RenameColumn(
                name: "NumberOfCommentLikes",
                table: "Comments",
                newName: "numberOfCommentLikes");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Comments",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Comments",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "CommentOwnerId",
                table: "Comments",
                newName: "commentOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_CommentOwnerId",
                table: "Comments",
                newName: "IX_Comments_commentOwnerId");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Chats",
                newName: "senderId");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Chats",
                newName: "receiverId");

            migrationBuilder.RenameColumn(
                name: "LastModified",
                table: "Chats",
                newName: "lastModified");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Chats",
                newName: "creationTime");

            migrationBuilder.RenameColumn(
                name: "ChatName",
                table: "Chats",
                newName: "chatName");

            migrationBuilder.RenameColumn(
                name: "ChatNums",
                table: "Chats",
                newName: "chatNums");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_SenderId",
                table: "Chats",
                newName: "IX_Chats_senderId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_ReceiverId",
                table: "Chats",
                newName: "IX_Chats_receiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_receiverId",
                table: "Chats",
                column: "receiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_senderId",
                table: "Chats",
                column: "senderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_commentOwnerId",
                table: "Comments",
                column: "commentOwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Users_likeOwnerId",
                table: "Likes",
                column: "likeOwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_chatNums",
                table: "Messages",
                column: "chatNums",
                principalTable: "Chats",
                principalColumn: "chatNums");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_messageOwnerId",
                table: "Messages",
                column: "messageOwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Projects_projectId",
                table: "Notifications",
                column: "projectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_ownerId",
                table: "Projects",
                column: "ownerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SysActions_Users_userId",
                table: "SysActions",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_receiverId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_senderId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_commentOwnerId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Users_likeOwnerId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_chatNums",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_messageOwnerId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Projects_projectId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_ownerId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_SysActions_Users_userId",
                table: "SysActions");

            migrationBuilder.RenameColumn(
                name: "userName",
                table: "Users",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "transcript",
                table: "Users",
                newName: "Transcript");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "Users",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "passwordResetVerified",
                table: "Users",
                newName: "PasswordResetVerified");

            migrationBuilder.RenameColumn(
                name: "passwordResetExpires",
                table: "Users",
                newName: "PasswordResetExpires");

            migrationBuilder.RenameColumn(
                name: "passwordResetCode",
                table: "Users",
                newName: "PasswordResetCode");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "Users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "lastModified",
                table: "Users",
                newName: "LastModified");

            migrationBuilder.RenameColumn(
                name: "isBlocked",
                table: "Users",
                newName: "IsBlocked");

            migrationBuilder.RenameColumn(
                name: "image",
                table: "Users",
                newName: "Image");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "Users",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "creationTime",
                table: "Users",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "bio",
                table: "Users",
                newName: "Bio");

            migrationBuilder.RenameColumn(
                name: "age",
                table: "Users",
                newName: "Age");

            migrationBuilder.RenameColumn(
                name: "active",
                table: "Users",
                newName: "Active");

            migrationBuilder.RenameColumn(
                name: "addresses_details",
                table: "Users",
                newName: "Address_Details");

            migrationBuilder.RenameColumn(
                name: "addresses_cityOrTown",
                table: "Users",
                newName: "Address_CityOrTown");

            migrationBuilder.RenameColumn(
                name: "addresses_Country",
                table: "Users",
                newName: "Address_Country");

            migrationBuilder.RenameColumn(
                name: "Gmail_Acc",
                table: "Users",
                newName: "GmailAcc");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "Tokens",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "expires",
                table: "Tokens",
                newName: "Expires");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "SysActions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "lastModified",
                table: "SysActions",
                newName: "LastModified");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "SysActions",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "creationTime",
                table: "SysActions",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "action",
                table: "SysActions",
                newName: "Action");

            migrationBuilder.RenameIndex(
                name: "IX_SysActions_userId",
                table: "SysActions",
                newName: "IX_SysActions_UserId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Projects",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "projectName",
                table: "Projects",
                newName: "ProjectName");

            migrationBuilder.RenameColumn(
                name: "pdf",
                table: "Projects",
                newName: "Pdf");

            migrationBuilder.RenameColumn(
                name: "ownerId",
                table: "Projects",
                newName: "OwnerId");

            migrationBuilder.RenameColumn(
                name: "numberOfUsersAddedThisProjectToFavoriteList",
                table: "Projects",
                newName: "NumberOfUsersAddedThisProjectToFavoriteList");

            migrationBuilder.RenameColumn(
                name: "numberOfLikes",
                table: "Projects",
                newName: "NumberOfLikes");

            migrationBuilder.RenameColumn(
                name: "numberOfComments",
                table: "Projects",
                newName: "NumberOfComments");

            migrationBuilder.RenameColumn(
                name: "lastModified",
                table: "Projects",
                newName: "LastModified");

            migrationBuilder.RenameColumn(
                name: "faculty",
                table: "Projects",
                newName: "Faculty");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Projects",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "Projects",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "creationTime",
                table: "Projects",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "Projects",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_ownerId",
                table: "Projects",
                newName: "IX_Projects_OwnerId");

            migrationBuilder.RenameColumn(
                name: "projectOwner",
                table: "Notifications",
                newName: "ProjectOwner");

            migrationBuilder.RenameColumn(
                name: "projectId",
                table: "Notifications",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "lastModified",
                table: "Notifications",
                newName: "LastModified");

            migrationBuilder.RenameColumn(
                name: "eventOwner",
                table: "Notifications",
                newName: "EventOwner");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "Notifications",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "creationTime",
                table: "Notifications",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Notifications",
                newName: "Content");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_projectId",
                table: "Notifications",
                newName: "IX_Notifications_ProjectId");

            migrationBuilder.RenameColumn(
                name: "theMessage",
                table: "Messages",
                newName: "TheMessage");

            migrationBuilder.RenameColumn(
                name: "messageOwnerId",
                table: "Messages",
                newName: "MessageOwnerId");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "Messages",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "chatNums",
                table: "Messages",
                newName: "ChatNums");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_messageOwnerId",
                table: "Messages",
                newName: "IX_Messages_MessageOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_chatNums",
                table: "Messages",
                newName: "IX_Messages_ChatNums");

            migrationBuilder.RenameColumn(
                name: "likeOwnerId",
                table: "Likes",
                newName: "LikeOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_likeOwnerId",
                table: "Likes",
                newName: "IX_Likes_LikeOwnerId");

            migrationBuilder.RenameColumn(
                name: "numberOfCommentLikes",
                table: "Comments",
                newName: "NumberOfCommentLikes");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "Comments",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Comments",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "commentOwnerId",
                table: "Comments",
                newName: "CommentOwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_commentOwnerId",
                table: "Comments",
                newName: "IX_Comments_CommentOwnerId");

            migrationBuilder.RenameColumn(
                name: "senderId",
                table: "Chats",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "receiverId",
                table: "Chats",
                newName: "ReceiverId");

            migrationBuilder.RenameColumn(
                name: "lastModified",
                table: "Chats",
                newName: "LastModified");

            migrationBuilder.RenameColumn(
                name: "creationTime",
                table: "Chats",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "chatName",
                table: "Chats",
                newName: "ChatName");

            migrationBuilder.RenameColumn(
                name: "chatNums",
                table: "Chats",
                newName: "ChatNums");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_senderId",
                table: "Chats",
                newName: "IX_Chats_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_receiverId",
                table: "Chats",
                newName: "IX_Chats_ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_ReceiverId",
                table: "Chats",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_SenderId",
                table: "Chats",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_CommentOwnerId",
                table: "Comments",
                column: "CommentOwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Users_LikeOwnerId",
                table: "Likes",
                column: "LikeOwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatNums",
                table: "Messages",
                column: "ChatNums",
                principalTable: "Chats",
                principalColumn: "ChatNums");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_MessageOwnerId",
                table: "Messages",
                column: "MessageOwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Projects_ProjectId",
                table: "Notifications",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_OwnerId",
                table: "Projects",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SysActions_Users_UserId",
                table: "SysActions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
