using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Ahorcado_KimberlyLeon.Data;
using Ahorcado_KimberlyLeon.Models;

namespace Ahorcado_KimberlyLeon.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AhorcadoContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;               // ok en dev
            AutomaticMigrationDataLossAllowed = true;        // ok en dev
        }

        protected override void Seed(AhorcadoContext ctx)
        {
            // Solo sembrar si hay menos de 100
            if (ctx.Palabras.Count() >= 100) return;

            var lista = new List<string>
            {
                // A
                "árbol","amigo","avión","arena","anillo","actor","almas","amable",
                // B
                "barco","bebida","bello","broma","brisa","bosque","botón","bailar",
                // C
                "café","cielo","cinta","coche","cobre","costa","corto","cesta",
                // D
                "dado","densa","dieta","dólar","duende","dulce","durar","deber",
                // E
                "eco","edita","enano","enojo","época","equipo","estrella","ensayo",
                // F
                "fácil","falda","fuego","futuro","firme","ficha","fondo","fruta",
                // G
                "gafas","ganar","gente","gripe","golpe","grupo","guante","grano",
                // H
                "hielo","hogar","huevo","humor","hojas","horno","héroe","honda",
                // I
                "iglesia","imagen","imán","isleta","invierno","índice","idea","igual",
                // J
                "jamón","juego","joven","joyas","junio","julio","jirafa","jardín",
                // K
                "kárate","koala","kiosco","karma",
                // L
                "lápiz","latir","lento","libro","lindo","luzca","llaves","llanto",
                // M
                "marea","mañana","mano","miedo","móvil","mundo","museo","madre",
                // N
                "nacer","nieve","niñez","noble","noche","nube","nutria","nueve",
                // Ñ
                "ñoqui","ñandú","ñuble",
                // O
                "oasis","océano","obra","ojera","olear","orquídea","oro","oscuro",
                // P
                "pájaro","palma","papel","pared","pereza","perfil","perla","plato",
                // Q
                "queso","quinto","quitar","químico",
                // R
                "rápido","ramas","ratón","reloj","río","rueda","rumbo","rubro",
                // S
                "salud","sabor","siglo","silla","suelo","sueño","sello","sonar",
                // T
                "tarde","taza","techo","temor","tesla","tigre","tocar","túnel",
                // U
                "útero","uña","unión","urbano","urnas","usar","útil","uvula",
                // V
                "vapor","valor","valla","verde","viaje","viejo","vigor","votar",
                // W (permitimos extranjerismos cortos)
                "whisky","wifi","wafle",
                // X (difícil en español; válidos y cortos)
                "xenón","xilófono","xolo",
                // Y
                "yacer","yate","yema","yerba","yogur","yunque",
                // Z
                "zorro","zumba","zafra","zarza","zonas","zapato","zócalo","zueco"
            };

            // Normalizar duplicados por tildes/mayúsculas
            var existentes = new HashSet<string>(
                ctx.Palabras.Select(p => p.Texto.ToLower()).ToList()
            );

            var nuevas = lista
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .Where(t => !existentes.Contains(t.ToLower()))
                .Distinct()
                .Select(t => new Palabra { Texto = t, Usada = false })
                .ToList();

            if (nuevas.Count > 0)
            {
                ctx.Palabras.AddRange(nuevas);
                ctx.SaveChanges();
            }
        }
    }
}
