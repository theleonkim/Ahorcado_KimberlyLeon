namespace Ahorcado_KimberlyLeon.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Ahorcado_KimberlyLeon.Data.AhorcadoContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Ahorcado_KimberlyLeon.Data.AhorcadoContext ctx)
        {
            // Normaliza para comparar sin tildes/mayúsculas (pero distinguiendo Ñ)
            System.Func<string, string> Normalizar = s =>
            {
                if (string.IsNullOrWhiteSpace(s)) return string.Empty;
                // preserva Ñ antes de quitar marcas
                s = s.Replace("ñ", "\u0001").Replace("Ñ", "\u0002");
                var formD = s.Normalize(System.Text.NormalizationForm.FormD);
                var sb = new System.Text.StringBuilder(formD.Length);
                for (int i = 0; i < formD.Length; i++)
                {
                    var ch = formD[i];
                    var cat = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);
                    if (cat != System.Globalization.UnicodeCategory.NonSpacingMark) sb.Append(ch);
                }
                return sb.ToString()
                         .Replace("\u0001", "Ñ").Replace("\u0002", "Ñ")
                         .ToUpperInvariant();
            };

            // Palabras ya existentes (para no duplicar)
            var existentesNorm = new HashSet<string>(
                ctx.Palabras.Select(p => p.Texto).AsEnumerable().Select(Normalizar)
            );

            // 120 palabras (5–10 caracteres aprox., con acentos y Ñ)
            var lista = new List<string>
    {
        // A
        "árbol","amigo","avión","arena","anillo","actor","almas","amable","aldea","aceite",
        "archivo","altura","aroma","armaño","astros",
        // B
        "barco","bebida","bello","broma","brisa","bosque","botón","bailar","balcón","bodega",
        "botella","bronce","bruñir","burque","barril",
        // C
        "cielo","cinta","coche","cobre","costa","camino","camisa","campana","canguro","canela",
        "cabina","cálido","cárcel","carbón","caracol","cariño","cartón","cascada","casero","castor",
        // D
        "dados","densa","dieta","dólar","duende","dulce","durar","deber","dedal","deriva",
        "detalles","diseño","disputa","dorado","doquier",
        // E
        "edita","enano","enojo","época","equipo","ensayo","estrella","espejo","esencia","espuma",
        "escudo","estadio","escena","escoba","escrito",
        // F
        "fácil","falda","fuego","futuro","firme","ficha","fondo","fruta","fragor","franco",
        "franja","frío","formato","forjar","fusible",
        // G
        "gafas","ganar","gente","gripe","golpe","grupo","guante","grano","gradas","granja",
        "gritar","guitarra","gusano","góndola","gárgola",
        // H
        "hielo","hogar","huevo","humor","hojas","horno","héroe","honda","hechizo","herrero",
        "honesto","huella","huracán","hosped","hábito",
        // I
        "iglesia","imagen","imanes","isleta","invierno","índice","ideas","igual","ilusión","ignoto",
        // J
        "jamón","juego","joven","joyas","junio","julio","jirafa","jardín","jabato","jadeos",
        // K
        "kárate","koala","kiosco","karma","kirios",
        // L
        "lápiz","latir","lento","libro","lindo","luzca","llaves","llanto","ladera","laguna",
        "lamina","lavado","lección","leñero","librar",
        // M
        "marea","mañana","manos","miedo","móvil","mundo","museo","madre","maleta","mantel",
        "marrón","marfil","melena","menta","molino",
        // N
        "nacer","nieve","niñez","noble","noche","nubes","nutria","nueve","nácar","navega",
        // Ñ
        "ñoqui","ñandú","ñuble","ñuñear","ñandutí",
        // O
        "oasis","océano","obras","ojera","olear","orquídea","oscuro","óvalo","oración","oloroso",
        // P
        "pájaro","palma","papel","pared","pereza","perfil","perla","plato","pincel","pistón",
        "pirata","planeta","pleito","poesía","portón",
        // Q
        "queso","quinto","quitar","químico","quimera","quemado",
        // R
        "rápido","ramas","ratón","reloj","rueda","rumbo","rubro","robles","rocío","roedor",
        "romero","rodaje","rosado","rotura","rumiar",
        // S
        "salud","sabor","siglo","silla","suelo","sueño","sello","sonar","sabana","sabueso",
        "sartén","serpiente","sierra","solapa","sonido",
        // T
        "tarde","tazas","techo","temor","tesla","tigre","tocar","túnel","tabaco","taladro",
        "tapete","tarea","tercio","tijera","torneo",
        // U
        "útero","uñero","unión","urbano","urnas","usaron","útil","úvula","universo","utopía",
        // V
        "vapor","valor","valla","verde","viaje","viejo","vigor","votar","valija","ventana",
        "verano","vestir","vianda","vibrar","volcán",
        // W
        "whisky","wafle","wagner","wolfram","walkman",
        // X
        "xenón","xilófono","xenial","xanto","xerófilo",
        // Y
        "yacer","yemas","yerba","yogur","yunque","yermos","yipeta","yoduro","yesero","yacare",
        // Z
        "zorro","zumba","zafra","zarza","zonas","zapato","zócalo","zueco","zulian","zafiro",
       
    };

            var nuevas = new List<Ahorcado_KimberlyLeon.Models.Palabra>();
            foreach (var raw in lista)
            {
                if (string.IsNullOrWhiteSpace(raw)) continue;
                var t = raw.Trim();
                // si ya existe (ignorando tildes/mayúsculas), no la dupliques
                var key = Normalizar(t);
                if (existentesNorm.Contains(key)) continue;
                existentesNorm.Add(key);
                nuevas.Add(new Ahorcado_KimberlyLeon.Models.Palabra { Texto = t, Usada = false });
            }

            if (nuevas.Count > 0)
            {
                ctx.Palabras.AddRange(nuevas);
                ctx.SaveChanges();
            }
        }

    }
}
