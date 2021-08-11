using lambda_cs.Components;

namespace lambda_cs.Renderer
{
    static class ASTBuilder
    {
        public static AsciiPlane BuildTree(LExpr expr)
        {
            return BuildTree(expr, 1);
        }

        private static AsciiPlane BuildTree(LExpr expr, int padding)
        {
            var tree = ConstructTree(expr);
            var treeRep = new BinTree(tree.ToString());
            return new AsciiPlane(treeRep.GetWidth() + 2 * padding, treeRep.GetHeight() + 2 * padding, 
                                  treeRep, AsciiPlane.PlaneAlignment.CENTER);
        }

        private static BinTree ConstructTree(LExpr expr)
        {
            if (expr is Constant)
            {
                var c = expr as Constant;
                return new BinTree(c.GetContent());
            }
            else if (expr is Variable)
            {
                var v = expr as Variable;
                return new BinTree(v.GetVar().ToString());
            }
            else if (expr is Application)
            {
                var a = expr as Application;
                return new BinTree("@", ConstructTree(a.GetExpr1()), ConstructTree(a.GetExpr2()));
            }
            else if (expr is Lambda)
            {
                var l = expr as Lambda;
                return new BinTree("\u03bb " + l.GetVar(), ConstructTree(l.GetExpr()));

            }
            return null;
        }
    }
}
