using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace GameTest001
{
    public partial class Form1 : Form
    {
        private const int SHIP_SPEED = 20;
        private const int STARS_COUNT = 250;

        private static int MAX_AMMO_COUNT = 0;
        public static int MIN_OP_SPEED=0;
        private int OPP_COUNT = 0;
        public static int LIVES = 0;
        private int OppKilled = 0;
        private int KilledForNextRound = 0;
        private Ship ship;
        private List<FireBall> ammo;
        private List<Stars> stars;
        private bool movingRight = false;
        private bool movingLeft = false;
        private Image background;
        private bool FireReturn = false;
        private SoundPlayer firePlayer;
        private SoundPlayer explosion;
        private bool sound = false;
        private Image kote;
        private int koteHide = 0;
        private int koteShow = 0;
        private int koteShowSec = 0;
        private int koteShowCount = 1;
        private bool pause = false;
        private List<Opp> opp;
        private Image oppimg;
        private int CurrentLevel = 1;
        private bool isFirstStart = true;
        private bool isCursorShow = false;
       

        public Form1()
        {
            InitializeComponent();
            
            ship = new Ship(150, pictureBox1.Size.Height-75);
            ammo = new List<FireBall>();
            stars = new List<Stars>();
            background = Image.FromFile("bg.jpg");
            kote = Image.FromFile("kotik.png");
            oppimg = Image.FromFile("opp.png");
            

            Random rnd = new Random();
            while ((STARS_COUNT/4) > stars.Count)
            {
                Color cl = Color.White;
                Point point = new Point(
                    rnd.Next(1, pictureBox1.Width),
                    rnd.Next(1, pictureBox1.Height)
                );
                int size = rnd.Next(1, 3);

                stars.Add(new Stars(point, size, pictureBox1.Width, pictureBox1.Height));
            }

            firePlayer = new System.Media.SoundPlayer();
            firePlayer.SoundLocation = @"fire.wav";

            explosion = new SoundPlayer();
            explosion.SoundLocation = @"explosion.wav";

            opp = new List<Opp>();

            setInitialValues();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isFirstStart)
            {
                firstStart();
                Application.DoEvents();
                return;
            }
           
            if (LIVES<=0)
            {
                gameover();
                Application.DoEvents();
                return;
            }

            if (pause)
            {
                pauseShow();
                Application.DoEvents();
                return;
            }

            redraw();
        }

        private void firstStart()
        {
            Bitmap DrawArea = getDrawArea();
            drawStars(ref DrawArea);
            Graphics.FromImage(DrawArea).DrawImage(kote,
                                                   new System.Drawing.Point(
                                                       pictureBox1.Width - kote.Width - 20,
                                                       pictureBox1.Height - kote.Height + 10
                                                   )
            );

            drawDialog(ref DrawArea, pictureBox1.Width - 100 - kote.Width - 10, pictureBox1.Height - kote.Height - 25, "Hello, dude!", 8, 15);
            
            pictureBox1.Image = DrawArea;
            pictureBox1.Refresh();
            if (!isCursorShow)
            {
                Cursor.Show();
                isCursorShow = true;
            }
        }

        private void pauseShow()
        {
            Bitmap DrawArea = getDrawArea();
            drawStars(ref DrawArea);
            Graphics.FromImage(DrawArea).DrawImage(kote,
                                                   new System.Drawing.Point(
                                                       pictureBox1.Width - kote.Width - 20,
                                                       pictureBox1.Height - kote.Height + 10
                                                   )
            );


            drawDialog(ref DrawArea, pictureBox1.Width - 100 - kote.Width - 10, pictureBox1.Height - kote.Height - 25, "Paused!", 22, 15);
            drawInfo(ref DrawArea);
            pictureBox1.Image = DrawArea;
            pictureBox1.Refresh();
            if (!isCursorShow)
            {
                Cursor.Show();
                isCursorShow = true;
            }
        }

        private void gameover()
        {
            
            Bitmap DrawArea = getDrawArea();
            drawStars(ref DrawArea);
            Graphics.FromImage(DrawArea).DrawImage(kote,
                                                   new System.Drawing.Point(
                                                       pictureBox1.Width - kote.Width - 20,
                                                       pictureBox1.Height - kote.Height+10
                                                   )
            );


            drawDialog(ref DrawArea, pictureBox1.Width - 100 - kote.Width-10, pictureBox1.Height - kote.Height-25, "Game over", 10, 15);
            drawInfo(ref DrawArea);
            pictureBox1.Image = DrawArea;
            pictureBox1.Refresh();
            button1.Visible = true;
            if (!isCursorShow)
            {
                Cursor.Show();
                isCursorShow = true;
            }
        }

        private void drawDialog(ref Bitmap DrawArea, int x, int y, string str, int str_x, int str_y)
        {
            Graphics g = Graphics.FromImage(DrawArea);
            SolidBrush myBrush;
            myBrush = new SolidBrush(Color.White);

            g.FillEllipse(myBrush, x, y, 100, 50);
            g.FillRectangle(myBrush, x+80, y+25, 20, 20);

            myBrush = new SolidBrush(Color.DarkRed);
            g.DrawString(str, new Font("Tahoma", 11), myBrush, x+str_x, y+str_y);
            g.Dispose();
        }

        private void drawStars(ref Bitmap DrawArea)
        {
            for (int i = stars.Count - 1; i >= 0; i--)
            {
                Stars fb = stars[i];
                if (fb.isDestroyed())
                {
                    stars.Remove(fb);
                    continue;
                }
            }

            Random rnd = new Random();
            while (STARS_COUNT > stars.Count)
            {
                Color cl = Color.White;
                Point point = new Point(
                    rnd.Next(1, pictureBox1.Width),
                    rnd.Next(-2 * pictureBox1.Height, 0)
                );
                int size = rnd.Next(1, 3);

                stars.Add(new Stars(point, size, pictureBox1.Width, pictureBox1.Height));
            }

            foreach (Stars fb in stars) fb.draw(ref DrawArea);            
        }

        private void redraw()
        {
            
            if (movingRight) movingRightAction();
            if (movingLeft) movingLeftAction();

            Bitmap DrawArea = getDrawArea();
            
            // STARS
            drawStars(ref DrawArea);

            // Check ammo and opponent

            foreach (Opp op in opp)
            {
                foreach (FireBall fb in ammo)
                {
                    if (!fb.isDestroyed() && op.isCoordIn(fb.getCoords(), fb.getGun()))
                    {
                        op.setDestroyed(true);
                        fb.setDestroyed(true);
                        if (sound) explosion.Play();
                        OppKilled++;
                        KilledForNextRound++;
                        if (KilledForNextRound >= OPP_COUNT * 5)
                        {
                            nextLevelValues();
                            KilledForNextRound=0;
                        }
                    }
                }
            }

            // OPPONENT

            for (int i = opp.Count - 1; i >= 0; i--)
            {
                Opp op = opp[i];
                if (op.isDestroyed())
                {
                    if (op.isOut())
                    {
                        LIVES--;
                    }

                    opp.Remove(op);
                    continue;
                }
            }

            Random rnd = new Random();
            while (OPP_COUNT > opp.Count)
            {
                opp.Add(new Opp(oppimg, rnd.Next(0, pictureBox1.Width - oppimg.Width - 10), -1 * oppimg.Height, pictureBox1.Width, pictureBox1.Height));
            }

            foreach (Opp op in opp) op.draw(ref DrawArea);

            // AMMO

            for (int i = ammo.Count - 1; i >= 0; i--)
            {
                FireBall fb = ammo[i];
                if (fb.isDestroyed())
                {
                    ammo.Remove(fb);
                    continue;
                }
                fb.correctCoord(ship.getCoord());
                fb.draw(ref DrawArea);
            }

            // Kote

            if (koteShowCount > 0)
            {
                Graphics.FromImage(DrawArea).DrawImage(kote,
                                                       new System.Drawing.Point(
                                                           pictureBox1.Width - kote.Width - 20,
                                                           pictureBox1.Height - koteHide
                                                       )
                );
            }

            // SHIP

            ship.draw(ref DrawArea);
            ship.drawAmmo(ref DrawArea, Form1.MAX_AMMO_COUNT - ammo.Count);

            if (FireReturn)
            {
                ship.addCoord(0, -5);
                FireReturn = false;
            }

            drawInfo(ref DrawArea);

            pictureBox1.Image = DrawArea;
            pictureBox1.Refresh();

            Application.DoEvents();                        
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode.ToString())
            {
                case "Right":
                        movingRight = true;
                    break;
                case "Left":
                        movingLeft = true;
                    break;

                case "Space":
                    fire();
                    break;
                case "M":
                    sound = !sound;
                    break;
            }
        }

        private void movingRightAction()
        {
            if (ship.getCoord().getX()>(pictureBox1.Width-75))
            {
                ship.setShipGoToReset();
                return;
            }
            ship.addCoord(SHIP_SPEED, 0);
            ship.setShipGoToRight(true);
        }

        private void movingLeftAction()
        {
            if (ship.getCoord().getX() < 75)
            {
                ship.setShipGoToReset();
                return;
            }

            ship.addCoord(-1*SHIP_SPEED, 0);
            ship.setShipGoToLeft(true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode.ToString())
            {
                case "Right":
                    movingRight = false;
                    break;
                case "Left":
                    movingLeft = false;
                    break;
            }
        }

        private void fire()
        {
            if (ammo.Count >= MAX_AMMO_COUNT-1) return;
            ammo.Add( new FireBall( ship.getCoord().clone(), 0 ) );
            ammo.Add( new FireBall( ship.getCoord().clone(), 1 ) );
            if (!FireReturn)
            {
                ship.addCoord(0, 5);
                FireReturn = true;
            }
            if (sound) firePlayer.Play();
        }

        private void drawInfo(ref Bitmap DrawArea)
        {
            Graphics g = Graphics.FromImage(DrawArea);
            SolidBrush myBrush;

            int line = 0;
            myBrush = new SolidBrush(Color.White);
            int lives = LIVES<=0 ? 0 : LIVES; 
            g.DrawString(" Lives:" + lives.ToString(), new Font("Arial", 8), myBrush, 5, 5 + line * 12); line++;

            myBrush = new SolidBrush(Color.Tomato);
            g.DrawString(" Level:" + CurrentLevel.ToString(), new Font("Arial", 8), myBrush, 5, 5 + line * 12); line++;
            g.DrawString(" Killed:" + OppKilled.ToString() + " (+" + (OPP_COUNT * 5 - KilledForNextRound) + ")", new Font("Arial", 8), myBrush, 5, 5 + line * 12); line++;
            g.DrawString(" Rockets:" + MAX_AMMO_COUNT.ToString(), new Font("Arial", 8), myBrush, 5, 5 + line * 12); line++;
            g.DrawString(" Opponents:" + OPP_COUNT.ToString(), new Font("Arial", 8), myBrush, 5, 5 + line * 12); line++;
            g.DrawString(" Sound (M):" + (sound ? "On" : "Off"), new Font("Arial", 8), myBrush, 5, 5 + line * 12); line++;
        }

        private Bitmap getDrawArea()
        {
            Bitmap DrawArea = new Bitmap(background);
            Graphics g = Graphics.FromImage(DrawArea);
            return DrawArea;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int fx = e.Location.X;

            if (fx < 75) fx = 75;
            if (fx > pictureBox1.Size.Width - 75) fx = pictureBox1.Size.Width - 75;

            if (fx < (ship.getCoord().getX()-3)) ship.setShipGoToLeft(true); 
            else if (fx > (ship.getCoord().getX()+3)) ship.setShipGoToRight(true);

            ship.setCoord(fx,ship.getCoord().getY());
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            if (!pause && isCursorShow)
            {
                Cursor.Hide();
                isCursorShow = false;
            }

        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {

            if (!pause && !isCursorShow)
            {
                Cursor.Show();
                isCursorShow = true;
            }

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Application.DoEvents();            
            if (pause) return;
            koteShow++;
            if ( (koteShowCount>0) && (koteShow % 15 == 0) && !timer3.Enabled && !timer4.Enabled)
            {
                timer4.Enabled = true;
                koteShow = 0;
                if (sound)
                {
                    SoundPlayer meow = new SoundPlayer();
                    meow.SoundLocation = @"meow.wav";
                    meow.Play();
                }
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (pause) return;
            fire();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (pause) {
                Application.DoEvents();            
                return;
            }
            if (koteShowSec-- > 0) return;
            if (koteHide > 0) koteHide -= 1;
            if (koteHide <= 0)
            {
                timer3.Enabled = false;
                koteShowCount--;
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            if (pause)
            {
                Application.DoEvents();
                return;
            }
            if (koteHide < 55) koteHide += 1;
            if (koteHide >= 55)
            {
                timer4.Enabled = false;
                timer3.Enabled = true;
                koteShowSec = 50;
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            pause = true;

            if (!isCursorShow)
            {
                Cursor.Show();
                isCursorShow = true;
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (pause)
            {
                pause = false;
                if (isCursorShow)
                {
                    Cursor.Hide();
                    isCursorShow = false;
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isFirstStart) Cursor.Hide();
            isFirstStart = false;
            if (isCursorShow)
            {
                Cursor.Hide();
                isCursorShow = false;
            }

            button1.Visible = false;
            setInitialValues();

        }

        private void setInitialValues()
        {
            MAX_AMMO_COUNT = 8;
            MIN_OP_SPEED = 1;
            OPP_COUNT = 4;
            OppKilled = 0;
            KilledForNextRound = 0;
            ammo.Clear();
            opp.Clear();
            
            Random rnd = new Random();
            
            while (OPP_COUNT > opp.Count)
            {
                opp.Add(
                    new Opp(
                        oppimg,
                        rnd.Next(0, pictureBox1.Width - oppimg.Width - 10),
                        -1 * oppimg.Height + rnd.Next(-50, 50),
                        pictureBox1.Width,
                        pictureBox1.Height)
                   );
            }

            CurrentLevel = 1;
            LIVES = 3;
            pause = false;
        }

        private void  nextLevelValues()
        {
            CurrentLevel++;

            if (OPP_COUNT < 5) OPP_COUNT += 3;
            else if (OPP_COUNT >= 10 && OPP_COUNT < 15) OPP_COUNT += 2;
            else if (OPP_COUNT >= 15) OPP_COUNT += 1;
            
            MAX_AMMO_COUNT = OPP_COUNT * 2;
            if (MAX_AMMO_COUNT > 50) MAX_AMMO_COUNT = 50;
            
            MIN_OP_SPEED = CurrentLevel/3;
            
            LIVES += 3;
            if (LIVES > 10) LIVES = 10;
        }


    }
}
