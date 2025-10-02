using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorEncuestas_MVC.Migrations
{
    /// <inheritdoc />
    public partial class FixColumnMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_encuestas_usuarios_autor",
                table: "encuestas");

            migrationBuilder.DropForeignKey(
                name: "FK_preguntas_encuestas_encuesta_id",
                table: "preguntas");

            migrationBuilder.DropForeignKey(
                name: "FK_preguntas_opciones_preguntas_pregunta_id",
                table: "preguntas_opciones");

            migrationBuilder.DropForeignKey(
                name: "FK_respuestas_encuestas_encuesta_id",
                table: "respuestas");

            migrationBuilder.DropForeignKey(
                name: "FK_respuestas_preguntas_opciones_seleccion_opcion_id",
                table: "respuestas");

            migrationBuilder.DropForeignKey(
                name: "FK_respuestas_preguntas_pregunta_id",
                table: "respuestas");

            migrationBuilder.DropForeignKey(
                name: "FK_respuestas_usuarios_usuario_respuesta",
                table: "respuestas");

            migrationBuilder.DropForeignKey(
                name: "FK_respuestas_opciones_preguntas_opciones_opcion",
                table: "respuestas_opciones");

            migrationBuilder.DropForeignKey(
                name: "FK_respuestas_opciones_respuestas_respuesta_id",
                table: "respuestas_opciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_respuestas",
                table: "respuestas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_preguntas",
                table: "preguntas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_encuestas",
                table: "encuestas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_respuestas_opciones",
                table: "respuestas_opciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_preguntas_opciones",
                table: "preguntas_opciones");

            migrationBuilder.RenameTable(
                name: "respuestas",
                newName: "Respuestas");

            migrationBuilder.RenameTable(
                name: "preguntas",
                newName: "Preguntas");

            migrationBuilder.RenameTable(
                name: "encuestas",
                newName: "Encuestas");

            migrationBuilder.RenameTable(
                name: "respuestas_opciones",
                newName: "RespuestasOpciones");

            migrationBuilder.RenameTable(
                name: "preguntas_opciones",
                newName: "PreguntasOpciones");

            migrationBuilder.RenameIndex(
                name: "IX_respuestas_usuario_respuesta",
                table: "Respuestas",
                newName: "IX_Respuestas_usuario_respuesta");

            migrationBuilder.RenameIndex(
                name: "IX_respuestas_seleccion_opcion_id",
                table: "Respuestas",
                newName: "IX_Respuestas_seleccion_opcion_id");

            migrationBuilder.RenameIndex(
                name: "IX_respuestas_pregunta_id",
                table: "Respuestas",
                newName: "IX_Respuestas_pregunta_id");

            migrationBuilder.RenameIndex(
                name: "IX_respuestas_encuesta_id",
                table: "Respuestas",
                newName: "IX_Respuestas_encuesta_id");

            migrationBuilder.RenameIndex(
                name: "IX_preguntas_encuesta_id",
                table: "Preguntas",
                newName: "IX_Preguntas_encuesta_id");

            migrationBuilder.RenameIndex(
                name: "IX_encuestas_autor",
                table: "Encuestas",
                newName: "IX_Encuestas_autor");

            migrationBuilder.RenameIndex(
                name: "IX_respuestas_opciones_opcion",
                table: "RespuestasOpciones",
                newName: "IX_RespuestasOpciones_opcion");

            migrationBuilder.RenameIndex(
                name: "IX_preguntas_opciones_pregunta_id",
                table: "PreguntasOpciones",
                newName: "IX_PreguntasOpciones_pregunta_id");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "usuarios",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "passwd",
                table: "usuarios",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "access_failed_count",
                table: "usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "concurrency_stamp",
                table: "usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "usuarios",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "email_confirmed",
                table: "usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "lockout_enabled",
                table: "usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "lockout_end",
                table: "usuarios",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "normalized_email",
                table: "usuarios",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "normalized_username",
                table: "usuarios",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "phone_number_confirmed",
                table: "usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "security_stamp",
                table: "usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "two_factor_enabled",
                table: "usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "rol",
                table: "roles",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "concurrency_stamp",
                table: "roles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "roles",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "normalized_name",
                table: "roles",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Respuestas",
                table: "Respuestas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Preguntas",
                table: "Preguntas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Encuestas",
                table: "Encuestas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RespuestasOpciones",
                table: "RespuestasOpciones",
                columns: new[] { "respuesta_id", "opcion" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PreguntasOpciones",
                table: "PreguntasOpciones",
                column: "id");

            migrationBuilder.CreateTable(
                name: "role_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_claims_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_claims_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_logins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_logins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_user_logins_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_roles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_user_roles_roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_roles_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_tokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_user_tokens_usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "usuarios",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "usuarios",
                column: "normalized_username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_claims_RoleId",
                table: "role_claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_user_claims_UserId",
                table: "user_claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_logins_UserId",
                table: "user_logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_RoleId",
                table: "user_roles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Encuestas_usuarios_autor",
                table: "Encuestas",
                column: "autor",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Preguntas_Encuestas_encuesta_id",
                table: "Preguntas",
                column: "encuesta_id",
                principalTable: "Encuestas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PreguntasOpciones_Preguntas_pregunta_id",
                table: "PreguntasOpciones",
                column: "pregunta_id",
                principalTable: "Preguntas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Respuestas_Encuestas_encuesta_id",
                table: "Respuestas",
                column: "encuesta_id",
                principalTable: "Encuestas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Respuestas_PreguntasOpciones_seleccion_opcion_id",
                table: "Respuestas",
                column: "seleccion_opcion_id",
                principalTable: "PreguntasOpciones",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Respuestas_Preguntas_pregunta_id",
                table: "Respuestas",
                column: "pregunta_id",
                principalTable: "Preguntas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Respuestas_usuarios_usuario_respuesta",
                table: "Respuestas",
                column: "usuario_respuesta",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RespuestasOpciones_PreguntasOpciones_opcion",
                table: "RespuestasOpciones",
                column: "opcion",
                principalTable: "PreguntasOpciones",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RespuestasOpciones_Respuestas_respuesta_id",
                table: "RespuestasOpciones",
                column: "respuesta_id",
                principalTable: "Respuestas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Encuestas_usuarios_autor",
                table: "Encuestas");

            migrationBuilder.DropForeignKey(
                name: "FK_Preguntas_Encuestas_encuesta_id",
                table: "Preguntas");

            migrationBuilder.DropForeignKey(
                name: "FK_PreguntasOpciones_Preguntas_pregunta_id",
                table: "PreguntasOpciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Respuestas_Encuestas_encuesta_id",
                table: "Respuestas");

            migrationBuilder.DropForeignKey(
                name: "FK_Respuestas_PreguntasOpciones_seleccion_opcion_id",
                table: "Respuestas");

            migrationBuilder.DropForeignKey(
                name: "FK_Respuestas_Preguntas_pregunta_id",
                table: "Respuestas");

            migrationBuilder.DropForeignKey(
                name: "FK_Respuestas_usuarios_usuario_respuesta",
                table: "Respuestas");

            migrationBuilder.DropForeignKey(
                name: "FK_RespuestasOpciones_PreguntasOpciones_opcion",
                table: "RespuestasOpciones");

            migrationBuilder.DropForeignKey(
                name: "FK_RespuestasOpciones_Respuestas_respuesta_id",
                table: "RespuestasOpciones");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "user_claims");

            migrationBuilder.DropTable(
                name: "user_logins");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_tokens");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Respuestas",
                table: "Respuestas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Preguntas",
                table: "Preguntas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Encuestas",
                table: "Encuestas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RespuestasOpciones",
                table: "RespuestasOpciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PreguntasOpciones",
                table: "PreguntasOpciones");

            migrationBuilder.DropColumn(
                name: "access_failed_count",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "concurrency_stamp",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "email",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "email_confirmed",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "lockout_enabled",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "lockout_end",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "normalized_email",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "normalized_username",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "phone_number_confirmed",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "security_stamp",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "two_factor_enabled",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "concurrency_stamp",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "name",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "normalized_name",
                table: "roles");

            migrationBuilder.RenameTable(
                name: "Respuestas",
                newName: "respuestas");

            migrationBuilder.RenameTable(
                name: "Preguntas",
                newName: "preguntas");

            migrationBuilder.RenameTable(
                name: "Encuestas",
                newName: "encuestas");

            migrationBuilder.RenameTable(
                name: "RespuestasOpciones",
                newName: "respuestas_opciones");

            migrationBuilder.RenameTable(
                name: "PreguntasOpciones",
                newName: "preguntas_opciones");

            migrationBuilder.RenameIndex(
                name: "IX_Respuestas_usuario_respuesta",
                table: "respuestas",
                newName: "IX_respuestas_usuario_respuesta");

            migrationBuilder.RenameIndex(
                name: "IX_Respuestas_seleccion_opcion_id",
                table: "respuestas",
                newName: "IX_respuestas_seleccion_opcion_id");

            migrationBuilder.RenameIndex(
                name: "IX_Respuestas_pregunta_id",
                table: "respuestas",
                newName: "IX_respuestas_pregunta_id");

            migrationBuilder.RenameIndex(
                name: "IX_Respuestas_encuesta_id",
                table: "respuestas",
                newName: "IX_respuestas_encuesta_id");

            migrationBuilder.RenameIndex(
                name: "IX_Preguntas_encuesta_id",
                table: "preguntas",
                newName: "IX_preguntas_encuesta_id");

            migrationBuilder.RenameIndex(
                name: "IX_Encuestas_autor",
                table: "encuestas",
                newName: "IX_encuestas_autor");

            migrationBuilder.RenameIndex(
                name: "IX_RespuestasOpciones_opcion",
                table: "respuestas_opciones",
                newName: "IX_respuestas_opciones_opcion");

            migrationBuilder.RenameIndex(
                name: "IX_PreguntasOpciones_pregunta_id",
                table: "preguntas_opciones",
                newName: "IX_preguntas_opciones_pregunta_id");

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "username",
                keyValue: null,
                column: "username",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "usuarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "usuarios",
                keyColumn: "passwd",
                keyValue: null,
                column: "passwd",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "passwd",
                table: "usuarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "rol",
                table: "roles",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_respuestas",
                table: "respuestas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_preguntas",
                table: "preguntas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_encuestas",
                table: "encuestas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_respuestas_opciones",
                table: "respuestas_opciones",
                columns: new[] { "respuesta_id", "opcion" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_preguntas_opciones",
                table: "preguntas_opciones",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_encuestas_usuarios_autor",
                table: "encuestas",
                column: "autor",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_preguntas_encuestas_encuesta_id",
                table: "preguntas",
                column: "encuesta_id",
                principalTable: "encuestas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_preguntas_opciones_preguntas_pregunta_id",
                table: "preguntas_opciones",
                column: "pregunta_id",
                principalTable: "preguntas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_respuestas_encuestas_encuesta_id",
                table: "respuestas",
                column: "encuesta_id",
                principalTable: "encuestas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_respuestas_preguntas_opciones_seleccion_opcion_id",
                table: "respuestas",
                column: "seleccion_opcion_id",
                principalTable: "preguntas_opciones",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_respuestas_preguntas_pregunta_id",
                table: "respuestas",
                column: "pregunta_id",
                principalTable: "preguntas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_respuestas_usuarios_usuario_respuesta",
                table: "respuestas",
                column: "usuario_respuesta",
                principalTable: "usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_respuestas_opciones_preguntas_opciones_opcion",
                table: "respuestas_opciones",
                column: "opcion",
                principalTable: "preguntas_opciones",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_respuestas_opciones_respuestas_respuesta_id",
                table: "respuestas_opciones",
                column: "respuesta_id",
                principalTable: "respuestas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
