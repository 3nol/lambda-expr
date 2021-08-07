using System;
using System.Collections.Generic;
using static lambda_cs.Parser.Parser;

namespace lambda_cs.Components
{
    // 3 operations can be made on a lambda expression:
    // alpha conversion, beta reduction or delta reduction
    public enum Operation
    {
        None, Alpha, Beta, Delta
    }

    // 3 evaluation strategies are possible:
    //   lazy - evaluates until WHNF (no top-level redexes)
    //          by starting the reduction leftmost outermost
    //   eager - tries to evaluate until WHNF (no top-level redexes)
    //          by starting the reduction leftmost innermost
    //   normal - evaluates until NF (no redexes at all)
    //          by reducing everything without mercy
    public enum Evaluation
    {
        Lazy, Eager, Normal
    }

    public abstract class LExpr : IEquatable<LExpr>
    {
        // concept of free variables: are retrieved here
        public abstract List<char> GetFreeVars();

        // concept of bound variables: are retrieved here
        public abstract List<char> GetBoundVars();

        // lambda expression can be reduced in 3 ways:
        // lazy, eager or normal evaluation
        public abstract List<Operation> Reduce(Evaluation eval); 

        // has to implement the equality check
        public abstract bool Equals(LExpr other);

        // has to override the string representation for correct notation
        public override abstract string ToString();
    }

    class Expression
    {
        static void Main(string[] _)
        {
            var expr = new Application(new Lambda('f', new Application(new Lambda('x', new Application(new Variable('f'), new Application(new Variable('x'), new Variable('x')))),
                                                       new Lambda('x', new Application(new Variable('f'), new Application(new Variable('x'), new Variable('x')))))),
                                       new Constant("42"));
            Console.WriteLine(Parse(expr.ToString()).ToString());
        }
    }
}
