using System;
using System.Collections.Generic;
using static lambda_cs.Parser.Parser;

namespace lambda_cs.Components
{
    public enum Operation
    {
        None, Alpha, Beta, Delta
    }

    public enum Evaluation
    {
        Lazy, Eager, Normal
    }

    public abstract class LExpr : IEquatable<LExpr>
    {
        public abstract List<char> getFreeVars();

        public abstract List<char> getBoundVars();

        public abstract List<Operation> Reduce(Evaluation eval); 

        public abstract bool Equals(LExpr other);

        public override abstract string ToString();
    }

    class Expression
    {
        static void Main(string[] args)
        {
            var expr = new Application(new Lambda('f', new Application(new Lambda('x', new Application(new Variable('f'), new Application(new Variable('x'), new Variable('x')))),
                                                       new Lambda('x', new Application(new Variable('f'), new Application(new Variable('x'), new Variable('x')))))),
                                       new Constant("42"));
            Console.WriteLine(Parse(expr.ToString()).ToString());
        }
    }
}
