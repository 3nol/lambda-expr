using System;
using System.Collections.Generic;

namespace lambda_cs.Components
{
    class Constant : LExpr
    {
        public enum Type
        {
            Characters, Numeric, Operator
        }

        private string content;
        private Type type;

        public Constant(string c)
        {
            this.content = c;
            determineType();
        }

        private void determineType()
        {
            if (Int32.TryParse(this.content, out int result))
            {
                this.type = Type.Numeric;
            }
            else if (new List<string>() { "+", "-", "*", "/", "=" }.Contains(this.content))
            {
                this.type = Type.Operator;
            }
            else
            {
                this.type = Type.Characters;
            }
        }

        public string getContent()
        {
            return this.content;
        }

        public Type getType()
        {
            return this.type;
        }

        public override List<char> getBoundVars()
        {
            return new List<char>();
        }

        public override List<char> getFreeVars()
        {
            return new List<char>();
        }

        public override List<Operation> Reduce(Evaluation eval)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(LExpr other)
        {
            if (other is Constant)
            {
                var otherConst = other as Constant;
                return this.type.Equals(otherConst.GetType()) && this.content.Equals(otherConst.getContent());
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        { 
            return this.type.ToString()[0] + this.content;
        }
    }
}
