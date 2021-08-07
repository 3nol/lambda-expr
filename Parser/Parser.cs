using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using lambda_cs.Components;

namespace lambda_cs.Parser
{
    static class Parser
    {
        public static LExpr Parse(string expr)
        {
            expr = expr.Trim();
            if (!"".Equals(expr))
            {
                if (expr.StartsWith('\\'))
                {
                    var vars = new List<char>();
                    var k = 1;
                    while (!'.'.Equals(expr[k]))
                    {
                        if (!' '.Equals(expr[k]))
                        {
                            vars.Add(expr[k]);
                        }
                        k++;
                    }
                    LExpr agg = Parse(expr.Substring(k + 1));
                    for (var i = vars.Count - 1; i >= 0; i--)
                    {
                        agg = new Lambda(vars[i], agg);
                    }
                    return agg;
                } 
                else if (!new Regex("\\(|\\)|\\\\|\\.").IsMatch(expr))
                {
                    var parts = expr.Split(" ");
                    LExpr agg = ParseTerminal(parts[0]);
                    for (var i = 1; i < parts.Length; i++)
                    {
                        agg = new Application(agg, ParseTerminal(parts[i]));
                    }
                    return agg;
                }
                else
                {
                    var parts = LevelSplit(expr);
                    LExpr agg = Parse(parts[0]);
                    for (var i = 1; i < parts.Count; i++)
                    {
                        agg = new Application(agg, Parse(parts[i]));
                    }
                    return agg;
                }
            }
            return null;
        }

        private static List<string> LevelSplit(string str)
        {
            var parts = new List<string>();
            var level = 0;
            var current = "";
            for (var i = 0; i < str.Length; i++)
            {
                if (level == 0)
                {
                    if ('('.Equals(str[i]))
                    {
                        parts.Add(current.Trim());
                        current = "";
                        level++;
                    }
                    else
                    {
                        current += str[i];
                    }
                }
                else
                {
                    if ('('.Equals(str[i]))
                    {
                        current += str[i];
                        level++;
                    }
                    else if (')'.Equals(str[i]))
                    {
                        if (level == 1)
                        {
                            parts.Add(current.Trim());
                            current = "";
                            level--;
                        }
                        else
                        {
                            current += str[i];
                            level--;
                        }
                    }
                    else
                    {
                        current += str[i];
                    }
                }
            }
            parts.Add(current.Trim());
            parts.RemoveAll(item => "".Equals(item));
            return parts;
        }

        private static LExpr ParseTerminal(string expr)
        {
            if (new Regex("\\+|-|\\*|/|=|\\d+|'.+'").IsMatch(expr))
            {
                return new Constant(expr);
            }
            else
            {
                return new Variable(expr[0]);
            }
        }
    }
}
