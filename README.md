# Lambda Expression Parser

## Usage

This is an lambda calculus interpreter, written in C#.
Executing `Evaluation.LambdaInteractive:Main`, the following commands are possible:
- `help` displays a small help page
- `set evaluation [lazy|eager]` sets the evaluation strategy
- `let <var_name> = <lambda_expr>` assigns an expression to a variable
- `reduce <lambda_expr>` reduces the given expression, using beta and delta reduction,
as well as alpha conversion if needed. Free, previously assigned variables are expanded if needed.
- `ast <lambda_expr>` renders an abstract syntax tree of the given expression
- `exit` quits the program

## Lambda Notation

The parser supports the following notations:
- Standard syntax: `\x. x`
- Haskell syntax: `\x -> x`
- Merged lambda abstractions: `\x y. y x`
- Polish operator notation (prefix): `\x y. + x y`

## Expression Components

There are 4 different kinds of components:
- Constant, with 4 types: \
Operator (`+`, `-`, `*`, `/`, `=`), Numeric (any integer), Boolean (`True`, `False`) or Characters (any coherent string)
- Variable (single, standalone character), e.g. `x`
- Application (two expressions in juxtaposition), e.g. `x y`
- Lambda Abstraction (head and body of an anonymous function), e.g. `\x. x`

## Examples

- Reduction using lazy evaluation (operators are strict):
```
λ> reduce (\a b c. a (b c)) (+ 1) (\x. x) 7
 -β-> (\b c. + 1 (b c)) (\x. x) 7
 -β-> (\c. + 1 ((\x. x) c)) 7
 -β-> + 1 ((\x. x) 7)
 -β-> + 1 7
 -δ-> 8
== reached WHNF ==
```

- Rendered abstract syntax tree:
```
λ> ast \x y. x (* 3 y)

              λ x
               |
              λ y
               |
               @
              / \
             /   \
            /     \
           /       \
          /         \
         /           \
        /             \
       x               @
                      / \
                     /   \
                    /     \
                   @       y
                  / \
                 *   3
```