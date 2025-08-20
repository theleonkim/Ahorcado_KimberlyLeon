namespace Ahorcado_KimberlyLeon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Jugadores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                        GanadasFacil = c.Int(nullable: false),
                        GanadasNormal = c.Int(nullable: false),
                        GanadasDificil = c.Int(nullable: false),
                        PerdidasFacil = c.Int(nullable: false),
                        PerdidasNormal = c.Int(nullable: false),
                        PerdidasDificil = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Palabras",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Texto = c.String(),
                        Usada = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Partidas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        JugadorId = c.Int(nullable: false),
                        PalabraId = c.Int(nullable: false),
                        Dificultad = c.Int(nullable: false),
                        Ganada = c.Boolean(nullable: false),
                        IntentosFallidos = c.Int(nullable: false),
                        Estado = c.Int(nullable: false),
                        LetrasProbadas = c.String(),
                        FechaInicio = c.DateTime(nullable: false),
                        FechaFin = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Jugadores", t => t.JugadorId, cascadeDelete: true)
                .ForeignKey("dbo.Palabras", t => t.PalabraId, cascadeDelete: true)
                .Index(t => t.JugadorId)
                .Index(t => t.PalabraId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Partidas", "PalabraId", "dbo.Palabras");
            DropForeignKey("dbo.Partidas", "JugadorId", "dbo.Jugadores");
            DropIndex("dbo.Partidas", new[] { "PalabraId" });
            DropIndex("dbo.Partidas", new[] { "JugadorId" });
            DropTable("dbo.Partidas");
            DropTable("dbo.Palabras");
            DropTable("dbo.Jugadores");
        }
    }
}
