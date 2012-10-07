using System;
using System.Drawing;

namespace GameTest001
{
    class Opp
    {
        private Point coord;
        private bool outFromField = false;
        private bool destroyed = false;

        private int destroyedPositionX;
        private int destroyedPositionY;

        private int inc_x = 0;

        private int speed_y;
        private int speed_x;

        private Image oppimg;


        public Opp(Image oppimg, int x, int y, int destroyedPositionX, int destroyedPositionY)
        {
            coord = new Point(x, y);
            this.destroyedPositionX = destroyedPositionX;
            this.destroyedPositionY = destroyedPositionY;
            destroyed = false;
            int min_op_speed = Form1.MIN_OP_SPEED;
            if (min_op_speed < 1) min_op_speed = 1;
            speed_y = (new Random()).Next(min_op_speed, min_op_speed+3);
            speed_x = (new Random()).Next(min_op_speed, min_op_speed+3);
            this.oppimg = oppimg;
        }

        public void next_coord()
        {
            
            int x = 0;
            
            if (inc_x==0)
            {
                inc_x = (new Random()).Next(-1 * (destroyedPositionX / 4), destroyedPositionX / 4);
                x = 0;
            }
            else if (inc_x<0)
            {
                inc_x+=1;
                x = -1 * speed_x;
            }
            else if (inc_x>0)
            {
                inc_x -= 1;
                x = speed_x;
            }

            // проверяем выход за границы экрана и отражаем движение в другую сторону в случае такового

            if ((coord.getX() + x) <= 0)
            {
                x = speed_x;
                inc_x = (new Random()).Next(10, destroyedPositionX / 4);
            }

            if ((coord.getX()+x) >= (destroyedPositionX-oppimg.Width-10))
            {
                x = -1*speed_x;
                inc_x = (new Random()).Next( -1 * (destroyedPositionX / 4), -10);
            }

            // меняем координаты

            coord.incX(x);
            coord.incY(speed_y);

            if (coord.getY() > destroyedPositionY-oppimg.Height-8)
            {
                outFromField = true;
                destroyed = true;
            }

        }

        public bool isOut()
        {
            return outFromField;
        }

        public void draw(ref Bitmap DrawArea)
        {
            next_coord();
            if (destroyed) return;

            Graphics g = Graphics.FromImage(DrawArea);
            SolidBrush myBrush;

            int x = coord.getX();
            int y = coord.getY();

            Graphics.FromImage(DrawArea).DrawImage(oppimg, new System.Drawing.Point(x,y));

            // рамка вокруг врага, надо для отладки
            /*
            Pen myPen = new Pen(Color.BlanchedAlmond);
            g.DrawRectangle(myPen, x+4, y+10, oppimg.Width+5, oppimg.Height); // центральная точка            
             */

            myBrush = new SolidBrush(Color.DimGray);
            Graphics.FromImage(DrawArea).DrawImage(oppimg, new System.Drawing.Point(x, y));
            int offset_x = 11; if (speed_y > 10) offset_x = 9;
            g.DrawString(speed_y.ToString(), new Font("Arial", 8), myBrush, x + offset_x, y + 30);
            g.Dispose();
        }

        public bool isCoordIn(Point checkCoord, int gun) // gun -> 0=left, 1=right
        {
            bool ret = false;

            int x = checkCoord.getX();
            if (gun == 0) x -= 56;
            if (gun == 1) x += 54;
            int y = checkCoord.getY()-27;

            int rx1 = coord.getX()+4;
            int ry1 = coord.getY()+10;

            int rx2 = coord.getX() + oppimg.Width + 5;
            int ry2 = coord.getY() + oppimg.Height;

            if (gun==0 && x >= rx1 && x <= rx2 && y >= ry1 && y <= ry2) ret = true;
            if (gun==1 && x >= rx1 && x <= rx2 && y >= ry1 && y <= ry2) ret = true;

            return ret;
        }

        public bool isDestroyed()
        {
            return destroyed;
        }

        public void setDestroyed(bool destroyed)
        {
            this.destroyed = destroyed;
        }

    }
}
