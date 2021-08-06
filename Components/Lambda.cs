using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace lambda_cs.Components
{
    class Lambda : LExpr
    {
        private char var;
        private LExpr expr;

        public Lambda(char v, LExpr e)
        {
            this.var = v;
            this.expr = e;
        }

        public char getVar()
        {
            return this.var;
        }

        public LExpr getExpr()
        {
            return this.expr;
        }

        bool IEquatable<LExpr>.Equals(LExpr other)
        {
            if (other is Lambda)
            {
                var otherLam = other as Lambda;
                return this.var.Equals(otherLam.getVar()) && this.expr.Equals(otherLam.getExpr());
            } 
            else
            {
                return false;
            }
        }

        List<char> LExpr.getFreeVars()
        {
            var freeVars = this.expr.getFreeVars();
            freeVars.RemoveAll(item => item.Equals(this.var));
            return freeVars;
        }

        List<char> LExpr.getBoundVars()
        {
            var boundVars = this.expr.getBoundVars();
            boundVars.Add(this.var);
            return boundVars;
        }

        public override string ToString()
        {
            // TODO merge consecutive Lambdas
            return "\\" + this.var + ". " + this.expr.ToString();
        }
    }
}
