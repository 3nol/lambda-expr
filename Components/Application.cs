using System.Collections.Generic;
using static lambda_cs.Evaluation.Utility;

namespace lambda_cs.Components
{
    class Application : LExpr
    {
        // first part of application
        private LExpr expr1;
        // second part of application
        private LExpr expr2;

        public Application(LExpr e1, LExpr e2)
        {
            this.expr1 = e1;
            this.expr2 = e2;
        }

        public LExpr GetExpr1()
        {
            return this.expr1;
        }

        public LExpr GetExpr2()
        {
            return this.expr2;
        }

        // collects free variables by combinbing all free variables found
        // in the two sub expressions
        public override List<char> GetFreeVars()
        {
            var freeVars = this.expr1.GetFreeVars();
            freeVars.AddRange(this.expr2.GetFreeVars());
            return freeVars;
        }

        // collects bound variables by combinbing all bound variables found
        // in the two sub expressions
        public override List<char> GetBoundVars()
        {
            var boundVars = this.expr1.GetBoundVars();
            boundVars.AddRange(this.expr2.GetBoundVars());
            return boundVars;
        }

        // reduces every combination of function application
        public override LExpr Reduce(Evaluation eval, bool annotate)
        {
            // first subexpression is a lambda abstraction
            if (this.expr1 is Lambda)
            {
                var lambda = this.expr1 as Lambda;
                // eager evaluation reduces the argument before the function
                if (eval == Evaluation.Eager && IsRedex(this.expr2, eval))
                {
                    var e = new Application(lambda, this.expr2.Reduce(eval, false));
                    Log("--beta-> " + e.ToString(), annotate);
                    return e;
                }
                // if a name captures is not present, this lambda abstraction is reduced
                else if (NameCapture(lambda.GetExpr(), this.expr2).Count == 0)
                {
                    var e = Substitute(lambda.GetExpr(), lambda.GetVar(), this.expr2);
                    Log("--beta-> " + e.ToString(), annotate);
                    return e;
                }
                // if a name capture happens, alpha conversion is being done
                else
                {
                    var e = new Application(Substitute(lambda, lambda.GetVar(), this.expr2), this.expr2);
                    Log("--alpha-> " + e.ToString(), annotate);
                    return e;
                }
            }
            // all cases for application reduction
            else if (this.expr1 is Application)
            {
                var app = this.expr1 as Application;
                // first expression is an operator which forces its arguments to strict evaluation
                if ((app.GetExpr1() is Constant) && (app.GetExpr1() as Constant).GetConstantType() == Constant.Type.Operator)
                {
                    // reduce first argument
                    if (IsRedex(app.GetExpr2(), eval))
                    {
                        var e = new Application(new Application(app.GetExpr1(), app.GetExpr2().Reduce(eval, false)), this.expr2);
                        Log("--beta-> " + e.ToString(), annotate);
                        return e;
                    }
                    // reduce second argument
                    else if (IsRedex(this.expr2, eval))
                    {
                        var e = new Application(new Application(app.GetExpr1(), app.GetExpr2()), this.expr2.Reduce(eval, false));
                        Log("--beta-> " + e.ToString(), annotate);
                        return e;
                    }
                    // make delta reduction by evaluating the operator
                    else if (app.GetExpr2() is Constant && this.expr2 is Constant)
                    {
                        var e = DeltaReduce(app.GetExpr1() as Constant, app.GetExpr2() as Constant, this.expr2 as Constant);
                        Log("--delta-> " + e.ToString(), annotate);
                        return e;
                    }
                    // if operator has no fitting arguments, nf is reached
                    else
                    {
                        Log("== reached NF ==", annotate);
                        return this;
                    }
                }
                else
                {
                    // finally, the expression is evaluated from left to right
                    if (IsRedex(this.expr1, eval))
                    {
                        // left part can be reduced
                        var e = new Application(this.expr1.Reduce(eval, false), this.expr2);
                        Log("--beta-> " + e.ToString(), annotate);
                        return e;
                    }
                    else
                    {
                        // left part cannot be reduced, expression is in whnf
                        Log("== reached WHNF ==", annotate);
                        return this;
                    }
                }
            }
            // otherwise, no reduction can take place, a constant/ variable is in whnf
            else
            {
                Log("== reached WHNF ==", annotate);
                return this;
            }
        }

        // this application is only equal if both subexpression are equal to other expression
        public override bool Equals(LExpr other)
        {
            if (other is Application)
            {
                var otherApp = other as Application;
                return this.expr1.Equals(otherApp.GetExpr1()) && this.expr2.Equals(otherApp.GetExpr2());
            }
            else
            {
                return false;
            }
        }

        // converts to a string by converting both subexpressions and placing parentheses only when:
        // - around first subexpression if it is a Lambda
        // - around second subexpression if if is either a Lambda or an Application
        public override string ToString()
        {
            var e1 = this.expr1 is Lambda ? "(" + this.expr1.ToString() + ")" : this.expr1.ToString();
            var e2 = this.expr2 is Variable || this.expr2 is Constant ? this.expr2.ToString() : "(" + this.expr2.ToString() + ")";
            return e1 + " " + e2;
        }
    }
}
