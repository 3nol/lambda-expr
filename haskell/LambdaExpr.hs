
module LambdaExpr where

--- SETUP ---

data LExpr = Var String
           | App LExpr LExpr
           | Lam String LExpr
           deriving(Eq)

instance Show LExpr where
  showsPrec _ (Var v)     = showString v
  showsPrec p (App e1 e2) = showParen (p > 1) $ showsPrec 1 e1 . showChar ' ' . showsPrec 2 e2
  showsPrec p (Lam v e)   = showParen (p > 0) $ showChar '\\' . showString v . merge e
     where
        merge (Lam v e)   = showChar ' ' . showString v . merge e
        merge other       = showString ". " . shows other


--- PARSER ---

input :: Int -> IO ()
input 2 = do
  expr <- getLine
  putStrLn $ display $ normalTrace $ parse expr
input 1 = do
  expr <- getLine
  putStrLn $ display $ eagerTrace $ parse expr
input _ = do
  expr <- getLine
  putStrLn $ display $ lazyTrace $ parse expr

parse :: String -> LExpr
parse ""        = error "cannot construct lambda expression"
parse ('\\':cs) = foldr Lam (parse . dropWhile (==' ') . tail . dropWhile (/='.') $ cs) (words $ takeWhile (/='.') cs)    -- parsing lamba abstractions and their variables
parse cs        | null [x | c <- cs, x <- "().\\", c==x] = let ws = words cs in
                  foldl App (Var $ head ws) (map Var $ tail ws)                                                           -- parsing single variables
                | otherwise                              = let e = separate cs in
                  foldl App (parse $ head e) (map parse $ tail e)                                                         -- parsing expression applications

separate :: String -> [String]
separate []  = []
separate str | null [x | c <- str, x <- "().\\", c==x] = words str                                                        -- separating expressions on spaces
             | otherwise                               = let (f,s) = go 0 ("","") $ dropWhile (/='(') str in
               filter (not . null) $ [takeWhile (/='(') str,f] ++ separate s                                              -- separating expressions on parentheses
               where
                  go _ acc ""       = acc
                  go n (x,y) (c:cs) | c == '(' && n == 0 = go (n+1) (x,y) cs
                                    | c == '('           = go (n+1) (x ++ [c],y) cs
                                    | c == ')' && n == 1 = (x,y ++ dropWhile (==' ') cs)
                                    | c == ')'           = go (n-1) (x ++ [c],y) cs
                                    | otherwise          = go n (x ++ [c],y) cs

display :: [(LExpr,Char)] -> String
display = flip (++) "\n\n== reached WHNF ==" . foldl (\x y -> case y of
                                                            (expr,'a') -> x ++ "\n--a-> " ++ show expr
                                                            (expr,'b') -> x ++ "\n--b-> " ++ show expr
                                                            (expr,'d') -> x ++ "\n--d-> " ++ show expr
                                                            _          -> x) ""

instance Read LExpr where
  readsPrec _ str = [(parse str,str)]


--- REDUCTION ---

lazy :: LExpr -> LExpr                                                              -- complete reduction using lazy evaluation until whnf
lazy = fst . last . lazyTrace
lazyTrace :: LExpr -> [(LExpr, Char)]
lazyTrace = takeWhile ((/='0') . snd) . iterate (reduce 0) . flip (,) '-'

eager :: LExpr -> LExpr                                                             -- complete reduction using eager evaluation until whnf
eager = fst . last . eagerTrace
eagerTrace :: LExpr -> [(LExpr, Char)]
eagerTrace = takeWhile ((/='0') . snd) . iterate (reduce 1) . flip (,) '-'

normal :: LExpr -> LExpr                                                            -- complete reduction using eager evaluation until nf
normal = fst . last . normalTrace
normalTrace :: LExpr -> [(LExpr, Char)]
normalTrace = takeWhile ((/='0') . snd) . iterate (reduce 2) . flip (,) '-'
                                                                                    -- single beta reduction, options: 0 = lazy to whnf, 1 = eager to whnf, 2 = eager to nf
