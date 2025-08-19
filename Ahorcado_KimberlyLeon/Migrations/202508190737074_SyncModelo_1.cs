namespace Ahorcado_KimberlyLeon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncModelo_1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Partidas", "Dificultad", c => c.Int(nullable: false));
            AddColumn("dbo.Partidas", "Ganada", c => c.Boolean(nullable: false));
            AddColumn("dbo.Partidas", "IntentosFallidos", c => c.Int(nullable: false));
            AddColumn("dbo.Partidas", "FechaInicio", c => c.DateTime(nullable: false));
            AddColumn("dbo.Partidas", "FechaFin", c => c.DateTime());
            DropColumn("dbo.Partidas", "Nivel");
            DropColumn("dbo.Partidas", "InicioUtc");
            DropColumn("dbo.Partidas", "FinUtc");
            DropColumn("dbo.Partidas", "IntentosRestantes");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Partidas", "IntentosRestantes", c => c.Int(nullable: false));
            AddColumn("dbo.Partidas", "FinUtc", c => c.DateTime());
            AddColumn("dbo.Partidas", "InicioUtc", c => c.DateTime(nullable: false));
            AddColumn("dbo.Partidas", "Nivel", c => c.Int(nullable: false));
            DropColumn("dbo.Partidas", "FechaFin");
            DropColumn("dbo.Partidas", "FechaInicio");
            DropColumn("dbo.Partidas", "IntentosFallidos");
            DropColumn("dbo.Partidas", "Ganada");
            DropColumn("dbo.Partidas", "Dificultad");
        }
    }
}
