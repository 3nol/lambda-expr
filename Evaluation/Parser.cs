using System.Collections.Generic;
using System.Text.RegularExpressions;
using lambda_cs.Components;

namespace lambda_cs.Evaluation
{
    static class Parser
    {
        // parses a string representation of a lambda expression into a LExpr
        public static LExpr Parse(string expr)
        {
            // removing leading and trailing whitespaces
            expr = expr.Trim();
            if (!"".Equals(expr))
            {
                // parsing lambdas
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
                    // denesting consecutive lambdas,
                    // connecting them from inside out (foldr)
                    LExpr agg = Parse(expr.Substring(k + 1));
                    for (var i = vars.Count - 1; i >= 0; i--)
                    {
                        agg = new Lambda(vars[i], agg);
                    }
                    return agg;
                }
                // parsing applications of variables and constants
                else if (!new Regex("\\(|\\)|\\\\|\\.").IsMatch(expr))
                {
                    var parts = expr.Split(" ");
                    // parsing terminal symbols (variables & constants),
                    // concatenating them with applications (foldl)
                    LExpr agg = ParseTerminal(parts[0]);
                    for (var i = 1; i < parts.Length; i++)
                    {
                        agg = new Application(agg, ParseTerminal(parts[i]));
                    }
                    return agg;
                }
                // parsing applications of anything
                else
                {
                    var parts = LevelSplit(expr);
                    // recursive Parse calls on subexpressions, separated by LevelSplit,
                    // concatenating them with applications (foldl)
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

        // splits an expression at one level, i.e. on parentheses,
        // example: "f (\x. f (x x)) (\x. f (x x))" => ["f", "\x. f (x x)", "\x. f (x x)"]
        private static List<string> LevelSplit(string str)
        {
            var parts = new List<string>();
            var level = 0;
            var current = "";
            for (var i = 0; i < str.Length; i++)
            {
                // staying on outer level 0
                if (level == 0)
                {
                    // going down 1 deeper ends current section
                    if ('('.Equals(str[i]))
                    {
                        parts.Add(current.Trim());
                        current = "";
                        level++;
                    }
                    // continue to collect current section
                    else
                    {
                        current += str[i];
                    }
                }
                // inner level > 0
                else
                {
                    // going down 1 deeper does NOT end current section
                    if ('('.Equals(str[i]))
                    {
                        current += str[i];
                        level++;
                    }
                    // going up 1 higher does end current section, if level = 1
                    // else continue to collect current section
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
                        // continue to collect current section
                        current += str[i];
                    }
                }
            }
            // adding last collected section
            parts.Add(current.Trim());
            // removing empty sections
            parts.RemoveAll(item => "".Equals(item));
            return parts;
        }

        // parses a terminal symbol (no recursive steps anymore), which are:
        // Constant: Operator, Number, Boolean, Characters
        // Variable: anything else (single character name)
        private static LExpr ParseTerminal(string expr)
        {
            if (new Regex("\\+|-|\\*|/|=|\\d+|True|False|'.+'").IsMatch(expr))
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
