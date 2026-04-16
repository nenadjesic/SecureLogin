using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureLogin.Migrations
{
    /// <inheritdoc />
    public partial class CreateTestClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO [Clients] ([Id], [ClientName], [ApiKey])
                VALUES (NEWID(), 'Admin Test Client', 'F91BDAB9-14FC-40FF-B9E8-7ACE33EE4EE2')
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
