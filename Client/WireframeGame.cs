using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlowtorchesAndGunpowder
{
    public partial class WireframeGame : Form
    {
        private Font fOutputFont = new Font("Arial", 8);
        private BufferedGraphicsContext fContext;
        private BufferedGraphics fGrafx;
        private Stopwatch fStopWatch = new Stopwatch();
        private TimeSpan fTotalTimeElapsed;
        private TimeSpan fTotalTimeElapsedWhenUpdateScreen;
        private TimeSpan fTotalTimeElapsedWhenLastShot;
        private bool fPause = false;

        Pen fHeroShotPen = new Pen(Color.YellowGreen);
        Pen fHeroShipPen = new Pen(Color.Cornsilk);
        Ship fHeroShip = new Ship();
        List<Shot> fHeroShotList = new List<Shot>();
        GameClient fGameClient = new GameClient();

        public WireframeGame() : base()
        {
            InitializeComponent();

            this.Text = "WireframeGame";
            this.SetClientSizeCore(800, 800);
            this.Resize += new EventHandler(this.OnResize);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            //Cursor.Hide();

            Application.Idle += new EventHandler(OnApplicationIdle);

            // Retrieves the BufferedGraphicsContext for the 
            // current application domain.
            fContext = BufferedGraphicsManager.Current;

            // Sets the maximum size for the primary graphics buffer
            // of the buffered graphics context for the application
            // domain.  Any allocation requests for a buffer larger 
            // than this will create a temporary buffered graphics 
            // context to host the graphics buffer.
            fContext.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);

            // Allocates a graphics buffer the size of this form
            // using the pixel format of the Graphics created by 
            // the Form.CreateGraphics() method, which returns a 
            // Graphics object that matches the pixel format of the form.
            fGrafx = fContext.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));

            // Draw the first frame to the buffer.
            //DoChange();
            //DrawToBuffer(grafx.Graphics);
            //this.WindowState = FormWindowState.Maximized;
            Task.Run(() => fGameClient.Start());
            fGameClient.SendMessage(new ClientEvent(ClientEventEnum.Joining).GetAsJson());
            fStopWatch.Start();
            fTotalTimeElapsed = fStopWatch.Elapsed;
            fTotalTimeElapsedWhenUpdateScreen = fTotalTimeElapsed;
            fTotalTimeElapsedWhenLastShot = fTotalTimeElapsed;
        }
        private void  OnApplicationIdle(object sender, EventArgs e)
        {
            if (User32Import.GetKeyState(Keys.Escape))
            {
                if (fPause)
                    return;
                fPause = true;
                //Cursor.Show();
                GameSettingsForm settingsForm = new GameSettingsForm(new Settings("localhost", 4567));
                DialogResult settingsResult = settingsForm.ShowDialog(this);
                if (settingsResult == DialogResult.Abort)
                {
                    fGameClient.Close();
                    Close();
                }
                else if (settingsResult == DialogResult.Cancel)
                    fPause = false;
                else if (settingsResult == DialogResult.OK)
                {
                    //_settings = settingsForm.GetSettings();
                    fPause = false;
                }
                //Cursor.Hide();
            }
            if (fPause)
                return;
            while (User32Import.AppStillIdle())
            {
                // LOOP
                TimeSpan timeElapsedNow = fStopWatch.Elapsed;
                TimeSpan timeElapsedFromLast = timeElapsedNow - fTotalTimeElapsed;
                fTotalTimeElapsed = timeElapsedNow;

                ProcessKeyState(timeElapsedFromLast);
                DoChange(timeElapsedFromLast);
                //CheckBounds(new RectangleF(0, 0, this.ClientSize.Width - size, this.ClientSize.Height - size));

                TimeSpan timeElapsedFromLastUpdateScreen = timeElapsedNow - fTotalTimeElapsedWhenUpdateScreen;
                if (timeElapsedFromLastUpdateScreen.Milliseconds > 16)
                {
                    fTotalTimeElapsedWhenUpdateScreen = timeElapsedNow;
                    UpdateScreen();
                }
            }
        }
        private void ProcessKeyState(TimeSpan aTimeElapsed)
        {
            var rotation = RotationEnum.None;
            var forwardThrustor = false;
            var shooting = false;
            if (User32Import.GetKeyState(Keys.Left))
            {
                fHeroShip.RotateLeft(aTimeElapsed);
                rotation = RotationEnum.Left;
            }
            else if (User32Import.GetKeyState(Keys.Right))
            {
                fHeroShip.RotateRight(aTimeElapsed);
                rotation = RotationEnum.Right;
            }
            if (User32Import.GetKeyState(Keys.Up))
            {
                fHeroShip.EngageForwardThrustors(aTimeElapsed);
                forwardThrustor = true;
            }
            if (User32Import.GetKeyState(Keys.Space) && (fTotalTimeElapsed - fTotalTimeElapsedWhenLastShot).Milliseconds > fHeroShip.GetBulletDelay())
            {
                fHeroShotList.Add(new Shot(fTotalTimeElapsed, fHeroShip.GetPosition(), fHeroShip.GetDirection(), fHeroShip.GetSpeedVector()));
                fTotalTimeElapsedWhenLastShot = fTotalTimeElapsed;
                shooting = true;
            }
            //TODO: Don't send all the time...
            if(rotation != RotationEnum.None || forwardThrustor || shooting)
                fGameClient.SendMessage(new ClientAction(rotation, forwardThrustor, shooting).GetAsJson());
        }
        private void DoChange(TimeSpan aTimeElapsed)
        {
            fHeroShip.CalcNewPosition(aTimeElapsed, this.ClientRectangle);
            int i = 0;
            while (i < fHeroShotList.Count)
            {
                if (fHeroShotList[i].IsTimeToRemove(fTotalTimeElapsed))
                    fHeroShotList.RemoveAt(i);
                else
                {
                    fHeroShotList[i].CalcNewPosition(aTimeElapsed, this.ClientRectangle);
                    i++;
                }
            }
        }
        private void UpdateScreen()
        {
            // Draw to the buffer.
            DrawToBuffer(fGrafx.Graphics);
            // draw in the paint method.
            this.Refresh();
        }
        private void OnResize(object sender, EventArgs e)
        {
            // Re-create the graphics buffer for a new window size.
            fContext.MaximumBuffer = new Size(this.ClientSize.Width + 1, this.ClientSize.Height + 1);
            if (fGrafx != null)
            {
                fGrafx.Dispose();
                fGrafx = null;
            }
            fGrafx = fContext.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));
            UpdateScreen();
        }
        private void DrawToBuffer(Graphics g)
        {
            // Clear the graphics buffer
            g.Clear(Color.Black);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            // Draw information strings.
            //g.DrawString("Click enter to toggle timed display refresh " + timer1.Enabled.ToString() , OutputFont, Brushes.White, 10, 10);
            g.DrawString(string.Format("Direction: {0:F2} radians", fHeroShip.GetDirection()), fOutputFont, Brushes.White, 10, 34);
            String[] allRows = fGameClient.GetLog();
            g.DrawString(String.Join(Environment.NewLine, allRows) + Environment.NewLine, fOutputFont, Brushes.LightBlue, new RectangleF(10, 50, 1900, 200));
            //Draw local graphics
            g.DrawLines(fHeroShipPen, fHeroShip.GetWorldPoints());
            for (int i = 0; i < fHeroShotList.Count; i++)
                g.DrawLines(fHeroShotPen, fHeroShotList[i].GetWorldPoints());
            //Draw server graphics
            var gameState = fGameClient.GetGameState();
            foreach(var playerShip in gameState.fPlayerShip)
            {
                var translatedPoints = RenderUtil.GetWorldPoints(
                        RenderUtil.ShipLocalPoints,
                        playerShip.Value.fPositionX,
                        playerShip.Value.fPositionY,
                        playerShip.Value.fDirection
                        );

                if (playerShip.Key == fGameClient.GetClientIndex())
                {
                    g.FillPolygon(Brushes.DarkSlateGray, translatedPoints);
                    g.DrawLines(fHeroShipPen, translatedPoints);
                }
                else
                {
                    g.FillPolygon(Brushes.DarkRed, translatedPoints);
                    g.DrawLines(fHeroShipPen, translatedPoints);
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            fGrafx.Render(e.Graphics);
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Idle -= OnApplicationIdle;
            fStopWatch.Stop();
            //Cursor.Show();
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WireframeGame
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WireframeGame";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }
        private void GoFullscreen(bool fullscreen)
        {
            if (fullscreen)
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.Bounds = Screen.PrimaryScreen.Bounds;
                this.Activate();
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
        } 
    }
}

