using System;
using System.Collections.Generic;
using static lambda_cs.Evaluation.Utility;

namespace lambda_cs.Components
{
    class Constant : LExpr
    {
        // classification of constants into an mathematical operator,
        // a numeric constant, a boolean value or a sequence of characters
        public enum Type
        {
            Operator, Numeric, Boolean, Characters
        }

        // content of the constant
        private string content;
        // type classification
        private Type type;

        public Constant(string c)
        {
            this.content = c;
            DetermineType();
        }

        // determines the constant type with trying to parse an integer,
        // matching to a list of math operators or otherwise, classifying as characters
        private void DetermineType()
        {
            if (new List<string>() { "+", "-", "*", "/", "=" }.Contains(this.content))
            {
                this.type = Type.Operator;
            }
            else if (Int32.TryParse(this.content, out int _))
            {
                this.type = Type.Numeric;
            }
            else if (new List<string>() { "True", "False" }.Contains(this.content))
            {
                this.type = Type.Boolean;
            }
            else 
            {
                this.type = Type.Characters;
            }
        }

        public string GetContent()
        {
            return this.content;
        }

        public Type GetConstantType()
        {
            return this.type;
        }

        // collects no free variables because there can be none here
        public override List<char> GetFreeVars()
        {
            return new List<char>();
        }

        // collects no bound variables because there can be none here
        public override List<char> GetBoundVars()
        {
            return new List<char>();
        }

        // tries to reduce this constant
        public override LExpr Reduce(Evaluation _, bool annotate)
        {
            // a constant is already in normal form
            Log("== reached NF ==", annotate);
            return this;
        }

        // this constant is equal to another one if the type and content are equal
        public override bool Equals(LExpr other)
        {
            if (other is Constant)
            {
                var otherConst = other as Constant;
                return this.type.Equals(otherConst.GetConstantType()) && this.content.Equals(otherConst.GetContent());
            }
            else
            {
                return false;
            }
        }

        // converts to string by simply returning the constant's content
        public override string ToString()
        { 
            return this.content;
        }
    }
}
