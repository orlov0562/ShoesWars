using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTest001
{
    class Point
    {
        private int x;
        private int y;

        public Point(int x, int y)
        {
            setXY(x, y);
        }
        
        public Point():this(0,0)
        {
            
        }

        public void setX(int x)
        {
            this.x = x;
        }

        public void setY(int y)
        {
            this.y = y;
        }

        public void incX(int x=0)
        {
            this.x += x;
        }

        public void incY(int y=0)
        {
            this.y += y;
        }

        public int getX()
        {
            return x;
        }

        public int getY()
        {
            return y;
        }

        public void setXY(int x, int y)
        {
            setX(x);
            setY(y);
        }

        public Point clone()
        {
            return new Point(x,y);
        }

    }
}
