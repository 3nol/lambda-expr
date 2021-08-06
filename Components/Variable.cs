using System;
using System.Collections.Generic;

namespace lambda_cs.Components
{
    class Variable : LExpr
    {
        private char var;

        public Variable(char v)
        {
            this.var = v;
        }

        public char getVar()
        {
            return this.var;
        }

        bool IEquatable<LExpr>.Equals(LExpr other)
        {
            if (other is Variable)
            {
                var otherVar = other as Variable;
                return this.var.Equals(otherVar.getVar());
            }
            else
            {
                return false;
            }
        }


        List<char> LExpr.getFreeVars()
        {
            return new List<char>(this.var);
        }

        List<char> LExpr.getBoundVars()
        {
            return new List<char>();
        }

        public override string ToString()
        {
            return this.var.ToString();
        }
    }
}
