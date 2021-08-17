# Lambda Expression Parser

This is an lambda calculus interpreter, written in C#.
Executing `Evaluation.LambdaInteractive:Main`, the following commands are possible:
- `help` displays a small help page
- `set evaluation [lazy|eager]` sets the evaluation strategy
- `let <var_name> = <lambda_expr>` assigns an expression to a variable
- `reduce <lambda_expr>` reduces the given expression, using beta and delta reduction,
as well as alpha conversion if needed. Free, previously assigned variables are expanded if needed.
- `ast <lambda_expr>` renders an abstract syntax tree of the given expression
- `exit` quits the program    