using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ahorcado_KimberlyLeon.Models
{
    public class Jugador
    {
        [Key]
        public int Id { get; set; }                 // PK interna (autoincremental)

        [Required, MaxLength(80)]
        public string Nombre { get; set; }

        // >>> ID que ingresa la persona (mostrar en UI)
        [Required(ErrorMessage = "El ID es obligatorio.")]
        [MaxLength(30)]
        [RegularExpression(@"^\d{4,30}$", ErrorMessage = "El ID debe ser numérico (4–30 dígitos).")]
        public string Codigo { get; set; }          // ← aquí guardarás 117580701, etc.

        public int GanadasFacil { get; set; }
        public int GanadasNormal { get; set; }
        public int GanadasDificil { get; set; }

        public int PerdidasFacil { get; set; }
        public int PerdidasNormal { get; set; }
        public int PerdidasDificil { get; set; }

        [NotMapped]
        public int Marcador =>
            (GanadasFacil * 1) + (GanadasNormal * 2) + (GanadasDificil * 3)
          - (PerdidasFacil * 1 + PerdidasNormal * 2 + PerdidasDificil * 3);
    }
}
