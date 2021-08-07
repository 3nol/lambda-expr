using System.Collections.Generic;

namespace lambda_cs.Evaluation
{
    static class Utility
    {
        // for alpha conversion, a new variable name is necessary
        // because there must not be variables named equal,
        // this method retrieves the first unused variable name
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
