using System;
using System.Collections.Generic;
using static lambda_cs.Evaluation.Parser;

namespace lambda_cs.Components
{
    // 2 evaluation strategies are possible:
    //   lazy - evaluates until WHNF (no top-level redexes)
    //          by starting the reduction leftmost outermost
    //   eager - tries to evaluate until WHNF (no top-level redexes)
    //          by starting the reduction leftmost innermost
    public enum Evaluation
    {
        Lazy, Eager
    }

    public abstract class LExpr : IEquatable<LExpr>
    {
        protected string alphaPrompt = "-a-> ";
        protected string betaPrompt = "-b-> ";
        protected string deltaPrompt = "-d-> ";

        // concept of free variables: are retrieved here
        public abstract List<char> GetFreeVars();

        // concept of bound variables: are retrieved here
        public abstract List<char> GetBoundVars();

        // default call to reduce
        public LExpr Reduce(Evaluation eval)
        {
            return Reduce(eval, true);
        }

        // Lambda expression can be reduced in 3 ways:
        // lazy, eager or normal evaluation
        // 
        // For this, 3 types of operations are used:
        // - alpha conversion (= renaming)
        // - beta reduction (= reduction)
        // - delta reduction (= primitive calulation)
        public abstract LExpr Reduce(Evaluation eval, bool annotate); 

        // has to implement the equality check
        public abstract bool Equals(LExpr other);

        // has to override the string representation for correct notation
        public override abstract string ToString();
    }

    class Expression
    {
        static void Main(string[] _)
        {
            // LambdaInteractive(Evaluation.Lazy);

            // beta reductions
            var yCombinator = Parse("(\\f. (\\x. f (x x)) (\\x. f (x x))) 42");
            Console.WriteLine("\n" + yCombinator.ToString());
            yCombinator.Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy);

            // delta reduction
            var calc = Parse("+ 42 (* 6 2)");
            Console.WriteLine("\n" + calc.ToString());
            calc.Reduce(Evaluation.Lazy);

            // alpha conversion
            var alph = Parse("(\\x y. x) y");
            Console.WriteLine("\n" + alph.ToString());
            alph.Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy);
        }

        // interactive lambda expression input, parsing and reduction
        static void LambdaInteractive(Evaluation eval)
        {
            var control = "";
            while (!"END".Equals(control))
            {
                Console.WriteLine("Enter a lambda expression that shall be reduced.");
                var expr = Parse(Console.ReadLine());
                var reduced = expr;
                do
                {
                    expr = reduced;
                    reduced = expr.Reduce(eval);
                } while (!expr.Equals(reduced));

                control = Console.ReadLine();
            }
        }
    }
}