reduce :: Int -> (LExpr, Char) -> (LExpr, Char)
reduce p = beta . fst
     where
        beta (Var v)                               = (Var v, '0')
        beta (App e1 e2)   = case (e1,e2) of
                               (Lam x e, _)        | (p > 0) && e2 /= fst (beta e2) -> let (ex,op) = beta e2 in (App e1 ex, op)                                                 -- reduce argument before function in eager
                                                   | null captured                  -> (subst e2 x e, 'b')                                                                      -- no alpha conversion needed
                                                   | otherwise                      -> (App (Lam x $ repl z (head captured) e) e2, 'a')                                         -- check for name capture, don't use alpha
                                                   where
                                                      captured = [ w | w <- bound e, w `elem` free e2 ]
                                                      z = newVar $ free e2 ++ free e ++ bound e 
                               (App (Var v) e1, _) | v `elem` ["+","-","*","/","="] -> if e1 /= fst (beta e1)
                                                                                       then let (ex,op) = beta e1 in (App (App (Var v) ex) e2, op)
                                                                                       else if e2 /= fst (beta e2)
                                                                                             then let (ex,op) = beta e2 in (App (App (Var v) e1) ex, op) 
                                                                                             else (Var $ delta v e1 e2, 'd')       
                                                   | p > 1                          -> case e1 of                                                                               -- case of multiple applications (eager)
                                                                                         (Lam x e) | null captured -> (App (Var v) (subst e2 x e), 'b')                         -- check for name capture, don't use alpha
                                                                                                   | otherwise     -> (App (App (Var v) (Lam x $ repl z (head captured) e)) e2, 'a')
                                                                                                   where
                                                                                                       captured = [ w | w <- bound e, w `elem` free e2 ]
                                                                                                       z = newVar $ free e2 ++ free e ++ bound e 
                                                                                         _         -> let (ex,op) = beta e1 in (App (App (Var v) ex) e2, op)                    -- no possibility for name capture arising
                                                   | otherwise                      -> (App (App (Var v) e1) e2, '0')
                               _                   -> let (ex,op) = beta e1 in (App ex e2, op)                                                                                  -- go left in application tree
        beta (Lam x e)     | (p > 1)               = let (ex,op) = beta e in (Lam x ex, op)                                                                                     -- reduce everything
                           | otherwise             = (Lam x e, '0')                                                                                                             -- expression is in weak head normal form
        delta op (Var x) (Var y) = show $ (case op of
                                            "+"  -> (+)
                                            "-"  -> (-)
                                            "*"  -> (*)
                                            "/"  -> div
                                            "="  -> (\a b -> if a==b then 1 else 0)
                                            _    -> const) (read x :: Int) (read y :: Int)
        delta _  _       _       = undefined        

subst :: LExpr -> String -> LExpr -> LExpr
subst m x (Var v)     | v == x                = m
                      | otherwise             = Var v
subst m x (App e1 e2)                         = App (subst m x e1) (subst m x e2)
subst m x (Lam v e)   | v == x                = Lam v e
                      | not (v `elem` free m) = Lam v (subst m x e)
                      | otherwise             = subst m x (Lam z (subst (Var z) v e))
     where 
       z = newVar $ free m ++ free e ++ bound e

repl :: String -> String -> LExpr -> LExpr
repl r x (Var v)     | v == x    = Var r
                     | otherwise = Var v
repl r x (App e1 e2)             = App (repl r x e1) (repl r x e2)
repl r x (Lam v e)   | v == x    = Lam r (repl r x e)
                     | otherwise = Lam v (repl r x e)

free :: LExpr -> [String]                                                             -- gets all free variables in a lambda expression
free (Var v)     = [v]
free (App e1 e2) = free e1 ++ free e2
free (Lam v e)   = filter (/= v) $ free e

bound :: LExpr -> [String]                                                            -- gets all bound variables in a lambda expression
bound (Var _)     = []
bound (App e1 e2) = bound e1 ++ bound e2
bound (Lam x e)   = bound e ++ [x]                                                    -- softer version: ++ (filter (==x) $ free e)

newVar :: [String] -> String                                                          -- creates a new variable name
newVar strs = go "a" 0
     where
       go c n | not (c `elem` strs) = c                                               -- variable is not in list
              | c !! n == 'z'     = go (c++"a") (n+1)                                 -- increase variable length by adding an 'a'
              | otherwise         = go [succ (c !! n)] n                              -- use the variables successor in the alphabet


--- FUNCTIONS --- 

lfold :: (String -> a) -> (a -> a -> a) -> (String -> a -> a) -> LExpr -> a
lfold var app lam = go
    where
      go (Var v)     = var v
      go (App e1 e2) = app (go e1) (go e2)
      go (Lam v e)   = lam v $ go e

-- :set prompt "λ> "