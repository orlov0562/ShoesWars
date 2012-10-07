using System;
using System.Drawing;

namespace GameTest001
{
    class Ship
    {
        const int FIRE_OFFSET_TIME = 3;

        private Point coord;
        private bool shipGoToLeft = false;
        private bool shipGoToRight = false;
        private int fireOffsetTime = 0;

        public Ship()
        {
            coord = new Point();
        }

        public Ship(int x, int y)
        {
            coord = new Point(x, y);
        }

        public void setShipGoToLeft(bool state)
        {
            setShipGoToReset();
            shipGoToLeft = state;
            fireOffsetTime = FIRE_OFFSET_TIME;
        }

        public void setShipGoToRight(bool state)
        {
            setShipGoToReset();
            shipGoToRight = state;
            fireOffsetTime = FIRE_OFFSET_TIME;
        }

        public void setShipGoToReset()
        {
            shipGoToLeft = false;
            shipGoToRight = false;
            this.fireOffsetTime = 0;
        }


        public Point getCoord()
        {
            return coord;
        }

        public void setCoord(int x, int y)
        {
            coord.setXY(x, y);
        }

        public void addCoord(int x, int y)
        {
            coord.setXY(coord.getX() + x, coord.getY() + y);
        }

        public void draw(ref Bitmap DrawArea)
        {
            Graphics g = Graphics.FromImage(DrawArea);
            Pen myPen = new Pen(Brushes.Black);
            SolidBrush myBrush;

            int x2 = 100;
            int y2 = 20;

            int x1 = coord.getX() - x2/2;
            int y1 = coord.getY() - y2/2;

            myBrush = new SolidBrush(Color.Black);
            g.FillRectangle(myBrush, x1, y1, x2, y2 ); // тело корабля
            
            myBrush = new SolidBrush(Color.MidnightBlue);
            g.FillRectangle(myBrush, x1+25, y1-20, x2-50, y2+40); // тело корабля

            myBrush = new SolidBrush(Color.Black);
            g.FillRectangle(myBrush, x1 + 35, y1 - 30, x2 - 70, y2 + 40); // тело корабля

            g.FillEllipse(myBrush, x1+34, y1-50, 31, 40);


            // башня корабля
            myBrush = new SolidBrush(Color.Gray);
            g.FillEllipse(myBrush, x1+42, y1-35, 15, 20);
            myBrush = new SolidBrush(Color.Gainsboro);
            g.FillEllipse(myBrush, x1 + 43, y1 - 35, 13, 16);
            myBrush = new SolidBrush(Color.White);
            g.FillEllipse(myBrush, x1 + 45, y1 - 35, 9, 10);

            drawGun(g, x1-10, y1); // дуло слева
            drawGun(g, x1+100, y1); // дуло справа

            g.Dispose();
        }

        private void drawGun(Graphics g, int x, int y)
        {
            SolidBrush myBrush;
            Pen myPen;


            if ( (shipGoToLeft || shipGoToRight) && fireOffsetTime > 0)
            {
                fireOffsetTime--;
                if (fireOffsetTime <= 0) setShipGoToReset();
            }

            // огонь
            drawFire(g, Color.Peru, x-1, y);
            drawFire(g, Color.Tomato, x + 1, y);
            drawFire(g, Color.Red, x, y);

            // дуло
            myBrush = new SolidBrush(Color.SkyBlue);
            g.FillRectangle(myBrush, x, y - 10, 10, 40);
            myPen = new Pen(Brushes.Black);
            g.DrawLine(myPen, x + 5, y+25, x + 5, y + 35);
            myPen = new Pen(Brushes.Black);
            g.DrawRectangle(myPen, x, y - 10, 10, 40);
        }

        private void drawFire(Graphics g, Color cl, int x, int y)
        {
            SolidBrush myBrush = new SolidBrush(cl);
            Random rdn = new Random();
            int r = rdn.Next(0, 2);
            for (int i = 0; i < 10; i++)
            {
                int cr = rdn.Next(0, 2);

                int fx = x;
                fx += r == 0 ? i/2 : 0;   // постоянное смещение для текущего кадра
                fx += (i > 0 && (cr == 0)) ? 2 : 0; // смещение текущего оконька
                
                // смещение огня если корабль поворачивает влево
                if (shipGoToLeft) 
                {
                    fx += (i > 0) ? (int)Math.Round(i * 1.5) : 0; 
                }

                // смещение огня если корабль поворачивает вправо
                if (shipGoToRight)
                {
                    fx -= (i > 0) ? (int) Math.Round(i*1.5) : 0;
                }


                int fy = y + 25;    // постоянное смещение
                fy += i*5;  // смещение огоньков

                int fw = 10 - i; // ширина огонька

                int fh = 15 - i; // высота огонька

                g.FillEllipse(myBrush, fx,  fy, fw, fh);

            }            
        }

        public void drawAmmo(ref Bitmap DrawArea, int ammo)
        {
            Graphics g = Graphics.FromImage(DrawArea);
            SolidBrush myBrush = new SolidBrush(Color.White);
            int x = coord.getX();
            int y = coord.getY();
            int offset_x = 0;
            if (ammo >= 10) offset_x = 5;

            g.DrawString(ammo.ToString(), new Font("Arial", 10), myBrush, x-5-offset_x, y-5);            


        }

    }
}
