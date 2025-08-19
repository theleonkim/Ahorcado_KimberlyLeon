using System.ComponentModel.DataAnnotations;

namespace Ahorcado_KimberlyLeon.Models
{
    public class Palabra
    {
        [Key]
        public int Id { get; set; }
        public string Texto { get; set; }
        public bool Usada { get; set; }
    }
}