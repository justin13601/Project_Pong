using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WMPLib;

namespace Project_Pong
{
    public partial class Form1 : Form
    {
        Graphics gr;
        WindowsMediaPlayer BackgroundPlayer = new WindowsMediaPlayer();

        PongGame Ball = new PongGame();

        public Form1()
        {
            InitializeComponent();
            pBGame.BackColor = Color.Black;

            gr = pBGame.CreateGraphics();

            btnStart.Enabled = true;
            btnPause.Enabled = false;
            btnReset.Enabled = false;
            lblWelcome.Visible = true;
            menuStrip1.Enabled = true;

            //Initializes Background music player
            BackgroundPlayer.URL = "Music.wav";
            BackgroundPlayer.settings.volume = 10;
            BackgroundPlayer.controls.play();

            //Sets initial mode as "Original" with a white ball
            btnOriginal.Visible = false;
            btnRainbow.Visible = true;
            Ball.RainbowMode = false;

            //Form fade in
            Opacity = 0;                                                //First sets the opacity is 0

            landingTimer.Interval = 10;                                 //Increases the opacity every 10ms
            landingTimer.Tick += new EventHandler(Landing);             //Calls the function that changes opacity 
            landingTimer.Start();
        }

        void Landing(object sender, EventArgs e)
        {
            if (Opacity >= 1)
            {
                landingTimer.Stop();   //this stops the timer if the form is completely displayed
            }
            else
            {
                Opacity += 0.01;
            }
        }

        public class PongGame
        {
            public Rectangle paddle1 = new Rectangle(14, 190, 20, 100);      //Paddle sizes and locations
            public Rectangle paddle2 = new Rectangle(764, 190, 20, 100);

            public Rectangle ball = new Rectangle(392, 115, 16, 16);         //Ball size and initial location

            public bool p1movesUp, p1movesDown, p2movesUp, p2movesDown;

            public SolidBrush myBrush = new SolidBrush(Color.White);

            public int p1Score = 10;                          //Integer that will store player 1 score
            public int p2Score;                          //Integer that will store player 2 score

            static int RandoMin = 1;                     //2 random integers used to randomize ball directions in the Randomize() method to avoid repetition of ball movement
            static int RandoMax = 3;

            public int Xspeed = -2;                      //Initial speeds
            public int Yspeed = 2;

            private readonly Random r = new Random();

            public bool RainbowMode = false;

            public void DrawGame(Graphics g)
            {
                //Draws paddles
                g.Clear(Color.Black);
                g.FillRectangle(myBrush, paddle2);
                g.FillRectangle(myBrush, paddle1);
            }

            public void DrawBall(Graphics g)
            {
                //Draws ball according to selected mode
                if (RainbowMode == true)
                {
                    Random rnd = new Random();
                    Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    SolidBrush myRainbowBrush = new SolidBrush(randomColor);
                    g.FillEllipse(myRainbowBrush, ball);
                }
                else
                {
                    g.FillEllipse(myBrush, ball);
                }
            }

            public void CheckIfMoving()
            {
                //If player press the key to move the paddle, this method changes the Y position of the paddle accordingly
                if (p1movesUp == true)
                {
                    if (paddle1.Y <= 0)
                    {
                        paddle1.Y = 0;
                    }
                    else
                    {
                        paddle1.Y -= 12;
                    }
                }
                if (p1movesDown == true)
                {
                    if (paddle1.Y >= 381)
                    {
                        paddle1.Y = 381;
                    }
                    else
                    {
                        paddle1.Y += 12;
                    }
                }

                if (p2movesUp == true)
                {
                    if (paddle2.Y <= 0)
                    {
                        paddle2.Y = 0;
                    }
                    else
                    {
                        paddle2.Y -= 12;
                    }
                }

                if (p2movesDown == true)
                {
                    if (paddle2.Y >= 381)
                    {
                        paddle2.Y = 381;
                    }
                    else
                    {
                        paddle2.Y += 12;
                    }
                }
            }

            public void Restart(int i)
            {
                //Method called upon a player scoring, resets game and ball position
                ball.X = 392;
                ball.Y = 115;
                if (i == 1)
                {
                    Xspeed = 1;
                }
                else
                {
                    Xspeed = -1;
                }

                Yspeed = 1;
                paddle1.Y = 190;
                paddle2.Y = 190;
                RandoMax = 3;
                RandoMin = 1;
            }

