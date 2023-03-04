# Lambda Expression Parser

This file contains a parser for converting a string of a lambda expression into an instance of `LExpr`.
An example for such a string would be: `\x. \y. x + y` \
This string would be converted into `Lam "x" (Lam "y" (App (App (Var "x") (Var "+")) (Var "y")))` which can be evaluated.

The string can be inputted by calling `input N` with different numbers for `N`:
- `input 2` -> normal evaluation
- `input 1` -> eager evaluation
- `input 0` -> lazy evaluation

The evaluation magic is done by `reduce` which supports alpha conversion, beta reduction and delta reduction.