using FlugbahnLib;
using Svg;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Media;
using System.Xml.Serialization;

namespace Game
{
    public partial class GameDlg : Form
    {
        private List<Insekt> insekten = new();
        private List<Image> schlaeger = new();
        private List<int> uniqueNumbers = new();

        private SoundPlayer click = new();
        private SoundPlayer player = new();

        private bool bGameStarted;
        private bool bGameOver = true;

        private int iGetroffen = 0;
        private int iZeit = 0;
        private int nAnzahlMouseClicked = 0;
        private int nWidth = 10;
        private int nHeight = 10;
        private int iDurchLandingZone;

        private Pos mouseClickPos = new Pos();

        private Rectangle rectEssen;

        private int nInsektSize = 50;

        private SvgDocument svgDocFood = SvgDocument.Open<SvgDocument>(AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\..\\pics\\pizza_0.svg");

        public GameDlg()
        {
            InitializeComponent();
            SetStartButtonPosition();
            Refresh();
        }

        private static List<int> GenerateUniqueNumbers(int range)
        {
            if (range < 0 || range > 10)
                throw new ArgumentException("Der Bereich muss zwischen 0 und 10 liegen.");

            List<int> numbers = Enumerable.Range(0, range).ToList();
            Random random = new Random();

            for (int i = 0; i < range; i++)
            {
                int randomIndex = random.Next(i, range);
                int temp = numbers[i];
                numbers[i] = numbers[randomIndex];
                numbers[randomIndex] = temp;
            }

            return numbers;
        }

        private void SchlaegerBilder()
        {
            for (int i = 0; i < 11; i++)
            {
                schlaeger.Add(Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\..\\pics\\Fliegenklatsche_gelb.jpg"));
            }
        }

        private void RemoveImageFromSchlaeger(int index)
        {
            if (index >= 0 && index < schlaeger.Count)
            {
                schlaeger.RemoveAt(index);
                Refresh();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            schlaeger.Clear();
            SchlaegerBilder();

            uniqueNumbers = GenerateUniqueNumbers(2);

            // Variable reset
            iGetroffen = 0;
            nAnzahlMouseClicked = 0;
            iZeit = 0;
            iDurchLandingZone = 0;
            Game.InsektHitBoxWidth = nInsektSize;
            Game.InsektHitBoxHeight = nInsektSize;
            mouseClickPos.X = Game.MOUSE_POS_X_INIT;
            mouseClickPos.Y = Game.MOUSE_POS_Y_INIT;

            ReadXML(null, null);

            // Timer
            timerCreate.Enabled = true;
            timerCreate.Start();

            timerPlayBack.Enabled = true;
            timerPlayBack.Start();
            timerZeitUebrig.Start();

            bGameStarted = true;
            bGameOver = false;

            // Visibility
            btnStart.Visible = false;
            lblGameOver.Visible = false;
            lblInterval.Visible = true;
            lblTreffen.Visible = true;
            lblZeitUebrig.Visible = true;

            // Label-Content
            lblTreffen.Text = "Insekt " + iGetroffen + "x" + " getroffen";
            lblInterval.Text = "Geschwindigkeit: " + timerPlayBack.Interval + " ms";
            lblInterval.Text = "Geschwindigkeit: " + timerPlayBack.Interval + " ms";
            lblZeitUebrig.Text = "Zeit uebrig: " + (Game.MAX_SPIEL_ZEIT - iZeit) + " sek";
        }

        private void GameDlg_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            if (SvgDocument.SystemIsGdiPlusCapable() == false)
            {
                // Wenn wir hier sind, ist es gar nicht gut. Dann ist es ein ganz altes Windows
                // das noch kein GDI+ unterstuetzt.
                return;
            }

            DrawBackground(g);

            DrawSchlaeger(g);

            DrawFood(g);

            DrawFoodHitBox(g);

            DrawInsekt(g);

            DrawMausClickKreis(g);
        }

        private void DrawMausClickKreis(Graphics g)
        {
            if (nAnzahlMouseClicked > Game.MAX_SCHLAEGE)
            {
                mouseClickPos.X = Game.MOUSE_POS_X_INIT;
                mouseClickPos.Y = Game.MOUSE_POS_Y_INIT;
            }
            else
            {
                g.DrawEllipse(Pens.Red, mouseClickPos.X - 51, mouseClickPos.Y - 51, 100, 100);
            }
        }

        /// <summary>
        /// Insekt zeichnen
        /// </summary>
        /// <param name="g">Graphics</param>
        private void DrawInsekt(Graphics g)
        {
            // Insekten zeichen
            foreach (Insekt insekt in insekten)
            {
                if (insekt.IsStarted == false)
                {
                    continue;
                }

                var posOfInsect = insekt.positionen[insekt.IndexOfPosition];
                var posInsektInPixel = ProzentInPixelUmwandeln(posOfInsect.XPos, posOfInsect.YPos);

                // Hitbox des Insekts
                Point point = new Point((int)posInsektInPixel.X, (int)posInsektInPixel.Y);
                insekt.HitBox = new Rectangle(point, new Size(Game.InsektHitBoxWidth, Game.InsektHitBoxHeight));

                // Mittelpunkt von Hitbox herausfinden
                Point pOffSet = new Point(insekt.HitBox.Width / 2, insekt.HitBox.Height / 2);

                var rect = insekt.HitBox;
                rect.Offset(-pOffSet.X, -pOffSet.Y);
                insekt.HitBox = rect;

                // Draw HitBox
                if (insekt.Tod == false)
                {
                    g.DrawRectangle(Pens.Green, insekt.HitBox);
                }

                // wir muessen mindesten 2 Position haben fuer Flugbahnberechnung
                if (insekt.positionen.Count < 2)
                {
                    continue;
                }

                // Runden auf 3 Stellen
                var xPosProzent = Math.Round(insekt.positionen[insekt.IndexOfPosition].XPos, 3);
                var yPosProzent = Math.Round(insekt.positionen[insekt.IndexOfPosition].YPos, 3);

                DrawFly(g, ProzentInPixelUmwandeln(xPosProzent, yPosProzent), insekt);

                if (insekt.positionen.Count - 3 < insekt.IndexOfPosition)
                {
                    insekt.IndexOfPosition = 0;
                }
                else
                {
                    insekt.IndexOfPosition += 1;
                }

                // Insekt Landingzone
                if (insekt.HitBox.IntersectsWith(rectEssen))
                {
                    if (insekt.StatusBefore == Insekt.InsektStatus.NONE && insekt.StatusNow == Insekt.InsektStatus.NONE)
                    {
                        insekt.StatusNow = Insekt.InsektStatus.ENTER;
                        iDurchLandingZone++;
                        lblLandingZone.Text = iDurchLandingZone + "x durch LandingZone geflogen";
                    }
                }

                if (!insekt.HitBox.IntersectsWith(rectEssen))
                {
                    insekt.StatusNow = Insekt.InsektStatus.NONE;
                    insekt.StatusBefore = Insekt.InsektStatus.NONE;
                }
                if (iDurchLandingZone == Game.MAX_FLIEGE_LANDINGZONE)
                {
                    GameOver();
                }
            }

            CheckIfGameOver();
        }

        /// <summary>
        /// Fliege zeichnen in Pixel
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="pos">Position in Pixel</param>
        /// <param name="insekt">Insekt</param>
        private void DrawFly(Graphics g, Pos pos, Insekt insekt)
        {
            var winkel = 0f;
            var eQuadrant = Quadrant.NONE;

            var bmpPic = insekt.Pic.Draw(nInsektSize, nInsektSize);
            var pic = bmpPic;

            var posFirst = pos;

            var posKurt = insekt.positionen[insekt.IndexOfPosition + 1];

            var posNext = ProzentInPixelUmwandeln(posKurt.XPos, posKurt.YPos);

            Debug.WriteLine("Position: " + GetQuadrant(posFirst, posNext));

            // Sonderfaelle fuer unendliche Steigung  (waagrecht, senkrecht)
            if ((int)posFirst.X == (int)posNext.X || (int)posFirst.Y == (int)posNext.Y)
            {
                if (insekt.Tod == false)
                {
                    // gleiche X-Position, aber ungleiche Y-Position
                    if ((int)posFirst.X == (int)posNext.X && (int)posFirst.Y != (int)posNext.Y)
                    {
                        if ((int)posFirst.Y < (int)posNext.Y)
                        {
                            // nach unten
                            Debug.WriteLine("SONDERFALL: UNTEN gleiche X-Position, aber ungleiche Y-Position");
                            pic.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else
                        {   // nach oben
                            Debug.WriteLine("SONDERFALL: OBEN gleiche X-Position, aber ungleiche Y-Position");
                            pic.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                        }
                    }
                    // gleiche Y-Position, aber ungleiche X-Position
                    else if ((int)posFirst.Y == (int)posNext.Y && (int)posFirst.X != (int)posNext.X)
                    {
                        if ((int)posFirst.X <= ((int)posNext.X))
                        {
                            // nach rechts
                            Debug.WriteLine("SONDERFALL: RECHTS gleiche Y-Position, aber ungleiche X-Position");
                            pic.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                        else
                        {
                            // nach links
                            Debug.WriteLine("SONDERFALL: LINKS gleiche Y-Position, aber ungleiche X-Position");
                            pic.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                    }
                }

                if (DrawFlyDeath(g, pos, insekt, pic) == true)
                {
                    return;
                }

                g.DrawImage(pic, pos.X - pic.Width / 2f, pos.Y - pic.Height / 2f);
            }
            else
            {
                // Normalfall
                winkel = GetWinkelDerPositionen(posFirst, posNext);
                eQuadrant = GetQuadrant(posFirst, posNext);
                DrawPic(g, pos, eQuadrant, winkel, insekt);
            }
        }

        /// <summary>
        /// Zeichnet das tote Insekt. Es faellt langsam zum Boden
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="p">Position in Pixel</param>
        /// <param name="insekt">Insekt</param>
        /// <param name="pic">Bild des Insekts</param>
        /// <returns></returns>
        private bool DrawFlyDeath(Graphics g, Pos p, Insekt insekt, Bitmap pic)
        {
            if (insekt.Tod)
            {
                if (insekt.TodesPosition.X == -1)
                {
                    insekt.TodesPosition.X = p.X;
                    insekt.TodesPosition.Y = p.Y;
                }
                if (insekt.TodesPosition.Y < Height - pic.Height)
                {
                    insekt.TodesPosition.Y += 5; // wie schnell nach unten
                }
                g.DrawImage(pic, insekt.TodesPosition.X - pic.Width / 2, insekt.TodesPosition.Y - pic.Height / 2);
                return true;
            }
            return false;
        }

        private void DrawPic(Graphics g, Pos p, Quadrant quadrant, float fWinkel, Insekt insekt)
        {
            var bmpPic = insekt.Pic.Draw(nInsektSize, nInsektSize);
            var pic = bmpPic;

            if (DrawFlyDeath(g, p, insekt, pic) == true)
            {
                return;
            }

            switch (quadrant)
            {
                case Quadrant.ONE:
                    g.DrawImage(RotateImage(pic, 90f - fWinkel), p.X - pic.Width / 2, p.Y - pic.Height / 2);
                    break;

                case Quadrant.TWO:
                    g.DrawImage(RotateImage(pic, fWinkel + 90f), p.X - pic.Width / 2, p.Y - pic.Height / 2);
                    break;

                case Quadrant.THREE:
                    g.DrawImage(RotateImage(pic, -90f - fWinkel), p.X - pic.Width / 2, p.Y - pic.Height / 2);
                    break;

                case Quadrant.FOUR:
                    g.DrawImage(RotateImage(pic, -1 * (90 - fWinkel)), p.X - pic.Width / 2, p.Y - pic.Height / 2);
                    break;

                case Quadrant.NONE:
                    break;
            }
        }

        /// <summary>
        /// Liefert den Winkel zwischen zwei Punkten in Grad
        /// </summary>
        /// <param name="posFirst">Erster Punkt</param>
        /// <param name="posNext">Naechster Punkt</param>
        /// <returns>Winkel in Grad</returns>
        private float GetWinkelDerPositionen(Pos posFirst, Pos posNext)
        {
            // Steigung (m) aus den Koordinaten zweier Punkte der Geraden wird sie so berechnet:
            // y2 - y1
            // ------- = m
            // x2 - x1

            var fSteigung = (Math.Abs(posFirst.Y - posNext.Y)) / (Math.Abs(posNext.X - posFirst.X));

            // DEBUGGING only
            //
            //Debug.WriteLine("Steigung m: " + fSteigung
            //    + " Winkel: radiant: " + Math.Atan(fSteigung)
            //    + " Grad: " + RadianToDegree(Math.Atan(fSteigung)));

            return (float)GlobalValues.RadianToDegree(Math.Atan(fSteigung));
        }

        /// <summary>
        /// method to rotate an image either clockwise or counter-clockwise
        /// </summary>
        /// <param name="img">the image to be rotated</param>
        /// <param name="rotationAngle">the angle (in degrees).
        /// NOTE:
        /// Positive values will rotate clockwise 1..180
        /// negative values will rotate counter-clockwise -1..-180
        /// FROM Stackoverflow Tony the Lion
        /// : https://stackoverflow.com/questions/2163829/how-do-i-rotate-a-picture-in-winforms
        /// </param>
        /// <returns>rotated Image</returns>
        private Image RotateImage(Image img, float rotationAngle)
        {
            //create an empty Bitmap image
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            //turn the Bitmap into a Graphics object
            Graphics gfx = Graphics.FromImage(bmp);

            //now we set the rotation point to the center of our image
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

            //now rotate the image
            gfx.RotateTransform(rotationAngle);

            //move image back
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

            //set the InterpolationMode to HighQualityBicubic so to ensure a high
            //quality image once it is transformed to the specified size
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //now draw our new image onto the graphics object
            gfx.DrawImage(img, new Point(0, 0));

            //dispose of our Graphics object
            gfx.Dispose();

            //return the image
            return bmp;
        }

        private void DrawFoodHitBox(Graphics g)
        {
            var pInPixel = ProzentInPixelUmwandeln(
                GlobalValues.LANDING_ZONE_POS_X_PERCENT,
                GlobalValues.LANDING_ZONE_POS_Y_PERCENT);

            var p = new Point((int)pInPixel.X, (int)pInPixel.Y);

            var w = this.ClientSize.Width / GlobalValues.LANDING_ZONE_WIDTH_AND_HEIGHT_VALUE;

            rectEssen.Width = (int)w;
            rectEssen.Height = (int)w;
            rectEssen.X = (int)(p.X - w / 2);
            rectEssen.Y = (int)(p.Y - w / 2);

            // Zeichnet die  HitBox der Landing-Zone
            g.DrawRectangle(Pens.Blue,
                (int)(p.X - w / 2),
                (int)(p.Y - w / 2),
                (int)w, (int)w);
        }

        private void DrawFood(Graphics g)
        {
            var w = this.ClientSize.Width / (int)GlobalValues.LANDING_ZONE_WIDTH_AND_HEIGHT_VALUE;
            var bmpPic = svgDocFood.Draw(w, w);

            var pInPixel = ProzentInPixelUmwandeln(
                GlobalValues.LANDING_ZONE_POS_X_PERCENT,
                GlobalValues.LANDING_ZONE_POS_Y_PERCENT);

            g.DrawImage(bmpPic, pInPixel.X - bmpPic.Width / 2, pInPixel.Y - bmpPic.Height / 2);
        }

        private void DrawSchlaeger(Graphics g)
        {
            try
            {
                var left = nWidth - 65;
                var top = nHeight - 110;
                for (int i = 0; i < schlaeger.Count - 1; i++)
                {
                    g.DrawImage(schlaeger[i], left, top, 50, 100);
                    //g.DrawImage(schlaeger[i + 1], left - 50, top, 50, 100);
                    left -= 50;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Prozent in Pixel umwandeln
        /// </summary>
        /// <param name="x">X-Pos in Prozent</param>
        /// <param name="y">Y-Pos in Prozent</param>
        /// <returns>new Pos()</returns>
        private Pos ProzentInPixelUmwandeln(double x, double y)
        {
            // Prozent in pixel umwandeln
            var xPosition = (int)(x * this.ClientSize.Width);
            var yPosition = (int)(y * this.ClientSize.Height);

            return new Pos() { X = xPosition, Y = yPosition };
        }

        private void DrawBackground(Graphics g)
        {
            // Zeichnet die Torte oder Pizza symbolisch
            var pInPixel = ProzentInPixelUmwandeln(
                GlobalValues.LANDING_ZONE_POS_X_PERCENT,
                GlobalValues.LANDING_ZONE_POS_Y_PERCENT);

            var p = new Point((int)pInPixel.X, (int)pInPixel.Y);

            var w = this.ClientSize.Width / GlobalValues.LANDING_ZONE_WIDTH_AND_HEIGHT_VALUE;

            g.DrawEllipse(Pens.Red,
                p.X - w / 2,
                p.Y - w / 2,
                w, w);
        }

        private void GameDlg_Resize(object sender, EventArgs e)
        {
            lblGameOver.Location = new Point(ClientSize.Width / 2, ClientSize.Height / 3) - lblGameOver.Size / 2;
            nWidth = this.ClientSize.Width;
            nHeight = this.ClientSize.Height;
            Game.InsektHitBoxWidth = nInsektSize;
            Game.InsektHitBoxHeight = nInsektSize;
            nInsektSize = Width / 10;
            Refresh();
            SetStartButtonPosition();
        }

        private void GameDlg_ResizeEnd(object sender, EventArgs e)
        {
            lblGameOver.Location = new Point(ClientSize.Width / 2, ClientSize.Height / 3) - lblGameOver.Size / 2;
            nWidth = this.ClientSize.Width;
            nHeight = this.ClientSize.Height;
            Game.InsektHitBoxWidth = nInsektSize;
            Game.InsektHitBoxHeight = nInsektSize;
            nInsektSize = Width / 10;
            Refresh();
            SetStartButtonPosition();
        }

        private void SetStartButtonPosition()
        {
            btnStart.Location = new Point(
                ClientSize.Width / 2,
                ClientSize.Height / 2) - btnStart.Size / 2;
        }

        private void ReadXML(object sender, EventArgs e)
        {
            for (int i = 0; i < uniqueNumbers.Count; i++)
            {
                try
                {
                    var sPath = AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\..\\flugbahnen\\flugbahn" + i + ".xml";

                    XmlSerializer seria = new XmlSerializer(typeof(List<Pos>));
                    using (FileStream fs = new FileStream(sPath, FileMode.Open))
                    {
                        var insekt = new Insekt();
                        insekt.Id = i;
                        insekt.positionen.Clear();
                        insekt.positionen = (List<Pos>)seria.Deserialize(fs);

                        // @"sounds\click\sound"" + numclick + ""_hall.wav";
                        // SvgDocument.Open<SvgDocument>
                        var sPathPic = @"pics\insekt" + i + ".svg";
                        insekt.Pic = SvgDocument.Open<SvgDocument>(sPathPic);

                        if (i == 0)
                        {
                            insekt.IsStarted = true;
                        }
                        insekten.Add(insekt);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("FEHLER: beim XML-File lesen: " + ex.Message + " StackTrace: " + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Liefert den Quadrant im Koordinatensystem anhand von zwei verschiedenen Positionen
        /// TopRight: 1. Quadrant, TopLeft: 2. Quadrant, BottomLeft: 3. Quaddrant, BottomRight: 4. Quadrant
        /// </summary>
        /// <param name="posFirst">Erste Position</param>
        /// <param name="posNext">Zweite Position</param>
        /// <returns>Quadrant</returns>
        private Quadrant GetQuadrant(Pos posFirst, Pos posNext)
        {
            if (posFirst.X <= posNext.X && posFirst.Y >= posNext.Y)
                return Quadrant.ONE;
            else if (posFirst.X <= posNext.X && posFirst.Y <= posNext.Y)
                return Quadrant.TWO;
            else if (posFirst.X >= posNext.X && posFirst.Y <= posNext.Y)
                return Quadrant.THREE;
            else if (posFirst.X >= posNext.X && posFirst.Y >= posNext.Y)
                return Quadrant.FOUR;
            else
                return Quadrant.NONE;
        }

        private void timerPlayBack_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void GameDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerPlayBack.Stop();
        }

        private void GameDlg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                GameOver();
            }
        }

        private void GameDlg_Load(object sender, EventArgs e)
        {
            SetStartButtonPosition();

            mouseClickPos.X = Game.MOUSE_POS_X_INIT;
            mouseClickPos.Y = Game.MOUSE_POS_Y_INIT;

            // Variablen
            iZeit = 0;

            // Label in Koordinatenpunkte setzen
            lblGameOver.Location = new Point((int)this.ClientSize.Width / 2, (int)this.ClientSize.Height / 3);

            // Labels verstecken

            lblGameOver.Visible = false;
            lblInterval.Visible = false;
            lblTreffen.Visible = false;
            lblZeitUebrig.Visible = false;

            nWidth = this.ClientSize.Width;
            nHeight = this.ClientSize.Height;
            Refresh();
        }

        private void GameDlg_MouseDown(object sender, MouseEventArgs e)
        {
            // Wir brauchen das fuer den Klickkreis beim DrawMausClickKreis()
            mouseClickPos.X = e.X;
            mouseClickPos.Y = e.Y;

            if (bGameStarted == false)
                return;

            RemoveImageFromSchlaeger(0);

            Random rand = new Random();
            int numclick = rand.Next(1, 11);

            click.SoundLocation = @"sounds\click\sound" + numclick + "_hall.wav";
            click.LoadAsync();

            if (nAnzahlMouseClicked < Game.MAX_SCHLAEGE)
            {
                nAnzahlMouseClicked++;
                click.Play();
            }
            else
            {
                GameOver();
                return;
            }

            // Hitbox Cursor

            foreach (var insekt in insekten)
            {
                if (insekt.Tod)
                {
                    continue;
                }

                var rectCursor = new Rectangle(e.X, e.Y, 100, 100);

                var posOfInsect = insekt.positionen[insekt.IndexOfPosition];
                var posInsektInPixel = ProzentInPixelUmwandeln(posOfInsect.XPos, posOfInsect.YPos);

                // Mittelpunkt von Hitbox herausfinden
                Point ptMitte = new Point(
                    (int)posInsektInPixel.X - insekt.HitBox.Width / 2,
                    (int)posInsektInPixel.Y - insekt.HitBox.Height / 2);

                // Mittelpunkt von Hitbox herausfinden
                rectCursor.X = e.X - 50;
                rectCursor.Y = e.Y - 50;

                // WENN Maus Position == Insektposition DANN ist es getroffen
                if (rectCursor.IntersectsWith(insekt.HitBox))
                {
                    Random rand2 = new Random();
                    int numschrei = rand2.Next(1, 6);

                    iGetroffen++;
                    lblTreffen.Text = "Insekt " + iGetroffen + "x" + " getroffen";
                    lblInterval.Text = "Geschwindigkeit: " + timerPlayBack.Interval + " ms";
                    player = new SoundPlayer("../../../sounds/schreie/scream" + numschrei + "_hall.wav");
                    player.Play();
                    InsektGeschlagen(insekt); ;
                }
            }
        }

        private void GameOver()
        {
            iZeit = Game.MAX_SPIEL_ZEIT;
            nAnzahlMouseClicked = Game.MAX_SCHLAEGE;
            bGameStarted = false;
            bGameOver = true;

            // Timer
            timerPlayBack.Stop();
            timerZeitUebrig.Stop();

            timerCreate.Stop();
            timerCreate.Enabled = false;

            lblGameOver.Text = "GAME OVER";
            lblGameOver.Font = new Font("Arial", 55, FontStyle.Bold);

            // Visibility
            btnStart.Visible = true;
            btnStart.Enabled = true;
            lblGameOver.Visible = true;

            // Label in Koordinatenpunkte setzen
            lblGameOver.Location = new Point(ClientSize.Width / 2, ClientSize.Height / 3) - lblGameOver.Size / 2;

            // Label-Content
            lblZeitUebrig.Text = "Zeit uebrig: " + (Game.MAX_SPIEL_ZEIT - iZeit) + "sek";

            insekten = new();

            Invalidate();
            Refresh();
        }

        private void InsektGeschlagen(Insekt insekt)
        {
            insekt.Tod = true;

            //SchlaegerBilder(insekt);
            //if (timerPlayBack.Interval > FLIEGE_MIN_GESCHWINDIGKEIT)
            //{
            //    timerPlayBack.Interval -= FLIEGE_GESCHWINDIGKEIT_AENDERUNG;
            //}
            //else if (timerPlayBack.Interval <= FLIEGE_MIN_GESCHWINDIGKEIT)
            //{
            //    timerPlayBack.Interval = timerPlayBack.Interval;
            //}
        }

        private void timerZeitUebrig_Tick(object sender, EventArgs e)
        {
            if (iZeit < Game.MAX_SPIEL_ZEIT)
            {
                iZeit++;
            }
            else if (iZeit >= Game.MAX_SPIEL_ZEIT)
            {
                GameOver();
                lblZeitUebrig.Text = "Zeit uebrig: " + iZeit + " sek";
            }
            lblZeitUebrig.Text = "Zeit uebrig: " + (Game.MAX_SPIEL_ZEIT - iZeit) + " sek";
        }

        private void timerCreate_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("Create timer");

            for (int i = 0; i < insekten.Count; i++)
            {
                if (insekten[i].IsStarted == false)
                {
                    insekten[i].IsStarted = true;
                    break;
                }
            }
        }

        private void CheckIfGameOver()
        {
            bool bIsOver = true;

            foreach (var insekt in insekten)
            {
                if (insekt.Tod == false)
                {
                    bIsOver = false;
                    break;
                }
            }

            if (bIsOver && !bGameOver)
            {
                GameOver();
            }
        }
    }
}