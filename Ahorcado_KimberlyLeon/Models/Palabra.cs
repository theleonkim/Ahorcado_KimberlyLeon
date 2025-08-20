using System.ComponentModel.DataAnnotations;

namespace Ahorcado_KimberlyLeon.Models
{
    public class Palabra
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La palabra es obligatoria.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ]+$", ErrorMessage = "La palabra solo puede contener letras.")]
        public string Texto { get; set; }

        public bool Usada { get; set; }
    }
}
