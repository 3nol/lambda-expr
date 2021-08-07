using System;
using System.Collections.Generic;

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

        public LExpr getExpr1()
        {
            return this.expr1;
        }

        public LExpr getExpr2()
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

        public override List<Operation> Reduce(Evaluation eval)
        {
            throw new NotImplementedException();
        }

        // this application is only equal if both subexpression are equal to other expression
        public override bool Equals(LExpr other)
        {
            if (other is Application)
            {
                var otherApp = other as Application;
                return this.expr1.Equals(otherApp.getExpr1()) && this.expr2.Equals(otherApp.getExpr2());
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
