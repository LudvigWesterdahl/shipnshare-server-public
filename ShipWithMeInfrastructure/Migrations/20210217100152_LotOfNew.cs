using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShipWithMeInfrastructure.Migrations
{
    public partial class LotOfNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_Posts_PostId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessage_AspNetUsers_FromUserId",
                table: "ChatMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessage_Chat_ChatId",
                table: "ChatMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRequest_AspNetUsers_FromUserId",
                table: "ChatRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRequest_AspNetUsers_ToUserId",
                table: "ChatRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRequest_Chat_ChatId",
                table: "ChatRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_AspNetUsers_UserId",
                table: "ChatUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_Chat_ChatId",
                table: "ChatUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatRequest",
                table: "ChatRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessage",
                table: "ChatMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chat",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "Open",
                table: "Chat");

            migrationBuilder.RenameTable(
                name: "ChatUser",
                newName: "ChatUsers");

            migrationBuilder.RenameTable(
                name: "ChatRequest",
                newName: "ChatRequests");

            migrationBuilder.RenameTable(
                name: "ChatMessage",
                newName: "ChatMessages");

            migrationBuilder.RenameTable(
                name: "Chat",
                newName: "Chats");

            migrationBuilder.RenameColumn(
                name: "Owner",
                table: "ChatUsers",
                newName: "Active");

            migrationBuilder.RenameIndex(
                name: "IX_ChatUser_ChatId",
                table: "ChatUsers",
                newName: "IX_ChatUsers_ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatRequest_ToUserId",
                table: "ChatRequests",
                newName: "IX_ChatRequests_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatRequest_FromUserId",
                table: "ChatRequests",
                newName: "IX_ChatRequests_FromUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatRequest_ChatId",
                table: "ChatRequests",
                newName: "IX_ChatRequests_ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessage_FromUserId",
                table: "ChatMessages",
                newName: "IX_ChatMessages_FromUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessage_ChatId",
                table: "ChatMessages",
                newName: "IX_ChatMessages_ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_Chat_PostId",
                table: "Chats",
                newName: "IX_Chats_PostId");

            migrationBuilder.AddColumn<string>(
                name: "CreatedAt",
                table: "Chats",
                type: "nvarchar(48)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatUsers",
                table: "ChatUsers",
                columns: new[] { "UserId", "ChatId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatRequests",
                table: "ChatRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chats",
                table: "Chats",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserReviews",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewerId = table.Column<long>(type: "bigint", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReviews_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserReviews_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_PostId",
                table: "UserReviews",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_ReviewerId",
                table: "UserReviews",
                column: "ReviewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_AspNetUsers_FromUserId",
                table: "ChatMessages",
                column: "FromUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Chats_ChatId",
                table: "ChatMessages",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRequests_AspNetUsers_FromUserId",
                table: "ChatRequests",
                column: "FromUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRequests_AspNetUsers_ToUserId",
                table: "ChatRequests",
                column: "ToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRequests_Chats_ChatId",
                table: "ChatRequests",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Posts_PostId",
                table: "Chats",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_AspNetUsers_UserId",
                table: "ChatUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_Chats_ChatId",
                table: "ChatUsers",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_AspNetUsers_FromUserId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Chats_ChatId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRequests_AspNetUsers_FromUserId",
                table: "ChatRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRequests_AspNetUsers_ToUserId",
                table: "ChatRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatRequests_Chats_ChatId",
                table: "ChatRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Posts_PostId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_AspNetUsers_UserId",
                table: "ChatUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_Chats_ChatId",
                table: "ChatUsers");

            migrationBuilder.DropTable(
                name: "UserReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatUsers",
                table: "ChatUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chats",
                table: "Chats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatRequests",
                table: "ChatRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Chats");

            migrationBuilder.RenameTable(
                name: "ChatUsers",
                newName: "ChatUser");

            migrationBuilder.RenameTable(
                name: "Chats",
                newName: "Chat");

            migrationBuilder.RenameTable(
                name: "ChatRequests",
                newName: "ChatRequest");

            migrationBuilder.RenameTable(
                name: "ChatMessages",
                newName: "ChatMessage");

            migrationBuilder.RenameColumn(
                name: "Active",
                table: "ChatUser",
                newName: "Owner");

            migrationBuilder.RenameIndex(
                name: "IX_ChatUsers_ChatId",
                table: "ChatUser",
                newName: "IX_ChatUser_ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_PostId",
                table: "Chat",
                newName: "IX_Chat_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatRequests_ToUserId",
                table: "ChatRequest",
                newName: "IX_ChatRequest_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatRequests_FromUserId",
                table: "ChatRequest",
                newName: "IX_ChatRequest_FromUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatRequests_ChatId",
                table: "ChatRequest",
                newName: "IX_ChatRequest_ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessages_FromUserId",
                table: "ChatMessage",
                newName: "IX_ChatMessage_FromUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessages_ChatId",
                table: "ChatMessage",
                newName: "IX_ChatMessage_ChatId");

            migrationBuilder.AddColumn<bool>(
                name: "Open",
                table: "Chat",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser",
                columns: new[] { "UserId", "ChatId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chat",
                table: "Chat",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatRequest",
                table: "ChatRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessage",
                table: "ChatMessage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_Posts_PostId",
                table: "Chat",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessage_AspNetUsers_FromUserId",
                table: "ChatMessage",
                column: "FromUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessage_Chat_ChatId",
                table: "ChatMessage",
                column: "ChatId",
                principalTable: "Chat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRequest_AspNetUsers_FromUserId",
                table: "ChatRequest",
                column: "FromUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRequest_AspNetUsers_ToUserId",
                table: "ChatRequest",
                column: "ToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRequest_Chat_ChatId",
                table: "ChatRequest",
                column: "ChatId",
                principalTable: "Chat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUser_AspNetUsers_UserId",
                table: "ChatUser",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUser_Chat_ChatId",
                table: "ChatUser",
                column: "ChatId",
                principalTable: "Chat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
