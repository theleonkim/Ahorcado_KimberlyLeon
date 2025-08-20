using System.Data.Entity.Migrations;

namespace Ahorcado_KimberlyLeon.Migrations
{
    public partial class PalabraConstraints : DbMigration
    {
        public override void Up()
        {
            // Sanea datos conflictivos
            Sql(@"
                UPDATE dbo.Palabras
                SET Texto = NULL
                WHERE LTRIM(RTRIM(ISNULL(Texto, ''))) = ''
            ");

            Sql(@"DELETE FROM dbo.Palabras WHERE Texto LIKE '%[0-9]%'");

            // Aplica restricciones
            AlterColumn("dbo.Palabras", "Texto", c => c.String(nullable: false, maxLength: 40));
        }

        public override void Down()
        {
            AlterColumn("dbo.Palabras", "Texto", c => c.String());
        }
    }
}
