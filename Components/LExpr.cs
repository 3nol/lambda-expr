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
            // beta reductions
            var yCombinator = new Application(new Lambda('f', new Application(new Lambda('x', new Application(new Variable('f'), new Application(new Variable('x'), new Variable('x')))),
                                                       new Lambda('x', new Application(new Variable('f'), new Application(new Variable('x'), new Variable('x')))))),
                                       new Constant("42"));
            Console.WriteLine("\n" + yCombinator.ToString());
            yCombinator.Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy);

            // delta reduction
            var calc = new Application(new Application(new Constant("+"), new Constant("3")), new Constant("5"));
            Console.WriteLine("\n" + calc.ToString());
            calc.Reduce(Evaluation.Lazy);

            // alpha conversion
            var alph = Parse("(\\x y. x) y");
            Console.WriteLine("\n" + alph.ToString());
            alph.Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy);
        }
    }
}
