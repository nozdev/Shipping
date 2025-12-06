using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shipping.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Itinerary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Itinerary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarrierMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FromId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Departure = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Arrival = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarrierMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarrierMovements_Location_FromId",
                        column: x => x.FromId,
                        principalTable: "Location",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CarrierMovements_Location_ToId",
                        column: x => x.ToId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DeliverySpecification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DestinationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DestinationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LatestArrival = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliverySpecification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliverySpecification_Location_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Leg",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VesselVoyageId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoadLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LoadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnloadLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnloadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ItineraryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leg", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leg_Itinerary_ItineraryId",
                        column: x => x.ItineraryId,
                        principalTable: "Itinerary",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Leg_Location_LoadLocationId",
                        column: x => x.LoadLocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Leg_Location_UnloadLocationId",
                        column: x => x.UnloadLocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HandlingEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CargoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CarrierMovementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandlingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HandlingEvents_CarrierMovements_CarrierMovementId",
                        column: x => x.CarrierMovementId,
                        principalTable: "CarrierMovements",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HandlingEvents_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Cargos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackingId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliverySpecificationId = table.Column<int>(type: "int", nullable: true),
                    DeliveryHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Size = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AssignedItineraryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cargos_DeliveryHistory_DeliveryHistoryId",
                        column: x => x.DeliveryHistoryId,
                        principalTable: "DeliveryHistory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cargos_DeliverySpecification_DeliverySpecificationId",
                        column: x => x.DeliverySpecificationId,
                        principalTable: "DeliverySpecification",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Cargos_Itinerary_AssignedItineraryId",
                        column: x => x.AssignedItineraryId,
                        principalTable: "Itinerary",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_AssignedItineraryId",
                table: "Cargos",
                column: "AssignedItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_DeliveryHistoryId",
                table: "Cargos",
                column: "DeliveryHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_DeliverySpecificationId",
                table: "Cargos",
                column: "DeliverySpecificationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarrierMovements_FromId",
                table: "CarrierMovements",
                column: "FromId");

            migrationBuilder.CreateIndex(
                name: "IX_CarrierMovements_ToId",
                table: "CarrierMovements",
                column: "ToId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliverySpecification_DestinationId",
                table: "DeliverySpecification",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_HandlingEvents_CarrierMovementId",
                table: "HandlingEvents",
                column: "CarrierMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_HandlingEvents_LocationId",
                table: "HandlingEvents",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Leg_ItineraryId",
                table: "Leg",
                column: "ItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_Leg_LoadLocationId",
                table: "Leg",
                column: "LoadLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Leg_UnloadLocationId",
                table: "Leg",
                column: "UnloadLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cargos");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "HandlingEvents");

            migrationBuilder.DropTable(
                name: "Leg");

            migrationBuilder.DropTable(
                name: "DeliveryHistory");

            migrationBuilder.DropTable(
                name: "DeliverySpecification");

            migrationBuilder.DropTable(
                name: "CarrierMovements");

            migrationBuilder.DropTable(
                name: "Itinerary");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
