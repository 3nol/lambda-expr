using System;
using System.Collections.Generic;
using System.Text;

namespace lambda_cs.Components
{
    class Application : LExpr
    {
        private LExpr expr1;
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

        bool IEquatable<LExpr>.Equals(LExpr other)
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

        List<char> LExpr.getFreeVars()
        {
            var freeVars = this.expr1.getFreeVars();
            freeVars.AddRange(this.expr2.getFreeVars());
            return freeVars;
        }

        List<char> LExpr.getBoundVars()
        {
            var boundVars = this.expr1.getBoundVars();
            boundVars.AddRange(this.expr2.getBoundVars());
            return boundVars;
        }

        public override string ToString()
        {
            // TODO remove unnecessary Parentheses
            var e1 = this.expr1 is Variable ? this.expr1.ToString() : "(" + this.expr1.ToString() + ")";
            var e2 = this.expr2 is Variable ? this.expr2.ToString() : "(" + this.expr2.ToString() + ")";
            return e1 + " " + e2;
        }
    }
}
