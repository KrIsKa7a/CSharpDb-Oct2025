#nullable disable

namespace AcademicRecordsApp.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class InitialDbBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Exams",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Exams__3214EC078D932BAA", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Students",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Students__3214EC07AB437044", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Grades",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Value = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
            //        ExamId = table.Column<int>(type: "int", nullable: false),
            //        StudentId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK__Grades__3214EC07811E1AE1", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Grades_Exams",
            //            column: x => x.ExamId,
            //            principalTable: "Exams",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_Grades_Students",
            //            column: x => x.StudentId,
            //            principalTable: "Students",
            //            principalColumn: "Id");
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_Grades_ExamId",
                table: "Grades",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_StudentId",
                table: "Grades",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Grades");

            //migrationBuilder.DropTable(
            //    name: "Exams");

            //migrationBuilder.DropTable(
            //    name: "Students");
        }
    }
}
