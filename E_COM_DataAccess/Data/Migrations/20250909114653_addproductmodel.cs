using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_COM_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addproductmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Isbn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Listprice = table.Column<double>(type: "float", nullable: false),
                    price = table.Column<double>(type: "float", nullable: false),
                    price50 = table.Column<double>(type: "float", nullable: false),
                    price100 = table.Column<double>(type: "float", nullable: false),
                    Imageurl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    categoryid = table.Column<int>(type: "int", nullable: false),
                    covertypeid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_categoryid",
                        column: x => x.categoryid,
                        principalTable: "Categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_CoverTypes_covertypeid",
                        column: x => x.covertypeid,
                        principalTable: "CoverTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_categoryid",
                table: "Products",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_Products_covertypeid",
                table: "Products",
                column: "covertypeid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