            public bool CheckScore()
            {
                //Checks if player scored, and increases scores accordingly
                if (ball.X < 1)
                {
                    p2Score += 1;
                    Restart(2);
                    PlayScoreSound();
                    return true;
                }
                else if (ball.X > 777)
                {
                    p1Score += 1;
                    Restart(1);
                    PlayScoreSound();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //Method that tests for win conditions (11 points to either player triggers win)
            public bool CheckWin()
            {
                if (p1Score >= 11)
                {
                    return true;
                }
                else if (p2Score >= 11)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void IncreaseSpeed()
            {
                //Increase both the normal speed and the results of any possible randomization in the Randomize() method
                RandoMin += 1;
                RandoMax += 1;
                Xspeed = Xspeed < 0 ? Xspeed -= 1 : Xspeed += 1;
            }

            public void MoveBall(Timer t1)
            {
                //Changes ball coordinates based on speed in both x & y axis
                ball.X += Xspeed;
                ball.Y += Yspeed;

                //If ball touches one of the table boundaries, its x/y speed gets a change in sign, and ball rebounds
                if (ball.Y > 463 || ball.Y < 8)
                {
                    Yspeed = -Yspeed;
                    PlayWallSound();
                }

                if (ball.IntersectsWith(paddle1) || ball.IntersectsWith(paddle2))
                {
                    //If the ball intersects the paddle "away" from the centre, the ball movement gets randomized, else its speed on the X axis is simply reverted
                    int dst = paddle1.Y + 100;
                    int Distance = dst - ball.Y;

                    if (Distance > 75 || Distance < 25)
                    {
                        Randomize();
                        PlayPaddleSound();
                    }
                    else
                    {
                        Xspeed = -Xspeed;
                        PlayPaddleSound();
                    }
                }
            }

            void Randomize()
            {
                //Uses RandoMin & RandoMax values to randomize the X speed of the ball
                int speed = r.Next(RandoMin, RandoMax);
                Xspeed = ball.IntersectsWith(paddle1) ? Xspeed = speed : Xspeed = -speed;

                //If ball is moving upward (negative Y speed), the random value assigned is changed in sign, allowing the ball to continue upward
                if (Yspeed < 0)
                {
                    int t = r.Next(RandoMin, RandoMax);
                    Yspeed = -t;
                }
                //Else, changes the Y speed to a positive value (moving downwards)
                else
                {
                    Yspeed = r.Next(RandoMin, RandoMax);
                }
            }

            //Plays game sound effects
            public void PlayWallSound()
            {
                //Sound for wall rebounds
                WindowsMediaPlayer player = new WindowsMediaPlayer();
                player.URL = "Pong Sound - Wall.wav";
            }
            public void PlayPaddleSound()
            {
                //Sound for paddle collision
                WindowsMediaPlayer player = new WindowsMediaPlayer();
                player.URL = "Pong Sound - Paddle.wav";
            }

            public void PlayScoreSound()
            {
                //Sound when either player scores
                WindowsMediaPlayer player = new WindowsMediaPlayer();
                player.URL = "Pong Sound - Score.wav";
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            Ball.DrawGame(gr);                      //Draws paddles & ball
            Ball.DrawBall(gr);
            Ball.MoveBall(gameTimer);               //Moves the ball
            Ball.CheckIfMoving();                   //Checks if player is controlling the paddle

            //Checks if player scored
            if (Ball.CheckScore() == true)
            {
                lblScore1.Text = Ball.p1Score.ToString();
                lblScore2.Text = Ball.p2Score.ToString();
            }
            else
            {
                return;
            }

            //Checks if either player has won the game
            if (Ball.CheckWin() == true)
            {
                if (Ball.p1Score >= 11)
                {
                    btnPause.PerformClick();
                    MessageBox.Show("Congratulations! Player 1 Wins!");
                    
                    //Resets selected mode
                    btnRainbow.Visible = true;
                    btnOriginal.Visible = false;
                    Ball.RainbowMode = false;

                    //Resets game board
                    Ball.Restart(2);
                    pBGame.Invalidate();
                }
                else if (Ball.p2Score >= 11)
                {
                    btnPause.PerformClick();
                    MessageBox.Show("Congratulations! Player 2 Wins!");

                    //Resets selected mode
                    btnRainbow.Visible = true;
                    btnOriginal.Visible = false;
                    Ball.RainbowMode = false;

                    //Resets game board
                    Ball.Restart(2);
                    pBGame.Invalidate();
                }
            }
            else
            {
                return;
            }

            pBGame.Invalidate();
        }

        private void speedTimer_Tick(object sender, EventArgs e)
        {
            //Increases overall speed every 3 seconds
            Ball.IncreaseSpeed();
        }

        //Detects keys (Key Ups) that control the paddle
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                //Keys W (Up) & S (Down) controls player 1 paddle
                case Keys.S:
                    Ball.p1movesDown = false;
                    return;
                case Keys.W:
                    Ball.p1movesUp = false;
                    return;

                //Keys P (Up) & L (Down) controls player 2 paddle
                case Keys.L:
                    Ball.p2movesDown = false;
                    return;
                case Keys.P:
                    Ball.p2movesUp = false;
                    return;
            }
        }
        //Detects keys (Key Downs) that control the paddle
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                //Keys W (Up) & S (Down) controls player 1 paddle
                case Keys.S:
                    Ball.p1movesDown = true;
                    return;
                case Keys.W:
                    Ball.p1movesUp = true;
                    return;
                //Keys P (Up) & L (Down) controls player 2 paddle
                case Keys.L:
                    Ball.p2movesDown = true;
                    return;
                case Keys.P:
                    Ball.p2movesUp = true;
                    return;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //Starts game timers and allows paddles to be controlled
            gameTimer.Interval = 20;
            gameTimer.Start();
            speedTimer.Start();
            this.KeyPreview = true;

            btnStart.Enabled = false;
            btnPause.Enabled = true;
            btnReset.Enabled = false;
            lblWelcome.Visible = false;
            menuStrip1.Enabled = false;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            //Temporarily pauses game timers and disables paddle controls
            gameTimer.Stop();
            speedTimer.Stop();
            this.KeyPreview = false;

            btnPause.Enabled = false;
            btnReset.Enabled = true;
            btnStart.Text = "Resume";
            btnStart.Enabled = true;
            menuStrip1.Enabled = true;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //Asks for confirmation from user when prompted for game reset
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to restart Pong? All unsaved scores will be cleared.", "Reset Game", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //Stops game timers
                gameTimer.Stop();
                speedTimer.Stop();

                btnReset.Enabled = false;
                btnPause.Enabled = false;
                btnStart.Text = "Play";
                btnStart.Enabled = true;
                lblWelcome.Visible = true;
                menuStrip1.Enabled = true;

                //Resets selected mode
                btnRainbow.Visible = true;
                btnOriginal.Visible = false;
                Ball.RainbowMode = false;

                //Clears scores
                Ball.p1Score = 0;
                Ball.p2Score = 0;

                lblScore1.Text = Ball.p1Score.ToString();
                lblScore2.Text = Ball.p2Score.ToString();

                //Resets game board
                Ball.Restart(2);
                pBGame.Invalidate();
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }
        }

