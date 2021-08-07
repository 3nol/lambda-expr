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

        public override List<char> getFreeVars()
        {
            return new List<char>(this.var);
        }

        public override List<char> getBoundVars()
        {
            return new List<char>();
        }

        public override List<Operation> Reduce(Evaluation eval)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(LExpr other)
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

        public override string ToString()
        {
            return this.var.ToString();
        }
    }
}
