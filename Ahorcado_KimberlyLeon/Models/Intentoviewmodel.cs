using System.ComponentModel.DataAnnotations;

namespace Ahorcado_KimberlyLeon.Models
{
    public class IntentoViewModel
    {
        [Required(ErrorMessage = "Ingresa una letra.")]
        [RegularExpression("^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]$",
            ErrorMessage = "Solo se permite UNA letra (incluye tildes y ñ).")]
        public string Letra { get; set; }
    }
}
