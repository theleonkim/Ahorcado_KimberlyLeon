using Ahorcado_KimberlyLeon.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ahorcado_KimberlyLeon.Models
{
    public class Partida
    {
        [Key]
        public int Id { get; set; }

        // ---------- FKs esperadas por tu código ----------
        public int JugadorId { get; set; }
        public int PalabraId { get; set; }

        // ---------- Dificultad (enum, NO string) ----------
        public Dificultad Dificultad { get; set; } = Dificultad.Normal;

        // ---------- Estado de juego ----------
        public bool Ganada { get; set; } = false;
        public int IntentosFallidos { get; set; } = 0;
        public EstadoPartida Estado { get; set; } = EstadoPartida.EnCurso;

        /// <summary>Letras probadas persistidas como "a,b,c".</summary>
        public string LetrasProbadas { get; set; } = string.Empty;

        // ---------- Fechas ----------
        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
        public DateTime? FechaFin { get; set; }

        // ---------- Navegaciones (opcional según tu modelo) ----------
        [ForeignKey(nameof(JugadorId))]
        public Jugador Jugador { get; set; }

        [ForeignKey(nameof(PalabraId))]
        public Palabra Palabra { get; set; }

        // ---------- Reglas del enunciado ----------
        [NotMapped]
        public int MaxIntentos => 5; // fijo

        [NotMapped]
        public int IntentosRestantes => Math.Max(0, MaxIntentos - IntentosFallidos);

        [NotMapped]
        public TimeSpan TiempoMaximo => GetTiempoMaximo(Dificultad);

        // ---------- Wrappers de compatibilidad ----------
        [NotMapped]
        public Dificultad Nivel
        {
            get => Dificultad;
            set => Dificultad = value;
        }

        // Texto <-> enum (si en algún lado siguen tratando Dificultad como string)
        [NotMapped]
        public string DificultadTexto
        {
            get => Dificultad.ToString(); // "Facil","Normal","Dificil"
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                if (Enum.TryParse(value.Trim(), true, out Dificultad parsed))
                {
                    Dificultad = parsed;
                    return;
                }
                var n = (value ?? "").Trim().ToLower();
                if (n.Contains("facil")) Dificultad = Dificultad.Facil;
                else if (n.Contains("dific")) Dificultad = Dificultad.Dificil;
                else Dificultad = Dificultad.Normal;
            }
        }

        // Lista de letras con Split/Join compatibles
        [NotMapped]
        public List<char> LetrasProbadasLista
        {
            get
            {
                if (string.IsNullOrWhiteSpace(LetrasProbadas))
                    return new List<char>();

                var tokens = LetrasProbadas.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var lista = new List<char>();
                foreach (var t in tokens)
                {
                    var s = t.Trim();
                    if (!string.IsNullOrEmpty(s)) lista.Add(s[0]);
                }
                return lista;
            }
            set
            {
                if (value == null || value.Count == 0) { LetrasProbadas = string.Empty; return; }
                var strings = new List<string>(value.Count);
                foreach (var c in value) strings.Add(c.ToString());
                LetrasProbadas = string.Join(",", strings);
            }
        }

        // Wrappers UTC si tu código los invoca
        [NotMapped]
        public DateTime InicioUtc
        {
            get => FechaInicio.Kind == DateTimeKind.Utc ? FechaInicio : FechaInicio.ToUniversalTime();
            set => FechaInicio = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        [NotMapped]
        public DateTime? FinUtc
        {
            get
            {
                if (!FechaFin.HasValue) return null;
                return FechaFin.Value.Kind == DateTimeKind.Utc ? FechaFin : FechaFin.Value.ToUniversalTime();
            }
            set => FechaFin = value.HasValue ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : (DateTime?)null;
        }

        // ---------- Helpers ----------
        public static TimeSpan GetTiempoMaximo(Dificultad d)
        {
            switch (d)
            {
                case Dificultad.Facil: return TimeSpan.FromSeconds(90); // 1:30
                case Dificultad.Normal: return TimeSpan.FromSeconds(60); // 1:00
                case Dificultad.Dificil: return TimeSpan.FromSeconds(30); // 0:30
                default: return TimeSpan.FromSeconds(60);
            }
        }


        public static int GetDeltaPuntaje(bool ganada, Dificultad d)
        {
            var basePts = (d == Dificultad.Facil) ? 1 : (d == Dificultad.Normal ? 2 : 3);
            return ganada ? basePts : -basePts;
        }
    }
}




