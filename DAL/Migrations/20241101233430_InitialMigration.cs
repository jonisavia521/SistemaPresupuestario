using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Familia",
                columns: table => new
                {
                    IdFamilia = table.Column<Guid>(unicode: false, maxLength: 36, nullable: false),
                    Nombre = table.Column<string>(unicode: false, maxLength: 1000, nullable: true),
                    timestamp = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Familia__751F80CFFEC2F220", x => x.IdFamilia);
                });

            migrationBuilder.CreateTable(
                name: "Patente",
                columns: table => new
                {
                    IdPatente = table.Column<Guid>(unicode: false, maxLength: 36, nullable: false),
                    Nombre = table.Column<string>(unicode: false, maxLength: 1000, nullable: true),
                    Vista = table.Column<string>(unicode: false, maxLength: 1000, nullable: true),
                    timestamp = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Patente__9F4EF95C34290DD0", x => x.IdPatente);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false, defaultValueSql: "(newsequentialid())"),
                    Codigo = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Estado = table.Column<int>(nullable: true),
                    FechaAlta = table.Column<DateTime>(type: "datetime", nullable: true),
                    UsuarioAlta = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false, defaultValueSql: "(newsequentialid())"),
                    CodigoAFIP = table.Column<string>(unicode: false, maxLength: 2, nullable: true),
                    Nombre = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TipoImpuesto",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false, defaultValueSql: "(newsequentialid())"),
                    Descripcion = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Definible = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoImpuesto", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<Guid>(unicode: false, maxLength: 36, nullable: false),
                    Nombre = table.Column<string>(unicode: false, maxLength: 1000, nullable: true),
                    timestamp = table.Column<byte[]>(rowVersion: true, nullable: false),
                    Usuario = table.Column<string>(unicode: false, maxLength: 20, nullable: false, defaultValueSql: "('')"),
                    Clave = table.Column<string>(unicode: false, maxLength: 50, nullable: true, defaultValueSql: "('')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuario__5B65BF970075BCE6", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Vendedor",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false, defaultValueSql: "(newsequentialid())"),
                    Nombre = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Mail = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Direccion = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Telefono = table.Column<string>(unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendedor", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Familia_Familia",
                columns: table => new
                {
                    IdFamilia = table.Column<Guid>(unicode: false, maxLength: 36, nullable: false),
                    IdFamiliaHijo = table.Column<Guid>(unicode: false, maxLength: 36, nullable: false),
                    timestamp = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Familia___ABFCC67E1F707C2E", x => new { x.IdFamilia, x.IdFamiliaHijo });
                    table.ForeignKey(
                        name: "FK__Familia_F__IdFam__7C4F7684",
                        column: x => x.IdFamilia,
                        principalTable: "Familia",
                        principalColumn: "IdFamilia",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Familia_A__Famil__37A5467C",
                        column: x => x.IdFamiliaHijo,
                        principalTable: "Familia",
                        principalColumn: "IdFamilia",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Familia_Patente",
                columns: table => new
                {
                    IdFamilia = table.Column<Guid>(unicode: false, maxLength: 36, nullable: false),
                    IdPatente = table.Column<Guid>(unicode: false, maxLength: 36, nullable: false),
                    timestamp = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__FamiliaE__166FEEA61367E606", x => new { x.IdFamilia, x.IdPatente });
                    table.ForeignKey(
                        name: "FK_Familia_Patente_Familia",
                        column: x => x.IdFamilia,
                        principalTable: "Familia",
                        principalColumn: "IdFamilia",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FamiliaElement_Patente",
                        column: x => x.IdPatente,
                        principalTable: "Patente",
                        principalColumn: "IdPatente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Impuesto",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false, defaultValueSql: "(newsequentialid())"),
                    Codigo = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    Alicuota = table.Column<double>(nullable: false),
                    IdProvincia = table.Column<Guid>(nullable: true),
                    IdTipoImpuesto = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Impuesto", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Impuesto_Provincia",
                        column: x => x.IdProvincia,
                        principalTable: "Provincia",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Impuesto_TipoImpuesto",
                        column: x => x.IdTipoImpuesto,
                        principalTable: "TipoImpuesto",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuario_Familia",
                columns: table => new
                {
                    IdUsuario = table.Column<Guid>(unicode: false, maxLength: 36, nullable: false),
                    IdFamilia = table.Column<Guid>(unicode: false, maxLength: 36, nullable: false),
                    timestamp = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Usuario___BC34479B87709AFE", x => new { x.IdUsuario, x.IdFamilia });
                    table.ForeignKey(
                        name: "FK__Usuario_P__Famil__35BCFE0A",
                        column: x => x.IdFamilia,
                        principalTable: "Familia",
                        principalColumn: "IdFamilia",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Usuario_F__IdUsu__7F2BE32F",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuario_Patente",
                columns: table => new
                {
                    IdUsuario = table.Column<Guid>(unicode: false, maxLength: 36, nullable: false),
                    IdPatente = table.Column<Guid>(unicode: false, maxLength: 36, nullable: false),
                    timestamp = table.Column<byte[]>(rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario_Patente", x => new { x.IdUsuario, x.IdPatente });
                    table.ForeignKey(
                        name: "FK_Usuario_Patente_Patente",
                        column: x => x.IdPatente,
                        principalTable: "Patente",
                        principalColumn: "IdPatente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuario_Patente_Usuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false, defaultValueSql: "(newsequentialid())"),
                    CodigoCliente = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    RazonSocial = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    IdProvincia = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Localidad = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    DireccionLegal = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    DireccionComercial = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    CUIT = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    IdVendedor = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cliente",
                        column: x => x.IdVendedor,
                        principalTable: "Vendedor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cliente_Impuesto",
                columns: table => new
                {
                    IdImpuesto = table.Column<Guid>(nullable: false),
                    IdTipoImpuesto = table.Column<Guid>(nullable: false),
                    IdCliente = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente_Impuesto", x => new { x.IdImpuesto, x.IdTipoImpuesto, x.IdCliente });
                    table.ForeignKey(
                        name: "FK_Cliente_Impuesto_Impuesto",
                        column: x => x.IdImpuesto,
                        principalTable: "Impuesto",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cliente_Impuesto_TipoImpuesto",
                        column: x => x.IdTipoImpuesto,
                        principalTable: "TipoImpuesto",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comprobantes",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false, defaultValueSql: "(newsequentialid())"),
                    IdVendedor = table.Column<Guid>(nullable: true),
                    IdCliente = table.Column<Guid>(nullable: true),
                    Estado = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime", nullable: true),
                    TipoComprobante = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    NumeroComprobante = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    ImporteGravado = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    ImporteIva = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comprobantes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Comprobantes_Cliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comprobantes_Vendedor",
                        column: x => x.IdVendedor,
                        principalTable: "Vendedor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Presupuesto",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false, defaultValueSql: "(newsequentialid())"),
                    Numero = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    IdCliente = table.Column<Guid>(nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime", nullable: false),
                    Estado = table.Column<int>(nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime", nullable: true),
                    IdPresupuestoPadre = table.Column<Guid>(nullable: true),
                    IdVendedor = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presupuesto", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Presupuesto_Cliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Presupuesto_Presupuesto",
                        column: x => x.IdPresupuestoPadre,
                        principalTable: "Presupuesto",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Presupuesto_Vendedor",
                        column: x => x.IdVendedor,
                        principalTable: "Vendedor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comprobante_Detalle",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false, defaultValueSql: "(newsequentialid())"),
                    IdComprobante = table.Column<Guid>(nullable: false),
                    TipoComprobante = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    NumeroComprobante = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    IdProducto = table.Column<Guid>(nullable: true),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime", nullable: true),
                    Renglon = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    Cantidad = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    ImporteNeto = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    PrecioNeto = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comprobante_Detalle", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Comprobante_Detalle_Comprobantes",
                        column: x => x.IdComprobante,
                        principalTable: "Comprobantes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comprobante_Detalle_Producto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Presupuesto_Detalle",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false, defaultValueSql: "(newsequentialid())"),
                    Numero = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    IdPresupuesto = table.Column<Guid>(nullable: true),
                    IdProducto = table.Column<Guid>(nullable: true),
                    Cantidad = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    Descuento = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    Renglon = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presupuesto_Detalle", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Presupuesto_Detalle_Presupuesto",
                        column: x => x.IdPresupuesto,
                        principalTable: "Presupuesto",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Presupuesto_Detalle_Producto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_IdVendedor",
                table: "Cliente",
                column: "IdVendedor");

            migrationBuilder.CreateIndex(
                name: "IXFK_Cliente_Impuesto_Cliente",
                table: "Cliente_Impuesto",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IXFK_Cliente_Impuesto_Impuesto",
                table: "Cliente_Impuesto",
                column: "IdImpuesto");

            migrationBuilder.CreateIndex(
                name: "IXFK_Cliente_Impuesto_TipoImpuesto",
                table: "Cliente_Impuesto",
                column: "IdTipoImpuesto");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_Detalle_IdComprobante",
                table: "Comprobante_Detalle",
                column: "IdComprobante");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobante_Detalle_IdProducto",
                table: "Comprobante_Detalle",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobantes_IdCliente",
                table: "Comprobantes",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Comprobantes_IdVendedor",
                table: "Comprobantes",
                column: "IdVendedor");

            migrationBuilder.CreateIndex(
                name: "IX_Familia_Familia_IdFamiliaHijo",
                table: "Familia_Familia",
                column: "IdFamiliaHijo");

            migrationBuilder.CreateIndex(
                name: "IX_Familia_Patente_IdPatente",
                table: "Familia_Patente",
                column: "IdPatente");

            migrationBuilder.CreateIndex(
                name: "IXFK_Impuesto_Provincia",
                table: "Impuesto",
                column: "IdProvincia");

            migrationBuilder.CreateIndex(
                name: "IXFK_Impuesto_TipoImpuesto",
                table: "Impuesto",
                column: "IdTipoImpuesto");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuesto_IdCliente",
                table: "Presupuesto",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuesto_IdPresupuestoPadre",
                table: "Presupuesto",
                column: "IdPresupuestoPadre");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuesto_IdVendedor",
                table: "Presupuesto",
                column: "IdVendedor");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuesto_Detalle_IdPresupuesto",
                table: "Presupuesto_Detalle",
                column: "IdPresupuesto");

            migrationBuilder.CreateIndex(
                name: "IX_Presupuesto_Detalle_IdProducto",
                table: "Presupuesto_Detalle",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Familia_IdFamilia",
                table: "Usuario_Familia",
                column: "IdFamilia");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Patente_IdPatente",
                table: "Usuario_Patente",
                column: "IdPatente");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cliente_Impuesto");

            migrationBuilder.DropTable(
                name: "Comprobante_Detalle");

            migrationBuilder.DropTable(
                name: "Familia_Familia");

            migrationBuilder.DropTable(
                name: "Familia_Patente");

            migrationBuilder.DropTable(
                name: "Presupuesto_Detalle");

            migrationBuilder.DropTable(
                name: "Usuario_Familia");

            migrationBuilder.DropTable(
                name: "Usuario_Patente");

            migrationBuilder.DropTable(
                name: "Impuesto");

            migrationBuilder.DropTable(
                name: "Comprobantes");

            migrationBuilder.DropTable(
                name: "Presupuesto");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Familia");

            migrationBuilder.DropTable(
                name: "Patente");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Provincia");

            migrationBuilder.DropTable(
                name: "TipoImpuesto");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Vendedor");
        }
    }
}
