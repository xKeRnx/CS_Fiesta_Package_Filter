using System;

namespace FilterLib
{
    internal class Money
    {
        public static string ConvertMoney(long Amount)
        {
            string AmountString = Amount.ToString();
            Char[] AmountChars = AmountString.ToCharArray();

            if (AmountString.Length == 1) { return string.Format("{0} Copper", Amount); }
            else if (AmountString.Length == 2) { return string.Format("{0} Copper", Amount); }
            else if (AmountString.Length == 3) { return string.Format("{0} Copper", Amount); }
            else if (AmountString.Length == 4) { return string.Format("{0} Silver {1}{2}{3} Copper", AmountChars[0], AmountChars[1], AmountChars[2], AmountChars[3]); }
            else if (AmountString.Length == 5) { return string.Format("{0}{1} Silver {2}{3}{4} Copper", AmountChars[0], AmountChars[1], AmountChars[2], AmountChars[3], AmountChars[4]); }
            else if (AmountString.Length == 6) { return string.Format("{0}{1}{2} Silver {3}{4}{5} Copper", AmountChars[0], AmountChars[1], AmountChars[2], AmountChars[3], AmountChars[4], AmountChars[5]); }
            else if (AmountString.Length == 7) { return string.Format("{0} Gold {1}{2}{3} Silver {4}{5}{6} Copper", AmountChars[0], AmountChars[1], AmountChars[2], AmountChars[3], AmountChars[4], AmountChars[5], AmountChars[6]); }
            else if (AmountString.Length == 8) { return string.Format("{0}{1} Gold {2}{3}{4} Silver {5}{6}{7} Copper", AmountChars[0], AmountChars[1], AmountChars[2], AmountChars[3], AmountChars[4], AmountChars[5], AmountChars[6], AmountChars[7]); }
            else if (AmountString.Length == 9) { return string.Format("{0} Gem {1}{2} Gold {3}{4}{5} Silver {6}{7}{8} Copper", AmountChars[0], AmountChars[1], AmountChars[2], AmountChars[3], AmountChars[4], AmountChars[5], AmountChars[6], AmountChars[7], AmountChars[8]); }
            else if (AmountString.Length == 10) { return string.Format("{0}{1} Gem {2}{3} Gold {4}{5}{6} Silver {7}{8}{9} Copper", AmountChars[0], AmountChars[1], AmountChars[2], AmountChars[3], AmountChars[4], AmountChars[5], AmountChars[6], AmountChars[7], AmountChars[8], AmountChars[9]); }
            else if (AmountString.Length == 11) { return string.Format("{0}{2}{3} Gem {4}{5} Gold {6}{7}{8} Silver {9}{10}{11} Copper", AmountChars[0], AmountChars[1], AmountChars[2], AmountChars[3], AmountChars[4], AmountChars[5], AmountChars[6], AmountChars[7], AmountChars[8], AmountChars[9], AmountChars[10], AmountChars[11]); }
            else if (AmountString.Length == 12) { return string.Format("{0}{2}{3}{4} Gem {5}{6} Gold {7}{8}{9} Silver {10}{11}{12} Copper", AmountChars[0], AmountChars[1], AmountChars[2], AmountChars[3], AmountChars[4], AmountChars[5], AmountChars[6], AmountChars[7], AmountChars[8], AmountChars[9], AmountChars[10], AmountChars[11], AmountChars[12]); }

            return "TOO MUCH MONEY";
        }
    }
}
