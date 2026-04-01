using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlackClone.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChannelsMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "channels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    topic = table.Column<string>(type: "text", nullable: true),
                    purpose = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false),
                    archived_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    archived_by = table.Column<Guid>(type: "uuid", nullable: true),
                    member_count = table.Column<int>(type: "integer", nullable: false),
                    last_message_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_channels", x => x.id);
                    table.ForeignKey(
                        name: "FK_channels_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_channels_workspaces_workspace_id",
                        column: x => x.workspace_id,
                        principalTable: "workspaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "channel_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    last_read_message_id = table.Column<Guid>(type: "uuid", nullable: true),
                    last_read_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    notification_pref = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_muted = table.Column<bool>(type: "boolean", nullable: false),
                    is_starred = table.Column<bool>(type: "boolean", nullable: false),
                    joined_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    left_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_channel_members", x => x.id);
                    table.ForeignKey(
                        name: "FK_channel_members_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_channel_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_channel_members_workspaces_workspace_id",
                        column: x => x.workspace_id,
                        principalTable: "workspaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    content_format = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    thread_parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reply_count = table.Column<int>(type: "integer", nullable: false),
                    latest_reply_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    latest_reply_sender_ids = table.Column<Guid[]>(type: "uuid[]", nullable: true),
                    is_pinned = table.Column<bool>(type: "boolean", nullable: false),
                    pinned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    pinned_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_edited = table.Column<bool>(type: "boolean", nullable: false),
                    edited_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_messages_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messages_messages_thread_parent_id",
                        column: x => x.thread_parent_id,
                        principalTable: "messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messages_users_sender_id",
                        column: x => x.sender_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_messages_workspaces_workspace_id",
                        column: x => x.workspace_id,
                        principalTable: "workspaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "message_mentions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    mentioned_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    mention_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message_mentions", x => x.id);
                    table.ForeignKey(
                        name: "FK_message_mentions_messages_message_id",
                        column: x => x.message_id,
                        principalTable: "messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_message_mentions_users_mentioned_user_id",
                        column: x => x.mentioned_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "channel_members_channel_idx",
                table: "channel_members",
                column: "channel_id",
                filter: "left_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "channel_members_unique_idx",
                table: "channel_members",
                columns: new[] { "channel_id", "user_id" },
                unique: true,
                filter: "left_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "channel_members_user_idx",
                table: "channel_members",
                columns: new[] { "user_id", "workspace_id" },
                filter: "left_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_channel_members_workspace_id",
                table: "channel_members",
                column: "workspace_id");

            migrationBuilder.CreateIndex(
                name: "channels_last_message_idx",
                table: "channels",
                columns: new[] { "workspace_id", "last_message_at" });

            migrationBuilder.CreateIndex(
                name: "channels_name_idx",
                table: "channels",
                columns: new[] { "workspace_id", "name" },
                unique: true,
                filter: "type IN ('public', 'private') AND is_deleted = FALSE");

            migrationBuilder.CreateIndex(
                name: "channels_type_idx",
                table: "channels",
                columns: new[] { "workspace_id", "type" });

            migrationBuilder.CreateIndex(
                name: "channels_workspace_idx",
                table: "channels",
                column: "workspace_id",
                filter: "is_deleted = FALSE AND is_archived = FALSE");

            migrationBuilder.CreateIndex(
                name: "IX_channels_created_by",
                table: "channels",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "message_mentions_message_idx",
                table: "message_mentions",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "message_mentions_user_idx",
                table: "message_mentions",
                columns: new[] { "mentioned_user_id", "workspace_id" });

            migrationBuilder.CreateIndex(
                name: "messages_pinned_idx",
                table: "messages",
                column: "channel_id",
                filter: "is_pinned = TRUE");

            migrationBuilder.CreateIndex(
                name: "messages_sender_idx",
                table: "messages",
                columns: new[] { "sender_id", "workspace_id" });

            migrationBuilder.CreateIndex(
                name: "messages_thread_idx",
                table: "messages",
                column: "thread_parent_id",
                filter: "thread_parent_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "messages_workspace_ts_idx",
                table: "messages",
                columns: new[] { "workspace_id", "created_at" },
                filter: "is_deleted = FALSE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "channel_members");

            migrationBuilder.DropTable(
                name: "message_mentions");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "channels");
        }
    }
}
