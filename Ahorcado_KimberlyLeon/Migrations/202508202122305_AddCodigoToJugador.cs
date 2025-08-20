namespace Ahorcado_KimberlyLeon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCodigoToJugador : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jugadores", "Codigo", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.Jugadores", "Nombre", c => c.String(nullable: false, maxLength: 80));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Jugadores", "Nombre", c => c.String());
            DropColumn("dbo.Jugadores", "Codigo");
        }
    }
}
