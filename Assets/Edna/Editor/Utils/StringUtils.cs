using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JamesOR.Edna.Utils
{
    public class StringUtils
    {
        public static string IncrementName(string name, List<string> list)
        {
            int counter = 0;
            string expr = $@"({name})(\((\d*)\))?";

            foreach (var s in list)
            {
                Match match = Regex.Match(s, expr);
                if (match.Success)
                {
                    string v = match.Groups[3].Value;
                    if (v == "")
                    {
                        counter = 1;
                    }
                    else
                    {
                        int num = int.Parse(v);
                        counter = Math.Max(counter, num) + 1;
                    }
                }
            }
            if (counter > 0)
            {
                return $"{name}({counter})";
            }

            return name;
        }
    }
}
