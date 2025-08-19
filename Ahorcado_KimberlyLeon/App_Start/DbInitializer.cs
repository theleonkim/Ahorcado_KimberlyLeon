using Ahorcado_KimberlyLeon.Data;
using Ahorcado_KimberlyLeon.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ahorcado_KimberlyLeon.App_Start
{
    public class DbInitializer : DropCreateDatabaseIfModelChanges<AhorcadoContext>
    {
        protected override void Seed(AhorcadoContext context)
        {
            var palabras = new List<Palabra>
            {
                // Palabras con tilde (al menos la mitad de las 100)
                new Palabra { Texto = "árbol", Usada = false },
                new Palabra { Texto = "camión", Usada = false },
                new Palabra { Texto = "cárcel", Usada = false },
                new Palabra { Texto = "corazón", Usada = false },
                new Palabra { Texto = "crédito", Usada = false },
                new Palabra { Texto = "fácil", Usada = false },
                new Palabra { Texto = "lápiz", Usada = false },
                new Palabra { Texto = "público", Usada = false },
                new Palabra { Texto = "reunión", Usada = false },
                new Palabra { Texto = "sofá", Usada = false },
                new Palabra { Texto = "teléfono", Usada = false },
                new Palabra { Texto = "último", Usada = false },
                new Palabra { Texto = "número", Usada = false },
                new Palabra { Texto = "música", Usada = false },
                new Palabra { Texto = "oxígeno", Usada = false },
                new Palabra { Texto = "también", Usada = false },
                new Palabra { Texto = "césped", Usada = false },
                new Palabra { Texto = "murciélago", Usada = false },
                new Palabra { Texto = "práctico", Usada = false },
                new Palabra { Texto = "fantástico", Usada = false },
                new Palabra { Texto = "biología", Usada = false },
                new Palabra { Texto = "matemática", Usada = false },
                new Palabra { Texto = "histórico", Usada = false },
                new Palabra { Texto = "médico", Usada = false },
                new Palabra { Texto = "química", Usada = false },
                new Palabra { Texto = "física", Usada = false },
                new Palabra { Texto = "teoría", Usada = false },
                new Palabra { Texto = "estadística", Usada = false },
                new Palabra { Texto = "geografía", Usada = false },
                new Palabra { Texto = "filosofía", Usada = false },
                new Palabra { Texto = "lógico", Usada = false },
                new Palabra { Texto = "mágico", Usada = false },
                new Palabra { Texto = "sílaba", Usada = false },
                new Palabra { Texto = "lámpara", Usada = false },
                new Palabra { Texto = "máquina", Usada = false },
                new Palabra { Texto = "cómico", Usada = false },
                new Palabra { Texto = "doméstico", Usada = false },
                new Palabra { Texto = "médula", Usada = false },
                new Palabra { Texto = "pétalo", Usada = false },
                new Palabra { Texto = "tímido", Usada = false },
                new Palabra { Texto = "brócoli", Usada = false },
                new Palabra { Texto = "semáforo", Usada = false },
                new Palabra { Texto = "pólvora", Usada = false },
                new Palabra { Texto = "cálculo", Usada = false },
                new Palabra { Texto = "búho", Usada = false },
                new Palabra { Texto = "fósforo", Usada = false },
                new Palabra { Texto = "mármol", Usada = false },
                new Palabra { Texto = "tórax", Usada = false },
                new Palabra { Texto = "cáncer", Usada = false },
                new Palabra { Texto = "júpiter", Usada = false },
                new Palabra { Texto = "satélite", Usada = false },
                new Palabra { Texto = "béisbol", Usada = false },
                new Palabra { Texto = "kilómetro", Usada = false },
                new Palabra { Texto = "ejército", Usada = false },

                // Palabras sin tilde
                new Palabra { Texto = "calle", Usada = false },
                new Palabra { Texto = "puerta", Usada = false },
                new Palabra { Texto = "ventana", Usada = false },
                new Palabra { Texto = "cielo", Usada = false },
                new Palabra { Texto = "bosque", Usada = false },
                new Palabra { Texto = "rio", Usada = false },
                new Palabra { Texto = "juego", Usada = false },
                new Palabra { Texto = "clase", Usada = false },
                new Palabra { Texto = "sol", Usada = false },
                new Palabra { Texto = "luna", Usada = false },
                new Palabra { Texto = "mesa", Usada = false },
                new Palabra { Texto = "silla", Usada = false },
                new Palabra { Texto = "pluma", Usada = false },
                new Palabra { Texto = "mano", Usada = false },
                new Palabra { Texto = "libro", Usada = false },
                new Palabra { Texto = "casa", Usada = false },
                new Palabra { Texto = "rojo", Usada = false },
                new Palabra { Texto = "verde", Usada = false },
                new Palabra { Texto = "azul", Usada = false },
                new Palabra { Texto = "cafe", Usada = false },
                new Palabra { Texto = "negro", Usada = false },
                new Palabra { Texto = "blanco", Usada = false },
                new Palabra { Texto = "morado", Usada = false },
                new Palabra { Texto = "amarillo", Usada = false },
                new Palabra { Texto = "gris", Usada = false },
                new Palabra { Texto = "plata", Usada = false },
                new Palabra { Texto = "oro", Usada = false },
                new Palabra { Texto = "cobre", Usada = false },
                new Palabra { Texto = "hierro", Usada = false },
                new Palabra { Texto = "acero", Usada = false },
                new Palabra { Texto = "madera", Usada = false },
                new Palabra { Texto = "vidrio", Usada = false },
                new Palabra { Texto = "plastico", Usada = false },
                new Palabra { Texto = "tierra", Usada = false },
                new Palabra { Texto = "fuego", Usada = false },
                new Palabra { Texto = "viento", Usada = false },
                new Palabra { Texto = "cuerpo", Usada = false },
                new Palabra { Texto = "brazo", Usada = false },
                new Palabra { Texto = "pierna", Usada = false },
                new Palabra { Texto = "cabeza", Usada = false },
                new Palabra { Texto = "nariz", Usada = false },
                new Palabra { Texto = "boca", Usada = false },
                new Palabra { Texto = "ojo", Usada = false },
                new Palabra { Texto = "oreja", Usada = false },
                new Palabra { Texto = "dedo", Usada = false },
                new Palabra { Texto = "mano", Usada = false },
                new Palabra { Texto = "pie", Usada = false },
                new Palabra { Texto = "correr", Usada = false },
                new Palabra { Texto = "saltar", Usada = false },
                new Palabra { Texto = "caminar", Usada = false },
                new Palabra { Texto = "dormir", Usada = false },
                new Palabra { Texto = "comer", Usada = false },
                new Palabra { Texto = "beber", Usada = false },
                new Palabra { Texto = "respirar", Usada = false },
                new Palabra { Texto = "pensar", Usada = false },
                new Palabra { Texto = "sentir", Usada = false },
                new Palabra { Texto = "amar", Usada = false },
                new Palabra { Texto = "odiar", Usada = false },
                new Palabra { Texto = "esperar", Usada = false },
                new Palabra { Texto = "buscar", Usada = false }
            };

            // Asegurar que la base de datos se crea con todas las palabras
            palabras.ForEach(p => context.Palabras.Add(p));
            context.SaveChanges();
        }
    }
}