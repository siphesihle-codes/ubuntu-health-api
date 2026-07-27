using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubuntu_health_api.Migrations
{
    /// <inheritdoc />
    public partial class DiaryInvoicingAndPrescribers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PractitionerId",
                table: "Prescriptions");

            migrationBuilder.RenameColumn(
                name: "Instructionss",
                table: "PrescriptionMedication",
                newName: "Instructions");

            migrationBuilder.AddColumn<string>(
                name: "PrescriberId",
                table: "Prescriptions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrescriberLicenseNumber",
                table: "Prescriptions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrescriberName",
                table: "Prescriptions",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "Invoices",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "PatientFirstName",
                table: "Invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientLastName",
                table: "Invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PractitionerId",
                table: "Appointments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PractitionerName",
                table: "Appointments",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_TenantId_AppointmentDate",
                table: "Appointments",
                columns: new[] { "TenantId", "AppointmentDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_TenantId_AppointmentDate",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PrescriberId",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "PrescriberLicenseNumber",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "PrescriberName",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "PatientFirstName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PatientLastName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PractitionerId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PractitionerName",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "Instructions",
                table: "PrescriptionMedication",
                newName: "Instructionss");

            migrationBuilder.AddColumn<int>(
                name: "PractitionerId",
                table: "Prescriptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "Invoices",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
