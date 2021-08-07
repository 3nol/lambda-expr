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

        public override List<char> getFreeVars()
        {
            var freeVars = this.expr.getFreeVars();
            freeVars.RemoveAll(item => item.Equals(this.var));
            return freeVars;
        }

        public override List<char> getBoundVars()
        {
            var boundVars = this.expr.getBoundVars();
            boundVars.Add(this.var);
            return boundVars;
        }

        public override List<Operation> Reduce(Evaluation eval)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(LExpr other)
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

        public override string ToString()
        {
            var str = "\\" + this.var;
            LExpr body = this.getExpr(); ;
            while (body is Lambda)
            {
                str += " " + (body as Lambda).getVar();
                body = (body as Lambda).getExpr();
            }
            return str + ". " + body.ToString();
        }
    }
}
