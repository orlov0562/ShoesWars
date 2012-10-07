using System;
using System.Drawing;

namespace GameTest001
{
    class Stars
    {
        private Point coord;
        private Color cl;
        private bool destroyed;
        private int size = 0;
        private int destroyedPositionX;
        private int destroyedPositionY;
        private int clSmooth;
        private int speed;
        private int x_offset;
        private int trans;

        public Stars(Point p, int size, int destroyedPositionX, int destroyedPositionY)
        {
            coord = p;
            this.size = size;
            this.destroyedPositionX = destroyedPositionX;
            this.destroyedPositionY = destroyedPositionY;
            destroyed = false;

            speed = (new Random()).Next(3, 10);
            x_offset = (new Random()).Next(-4, 4);
            trans = (new Random()).Next(75, 155);
        }

        public bool isDestroyed()
        {
            return destroyed;
        }

        public void draw(ref Bitmap drawArea)
        {
            if (destroyed) return;

            int x = coord.getX();
            int y = coord.getY();
            clSmooth = coord.getY() % 255;
            if (y >= 0)
            {
                Graphics g = Graphics.FromImage(drawArea);
                SolidBrush myBrush;
                cl = Color.FromArgb(trans, 255 - clSmooth, 255 - clSmooth / 4, (new Random()).Next(175, 255) - clSmooth / 8);
                myBrush = new SolidBrush(cl);
                g.FillRectangle(myBrush, x, y, size, size);
                g.Dispose();
            }

            coord.setY(coord.getY() + speed);

            coord.setX(coord.getX() + x_offset);

            if (coord.getY() > destroyedPositionY) destroyed = true;
            if (coord.getX() < 0) destroyed = true;
            if (coord.getX() > destroyedPositionX) destroyed = true;
        }

    }
}
