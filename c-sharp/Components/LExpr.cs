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

    // 3 operations are possible:
    //   alpha conversion (= bound variable renaming)
    //   beta reduction (= term evaluation by substitution)
    //   delta reduction (= primitive calculation)
    // 1 extra operation is the variable expansion, called when there are no redexes
    public enum Operation
    {
        None, Alpha, Beta, Delta, Expand
    }

    public abstract class LExpr : IEquatable<LExpr>
    {
        // -- STATIC METHODS --
        
        internal static Dictionary<char, LExpr> assignedVariables = new Dictionary<char, LExpr>();
        internal static Operation lastOperation = Operation.None;

        public static LExpr GetVariable(char name)
        {
            return assignedVariables[name];
        }

        public static void SetVariable(char name, LExpr expr)
        {
            assignedVariables[name] = expr;
        }

        // access to the last reduction operation
        public static Operation GetLastOperation()
        {
            return LExpr.lastOperation;
        }

        // -- INSTANCE METHODS --

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

        // Expands one free variable if it has an assigned value
        public abstract LExpr ExpandVariable(bool annotate);

        // has to implement the equality check
        public abstract bool Equals(LExpr other);

        // has to override the string representation for correct notation
        public override abstract string ToString();
    }

    class Expression
    {
        public static void main(string[] _)
        {
            // beta reductions
            var yCombinator = Parse("(\\f. (\\x. f (x x)) (\\x. f (x x))) 42");
            Console.WriteLine("\n" + yCombinator.ToString());
            yCombinator.Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy);

            // delta reduction
            var calc = Parse("+ 42 (* 6 2)");
            Console.WriteLine("\n" + calc.ToString());
            calc.Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy);

            // alpha conversion
            var alph = Parse("(\\x y. x) y");
            Console.WriteLine("\n" + alph.ToString());
            alph.Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy);

            // ultimate alpha test
            var alph2 = Parse("(\\f x. g (\\x. f x)) x (\\x. x)");
            Console.WriteLine("\n" + alph2.ToString());
            alph2.Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy).Reduce(Evaluation.Lazy);
        }
    }
}
