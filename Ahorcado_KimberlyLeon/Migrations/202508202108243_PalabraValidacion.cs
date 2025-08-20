namespace Ahorcado_KimberlyLeon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PalabraValidacion : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Palabras", "Texto", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Palabras", "Texto", c => c.String());
        }
    }
}