        private void pBGame_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Ball.DrawGame(g);                      //Draws paddles & ball
        }

        //Allows user to save current scores
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //Notifies user if current Scoreboard is empty
                if (Ball.p2Score == 0 && Ball.p1Score == 0)
                {
                    DialogResult dialogResult = MessageBox.Show("The current scores are empty. Would you still like to continue with the save?", "Empty Scoreboard Found", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.No)
                    {
                        return;
                    }
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();       //Declares a new savefile dialog
                saveFileDialog.Title = "Save as a Scoreboard File";         //Sets the title of the dialog
                saveFileDialog.FileName = "Scores.scoreboard";              //Default file name if user chooses not to insert a new name
                saveFileDialog.Filter = "Scoreboard Files|*.scoreboard";    //Limits selection of file types

                if (saveFileDialog.ShowDialog() == DialogResult.OK)         //If user clicks 'save'
                {
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        BinaryWriter binaryWriter = new BinaryWriter(fs);

                        //Writes all scores into the file
                        binaryWriter.Write(Ball.p1Score);
                        binaryWriter.Write(Ball.p2Score);

                        binaryWriter.Flush();
                        binaryWriter.Close();
                    }
                    MessageBox.Show("Scoreboard successfully saved to " + "'" + saveFileDialog.FileName + "'.");
                }

            }
            catch
            {
                MessageBox.Show("Oops, something went wrong. Please try again");
            }
        }

        //Allows user to import existing scores to "continue" games
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                long length;

                OpenFileDialog openFileDialog = new OpenFileDialog();           //Declares openfile dialog
                openFileDialog.Title = "Open a Scoreboard File";                //Sets dialog title
                openFileDialog.Filter = "Scoreboard Files|*.scoreboard";        //Limits the file types a user can open

                if (openFileDialog.ShowDialog() == DialogResult.OK)             //If user clicks 'open'
                {
                    //Initiates reader
                    FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open);
                    BinaryReader binaryReader = new BinaryReader(fs);

                    //Retrieves the length of the binary file
                    length = binaryReader.BaseStream.Length;

                    //Reads data from binary file until the end of the file
                    while (fs.Position < length)
                    {
                        //Transfers information from the scoreboard file into the game
                        Ball.p1Score = binaryReader.ReadInt32();
                        Ball.p1Score = binaryReader.ReadInt32();
                    }

                    binaryReader.Close();

                    lblScore1.Text = Ball.p1Score.ToString();
                    lblScore2.Text = Ball.p2Score.ToString();
                    lblWelcome.Visible = false;
                    MessageBox.Show("Scoreboard Successfully Opened!");

                }
            }
            catch
            {
                MessageBox.Show("Oops, something went wrong. Your file may be corrupt or unreadable.");
            }
        }

        //Shortcuts for ease of access
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                saveToolStripMenuItem.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.O))
            {
                openToolStripMenuItem.PerformClick();
                return true;
            }
            if (keyData == (Keys.Control | Keys.R))
            {
                btnReset.PerformClick();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnOriginal_Click(object sender, EventArgs e)
        {
            //Disables rainbow mode
            btnOriginal.Visible = false;
            btnRainbow.Visible = true;
            Ball.RainbowMode = false;
        }

        private void btnRainbow_Click(object sender, EventArgs e)
        {
            //Enables rainbow mode
            btnOriginal.Visible = true;
            btnRainbow.Visible = false;
            Ball.RainbowMode = true;
        }
    }
}


