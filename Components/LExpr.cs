using System;
using System.Collections.Generic;
using System.Text;

namespace lambda_cs.Components
{
    interface LExpr : IEquatable<LExpr>
    {
        List<char> getFreeVars();

        List<char> getBoundVars();
    }

    class Expression
    {
        static void Main(string[] args)
        {
            var expr = new Lambda('f', new Application(new Lambda('x', new Application(new Variable('f'), new Application(new Variable('x'), new Variable('x')))),
                                                       new Lambda('x', new Application(new Variable('f'), new Application(new Variable('x'), new Variable('x'))))));
            Console.WriteLine(expr.ToString());
        }
    }
}
