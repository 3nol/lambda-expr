using System;
using lambda_cs.Components;
using static lambda_cs.Components.LExpr;
using static lambda_cs.Evaluation.Parser;
using static lambda_cs.Renderer.ASTBuilder;

namespace lambda_cs.Evaluation
{
    static class LambdaInteractive
    {
        private static Components.Evaluation currentEval = Components.Evaluation.Lazy;
        public static void Main(string[] _)
        {
            bool running;
            do
            {
                // interactive lambda expression input, parsing and reduction
                running = DoAction(GetInput());
            } while (running);
        }

        // writes a lambda prompt in console and receives inputted text
        private static string GetInput()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Write("\u03bb> ");
            return Console.ReadLine();
        }

        // decides which action to make
        private static bool DoAction(string input)
        {
            if (!input.ToLower().Equals("exit"))
            {
                if (input.ToLower().Equals("help"))
                {
                    Console.WriteLine("This is an interactive shell that can evaluate lambda expressions.\n" +
                                      "- shell control:\n" +
                                      "    help                   - shows this message\n" +
                                      "    exit                   - closes the shell\n" +
                                      "    set [property] [value] - sets shell properties, e.g. the evaluation strategy to lazy/eager\n" +
                                      "- lambda expressions:\n" +
                                      "    let [var] = [expr]     - assigns a lambda expression to the variable\n" +
                                      "    reduce [expr]          - reduces the expression according to the current strategy,\n" +
                                      "                             uses alpha conversion, beta & delta reduction and variable expansion\n" +
                                      "    ast [expr]             - displays an abstract syntax tree of the given expression");
                }
                else if (input.ToLower().StartsWith("set") && input.Split(' ').Length >= 3)
                {
                    switch (input.Split(' ')[1].ToLower())
                    {
                        case "evaluation":
                            try
                            {
                                var evalName = char.ToUpper(input.Split(' ')[2][0]) + input.Split(' ')[2].Substring(1).ToLower();
                                currentEval = (Components.Evaluation)Enum.Parse(Components.Evaluation.Lazy.GetType(), evalName);
                                Console.WriteLine("using " + currentEval.ToString().ToLower() + " evaluation ");
                            }
                            catch (ArgumentException )
                            {
                                Console.WriteLine("invalid evaluation name");
                            }
                            break;
                        default:
                            Console.WriteLine("invalid property");
                            break;
                    }
                }
                else if (input.ToLower().StartsWith("let") && input.Split(' ').Length >= 2 && input.Contains('='))
                {
                    var name = input.Split(' ')[1][0];
                    var expr = Parse(input.Split('=', 2)[1]);
                    if (!(expr is Variable && (expr as Variable).GetVar().Equals(name)))
                    {
                        SetVariable(name, expr);
                    }
                }
                else if (input.ToLower().StartsWith("reduce") && input.Split(' ').Length >= 2)
                {
                    var expr = Parse(input.Split(' ', 2)[1]);
                    var reduced = expr;
                    do
                    {
                        do
                        {
                            expr = reduced;
                            reduced = expr.Reduce(currentEval);
                        } while (!expr.Equals(reduced));
                        expr = reduced;
                        reduced = expr.ExpandVariable(true);
                    } while (!expr.Equals(reduced));
                    Console.WriteLine("== reached WHNF ==\n");
                }
                else if (input.ToLower().StartsWith("ast") && input.Split(' ').Length >= 2)
                {
                    Console.WriteLine(BuildTree(Parse(input.Split(' ', 2)[1])));
                }
                else if (input.ToLower().Equals("author"))
                {
                    Console.WriteLine("Leon Wenzler");
                }
                else
                {
                    Console.WriteLine("invalid action");
                }
                return true;
            }
            return false;
        }
    }
}
