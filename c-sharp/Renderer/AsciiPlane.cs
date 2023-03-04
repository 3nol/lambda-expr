namespace lambda_cs.Renderer
{
    class AsciiPlane
    {
        public enum PlaneAlignment
        {
            TOP_LEFT, TOP_CENTER, TOP_RIGHT,
            CENTER_LEFT, CENTER, CENTER_RIGHT,
            BOTTOM_LEFT, BOTTOM_CENTER, BOTTOM_RIGHT
        }

        private char[,] plane;

        internal AsciiPlane(char[,] plane)
        {
            this.plane = plane;
            this.plane = FillBlanks(this.plane, ' ');
        }

        internal AsciiPlane(int width, int height)
        {
            this.plane = new char[width, height];
            this.plane = FillBlanks(this.plane, ' ');
        }

        internal AsciiPlane (int width, int height, AsciiPlane subplane, PlaneAlignment alignment) : this(width, height)
        {
            if (subplane.GetWidth() <= this.GetWidth() && subplane.GetHeight() <= this.GetHeight())
            {
                this.Embedd(subplane, alignment);
            }
        }

        internal AsciiPlane(int width, int height, AsciiPlane subplane) : this(width, height, subplane, PlaneAlignment.TOP_LEFT) {}

        private static char[,] FillBlanks(char[,] array, char fill)
        {
            for (var i = 0; i < array.GetLength(0); i++)
            {
                for (var j = 0; j < array.GetLength(1); j++)
                {
                    if (array[i, j].Equals('\0'))
                    {
                        array[i, j] = fill;
                    }
                }
            }
            return array;
        }

        public int GetWidth()
        {
            return this.plane.GetLength(0);
        }

        public int GetHeight()
        {
            return this.plane.GetLength(1);
        }

        public char Get(int x, int y)
        {
            return plane[x, y];
        }

        internal void Embedd(AsciiPlane subplane, PlaneAlignment alignment)
        {
            var (x, y) = CalculateInset(subplane.GetWidth(), subplane.GetHeight(), alignment);
            if (x >= 0 && y >= 0)
            {
                for (var j = y; j < this.GetHeight() && j - y < subplane.GetHeight(); j++)
                {
                    for (var i = x; i < this.GetWidth() && i - x < subplane.GetWidth(); i++)
                    {
                        this.plane[i, j] = subplane.Get(i - x, j - y);
                    }
                }
            }
        }

        private (int, int) CalculateInset(int subwidth, int subheight, PlaneAlignment alignment)
        {
            return alignment switch
            {
                PlaneAlignment.TOP_LEFT      => (0, 0),
                PlaneAlignment.TOP_CENTER    => ((this.GetWidth() - subwidth) / 2, 0),
                PlaneAlignment.TOP_RIGHT     => (this.GetWidth() - subwidth, 0),
                PlaneAlignment.CENTER_LEFT   => (0, (this.GetHeight() - subheight) / 2),
                PlaneAlignment.CENTER        => ((this.GetWidth() - subwidth) / 2, (this.GetHeight() - subheight) / 2),
                PlaneAlignment.CENTER_RIGHT  => (this.GetWidth() - subwidth, (this.GetHeight() - subheight) / 2),
                PlaneAlignment.BOTTOM_LEFT   => (0, this.GetHeight() - subheight),
                PlaneAlignment.BOTTOM_CENTER => ((this.GetWidth() - subwidth) / 2, this.GetHeight() - subheight),
                PlaneAlignment.BOTTOM_RIGHT  => (this.GetWidth() - subwidth, this.GetHeight() - subheight),
                _                            => (-1, -1),
            };
        }

        public override string ToString()
        {
            var printString = "";
            for (var j = 0; j < this.GetHeight(); j++)
            {
                for (var i = 0; i < this.GetWidth(); i++)
                {
                    printString += this.Get(i, j);
                }
                if (!j.Equals(this.GetHeight() - 1))
                {
                    printString += "\n";
                }
            }
            return printString;
        }
    }
}
