using System;
using System.Linq;

namespace lambda_cs.Renderer
{
    class BinTree : AsciiPlane
    {
        private BinTree child1;
        private BinTree child2 = null;

        public BinTree(string content) : base(content.Split('\n').Select(x => x.Length).Aggregate((x, y) => Math.Max(x, y)), content.Split('\n').Length)
        {
            this.InsertIntoPlane(content);
        }

        public BinTree(string content, BinTree child) : this(content)
        {
            this.child1 = child;

        }

        public BinTree(string content, BinTree child1, BinTree child2) : this(content)
        {
            this.child1 = child1;
            this.child2 = child2;
        }

        public BinTree GetChild1()
        {
            return this.child1;
        }

        public BinTree GetChild2()
        {
            return this.child2;
        }

        private void InsertIntoPlane(string content)
        {
            var textWidth = content.Split('\n').Select(x => x.Length).Aggregate((x, y) => Math.Max(x, y));
            var plane = new char[textWidth, content.Split('\n').Length];
            var (x, y) = (0, 0);
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Equals('\n'))
                {
                    y++;
                }
                else
                {
                    plane[x++ % textWidth, y] = content[i];
                }
            }
            this.Embedd(new AsciiPlane(plane), PlaneAlignment.TOP_CENTER);
        }

        public override string ToString()
        {
            if (this.child1 != null && this.child2 != null)
            {
                AsciiPlane child1Rep = new BinTree(this.child1.ToString());
                AsciiPlane child2Rep = new BinTree(this.child2.ToString());
                var (childWidth, childHeight) = (Math.Max(child1Rep.GetWidth(), child2Rep.GetWidth()), Math.Max(child1Rep.GetHeight(), child2Rep.GetHeight()));
                child1Rep = new AsciiPlane(childWidth, childHeight, child1Rep, PlaneAlignment.TOP_CENTER);
                child2Rep = new AsciiPlane(childWidth, childHeight, child2Rep, PlaneAlignment.TOP_CENTER);

                var branchWidth = childWidth / 2 * 2 + 3;
                var branchHeight = branchWidth / 2 + this.GetHeight();
                var thisRep = new char[2 * childWidth + 3, branchHeight + childHeight];
                var xInset = childWidth / 2;
                for (int h = branchHeight; h >= this.GetHeight(); h--)
                {
                    thisRep[xInset, h] = '/';
                    thisRep[thisRep.GetLength(0) - 1 - xInset, h] = '\\';
                    xInset++;
                }
                var printPlane = new AsciiPlane(thisRep);
                printPlane.Embedd(this, PlaneAlignment.TOP_CENTER);
                printPlane.Embedd(child1Rep, PlaneAlignment.BOTTOM_LEFT);
                printPlane.Embedd(child2Rep, PlaneAlignment.BOTTOM_RIGHT);
                return printPlane.ToString();
            }
            else if (child1 != null)
            {
                var childRep = this.child1.ToString();
                var (width, height) = (childRep.Split('\n').Select(x => x.Length).Aggregate((x, y) => Math.Max(x, y)), childRep.Split('\n').Length);
                var thisRep = new char[Math.Max(this.GetWidth(), width), this.GetHeight() + 1 + height];
                thisRep[Math.Max(this.GetWidth(), width) / 2, this.GetHeight()] = '|';
                var printPlane = new AsciiPlane(thisRep);
                printPlane.Embedd(this, PlaneAlignment.TOP_CENTER);
                printPlane.Embedd(new BinTree(this.child1.ToString()), PlaneAlignment.BOTTOM_CENTER);
                return printPlane.ToString();
            }
            else
            {
                return base.ToString();
            }
        }
    }
}
