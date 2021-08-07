using System;
using System.Collections.Generic;
using lambda_cs.Components;

namespace lambda_cs.Evaluation
{
    static class Utility
    {
        public static LExpr Substitute(LExpr source, char target, LExpr substitute)
        {
            if (source is Constant)
            {
                // nothing to substitute
                return source;
            }
            else if (source is Variable)
            {
                var v = source as Variable;
                // full replacement
                if (v.GetVar().Equals(target))
                {
                    return substitute;
                }
                // nothing to substitute
                else
                {
                    return source;
                }
            }
            else if (source is Application)
            {
                var a = source as Application;
                // substitution in both subexpressions
                return new Application(Substitute(a.GetExpr1(), target, substitute), Substitute(a.GetExpr2(), target, substitute));
            }
            else if (source is Lambda)
            {
                // nothing to substitute
                var l = source as Lambda;
                if (l.GetBoundVars().TrueForAll(item => item.Equals(target)))
                {
                    return source;
                }
                // substitution for beta reduction
                else if (!substitute.GetFreeVars().Contains(l.GetVar()))
                {
                    return new Lambda(l.GetVar(), Substitute(l.GetExpr(), target, substitute));
                }
                // alpha conversion
                else
                {
                    var usedVars = substitute.GetFreeVars();
                    usedVars.AddRange(l.GetExpr().GetFreeVars());
                    usedVars.AddRange(l.GetExpr().GetBoundVars());
                    var z = GetNewVar(usedVars);
                    return new Lambda(z, Substitute(new Variable(z), l.GetVar(), l.GetExpr()));
                }
            }
            return null;
        }

        public static List<char> NameCapture(LExpr source, LExpr substitute)
        {
            var captured = new List<char>();
            foreach (char v in source.GetBoundVars())
            {
                if (substitute.GetFreeVars().Contains(v))
                {
                    captured.Add(v);
                }
            }
            return captured;
        }

        // for alpha conversion, a new variable name is necessary
        // because there must not be variables named equal,
        // this method retrieves the first unused variable name
        private static char GetNewVar(List<char> usedVars)
        {
            var newVar = 'a';
            while (usedVars.Contains(newVar))
            {
                newVar = (char)((int)newVar + 1);
            }
            return newVar;
        }

        public static bool IsRedex(LExpr expr, Components.Evaluation eval)
        {
            return !expr.Equals(expr.Reduce(eval, false));
        }

        public static LExpr DeltaReduce(Constant op, Constant e1, Constant e2)
        {
            switch (op.GetContent())
            {
                case "+":
                    if (e1.GetConstantType() == Constant.Type.Numeric && e2.GetConstantType() == Constant.Type.Numeric)
                    {
                        return new Constant((Int32.Parse(e1.GetContent()) + Int32.Parse(e2.GetContent())).ToString());
                    }
                    else if (e1.GetConstantType() == Constant.Type.Characters && e2.GetConstantType() == Constant.Type.Characters)
                    {
                        return new Constant(e1.GetContent().Substring(0, e1.GetContent().Length - 1) + e2.GetContent().Substring(1));
                    }
                    break;
                case "-":
                    if (e1.GetConstantType() == Constant.Type.Numeric && e2.GetConstantType() == Constant.Type.Numeric)
                    {
                        return new Constant((Int32.Parse(e1.GetContent()) - Int32.Parse(e2.GetContent())).ToString());
                    }
                    break;
                case "*":
                    if (e1.GetConstantType() == Constant.Type.Numeric && e2.GetConstantType() == Constant.Type.Numeric)
                    {
                        return new Constant((Int32.Parse(e1.GetContent()) * Int32.Parse(e2.GetContent())).ToString());
                    }
                    break;
                case "/":
                    if (e1.GetConstantType() == Constant.Type.Numeric && e2.GetConstantType() == Constant.Type.Numeric)
                    {
                        return new Constant((Int32.Parse(e1.GetContent()) / Int32.Parse(e2.GetContent())).ToString());
                    }
                    break;
                case "=":
                    return new Constant(e1.GetContent().Equals(e2.GetContent()) ? "True" : "False");
                default:
                    break;
            }
            return null;
        }

        public static void Log(string message, bool visible)
        {
            if (visible)
            {
                Console.WriteLine(message);
            }
        }
    }
}
