namespace Ahorcado_KimberlyLeon.Models
{
    public class JugadorEscalafonVM
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Marcador { get; set; }

        public int GanadasFacil { get; set; }
        public int GanadasNormal { get; set; }
        public int GanadasDificil { get; set; }
        public int PerdidasFacil { get; set; }
        public int PerdidasNormal { get; set; }
        public int PerdidasDificil { get; set; }
    }
}
