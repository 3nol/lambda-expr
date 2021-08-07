using System.Collections.Generic;

namespace lambda_cs.Evaluation
{
    static class Utility
    {
        static char getNewVar(List<char> usedVars)
        {
            var newVar = 'a';
            while (usedVars.Contains(newVar))
            {
                newVar = (char)((int)newVar + 1);
            }
            return newVar;
        }

        // TODO: alpha conversion, substitution

        // TODO folds
    }
}
