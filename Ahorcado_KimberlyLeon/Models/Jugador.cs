using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ahorcado_KimberlyLeon.Models
{
    public class Jugador
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }

        public int GanadasFacil { get; set; }
        public int GanadasNormal { get; set; }
        public int GanadasDificil { get; set; }

        public int PerdidasFacil { get; set; }
        public int PerdidasNormal { get; set; }
        public int PerdidasDificil { get; set; }

        [NotMapped]
        public int Marcador =>
            (GanadasFacil * 1) + (GanadasNormal * 2) + (GanadasDificil * 3)
            - ((PerdidasFacil * 1) + (PerdidasNormal * 2) + (PerdidasDificil * 3));
    }
}