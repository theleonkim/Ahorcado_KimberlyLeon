using System;                       // DateTime, Math (si lo usas)
using System.Globalization;         // CharUnicodeInfo, UnicodeCategory
using System.Text;                  // StringBuilder, NormalizationForm

namespace Ahorcado_KimberlyLeon.Services
{
    public static class TextoHelper
    {
        // Normaliza a MAYÚSCULAS, quita tildes y conserva Ñ
        public static string Norm(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;

            // Preservar Ñ/ñ antes de descomponer
            var tmp = s.Trim()
                       .Replace("ñ", "__ENYE__")
                       .Replace("Ñ", "__ENYE__")
                       .ToUpperInvariant();

            var formD = tmp.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(formD.Length);

            foreach (var ch in formD)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }

            var sinTildes = sb.ToString().Normalize(NormalizationForm.FormC);
            return sinTildes.Replace("__ENYE__", "Ñ");
        }

        // ¿La palabra está completa con las letras ya probadas?
        public static bool EstaCompleta(string palabraOriginal, string letrasProbadas)
        {
            var palabra = Norm(palabraOriginal);
            var letras = Norm(letrasProbadas ?? "");

            foreach (var ch in palabra)
            {
                if (char.IsLetter(ch) && !letras.Contains(ch.ToString()))
                    return false;
            }
            return true;
        }

        // Muestra la palabra con guiones bajos y letras acertadas (para la vista)
        public static string Enmascarar(string palabraOriginal, string letrasProbadas)
        {
            var palabra = Norm(palabraOriginal);
            var letras = Norm(letrasProbadas ?? "");
            var sb = new StringBuilder();

            foreach (var ch in palabra)
            {
                if (!char.IsLetter(ch))
                    sb.Append(ch);
                else
                    sb.Append(letras.Contains(ch.ToString()) ? ch : '_');

                sb.Append(' ');
            }
            return sb.ToString().TrimEnd();
        }

        internal static string NormalizarJuego(string s)
        {
            throw new NotImplementedException();
        }
    }
}
