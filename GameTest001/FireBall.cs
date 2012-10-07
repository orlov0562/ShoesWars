using System;
using System.Drawing;

namespace GameTest001
{
    class FireBall
    {
        private Point coord;
        private bool destroyed;
        private int fbCorrecting = 6;
        private int gun = 0;

        public FireBall(Point p, int gun)
        {
            coord = p;
            this.gun = gun;
            destroyed = false;
        }

        public bool isDestroyed()
        {
            return destroyed;
        }

        public void correctCoord(Point coord)
        {
            if (this.coord.getY() == coord.getY())
            {
                this.coord.setX(coord.getX());
            }
        }

        public void draw(ref Bitmap drawArea)
        {
            if (destroyed) return;

            Graphics g = Graphics.FromImage(drawArea);
            SolidBrush myBrush;
            
            int x1 = 0;
            
            if (gun == 0)
            {
                x1 = coord.getX() - 58 - fbCorrecting/2;
            }
            else
            {
                x1 = coord.getX() + 52 - fbCorrecting / 2;
            }

            int y1 = coord.getY()-30;

            myBrush = new SolidBrush(Color.Yellow);
            Pen myPen = new Pen(myBrush, 3);
            g.DrawLine(myPen, x1 + 3 + fbCorrecting/2, y1, x1 + 3 + fbCorrecting/2, y1 - 1); // центральная точка            

            // пути патронов, до верха, надо для отладки
            /*
            myBrush = new SolidBrush(Color.White);
            myPen = new Pen(myBrush, 1);
            g.DrawLine(myPen, coord.getX() - 56, 0, coord.getX() - 56, coord.getY()-35);
            g.DrawLine(myPen, coord.getX() + 54, 0, coord.getX() + 54, coord.getY()-35);
             */

            myBrush = new SolidBrush(Color.Red);
            g.FillEllipse(myBrush, x1 + 1, y1 + 3, 10 - 6 + fbCorrecting, 14); // центральная точка
            
            myBrush = new SolidBrush(Color.SkyBlue);
            g.FillEllipse(myBrush, x1, y1, 12-6+fbCorrecting, 11); // центральная точка
            
            if (fbCorrecting > 0) fbCorrecting--;

            coord.setY(coord.getY() - 5 - 6 + fbCorrecting);

            if (coord.getY() <= 0) destroyed = true;

            g.Dispose();
        }

        public Point getCoords()
        {
            return coord;
        }

        public void setDestroyed(bool destroyed)
        {
            this.destroyed = destroyed;
        }

        public int getGun()
        {
            return gun;
        }





    }
}
