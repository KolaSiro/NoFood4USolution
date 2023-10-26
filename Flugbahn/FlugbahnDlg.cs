

using FlugbahnLib;
using Svg;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

namespace Flugbahn
{
    public partial class FlugbahnDlg : Form
    {
        public float widthProzent = 0.0f;
        public float heightProzent;
        private bool bRecording = false;

        // Positionen
        public List<Pos> positionen = new List<Pos>();

        // Zufaellige Groessenaenderung der Fliege
        public List<int> deltas = new List<int>();

        private int idxDeltas = 0;

        /// <summary>
        /// Index der Positionenliste
        /// </summary>
        private int nIdx = 0;

        private SvgDocument svgDoc = SvgDocument.Open<SvgDocument>(AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\..\\pics\\herz.svg");

        public FlugbahnDlg()
        {
            widthProzent = 0;
            InitializeComponent();

            Cursor cur = System.Windows.Forms.Cursors.Hand;
            this.Cursor = cur;
            Cursor.Clip = ClientRectangle;

            CreateDeltaValues();
        }

        private void CreateDeltaValues()
        {
            for (int grad = 0; grad < 360;)
            {
                if (grad != 180)
                {
                    double angle = Math.PI * grad / 180.0;
                    double sinAngle = Math.Sin(angle);
                    sinAngle = Math.Abs(sinAngle);
                    sinAngle *= 20;
                    sinAngle = Math.Round(sinAngle, 2);

                    //Console.WriteLine("Sinus Winkel : " + sinAngle + " von " + grad + " Grad");
                    deltas.Add((int)sinAngle); // explicit cast
                    idxDeltas++; ;
                }
                grad += 10;
            }
            idxDeltas = 0;
        }

        private void FlugbahnDlg_Load(object sender, EventArgs e)
        {
            lblResolution.Text = (this.ClientSize.Width + " x " + this.ClientSize.Height);
        }

        private void FlugbahnDlg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F1)
            {
                MessageBox.Show("Doku:  "
                    + "F1: Help\nF5 REC\nF6 REC Stop\nF7 Xml Write\nF8 Xml Read\nF9 Play\nF10 Stop");
            }
            else if (e.KeyData == Keys.F5)
            {
                // Aufzeichnung beginnen
                bRecording = true;
                timerRecording.Start();
            }
            else if (e.KeyData == Keys.F6)
            {
                // Aufzeichnung stoppen
                bRecording = false;
                timerRecording.Stop();
            }
            else if (e.KeyData == Keys.F7)
            {
                // Daten in XML ablegen
                WriteXml();
            }
            else if (e.KeyData == Keys.F8)
            {
                // Daten Lesen
                ReadXml();
            }
            else if (e.KeyData == Keys.F9)
            {
                timerPlayBack.Start();
            }
            else if (e.KeyData == Keys.F10)
            {
                timerPlayBack.Stop();
            }
            else
            {
                // nix tun
            }
        }

        private void WriteXml()
        {
            XmlSerializer seria = new XmlSerializer(typeof(List<Pos>));

            try
            {

                using (FileStream fs = new FileStream(Application.StartupPath + "\\flugbahn.xml",
                    FileMode.Create))
                {
                    seria.Serialize(fs, positionen);
                    positionen.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FEHLER: beim XML-File schreiben: "
                    + ex.Message + " StackTrace: " + ex.StackTrace);
            }
        }

        private void ReadXml()
        {
            XmlSerializer seria = new XmlSerializer(typeof(List<Pos>));

            try
            {
                using (FileStream fs = new FileStream(Application.StartupPath + "\\flugbahn.xml",
                    FileMode.Open))
                {
                    positionen = (List<Pos>)seria.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FEHLER: beim XML-File lesen: "
                + ex.Message + " StackTrace: " + ex.StackTrace);
            }
        }

        private void FlugbahnDlg_MouseMove(object sender, MouseEventArgs e)
        {
            widthProzent = (float)e.X / this.ClientSize.Width;
            heightProzent = (float)e.Y / this.ClientSize.Height;

            var relativePoint = this.PointToClient(MousePosition);
            Point pPnt = new Point(relativePoint.X - 45, relativePoint.Y - 45);
            lblMaus.Text = ("X: " + e.X + "px (" + Math.Round(widthProzent, 3) + ") / Y: " + e.Y + "px (" + Math.Round(heightProzent, 3) + ")");
        }

        private void FlugbahnDlg_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            DrawBackground(g);

            // Debug only
            // -------------------------------------------------------
            Pen penRed = new Pen(Color.Red, 3);

            g.DrawLine(penRed, 0, this.ClientSize.Height / 2, this.ClientSize.Width, this.ClientSize.Height / 2);

            g.DrawLine(new Pen(Color.Blue, 3), this.ClientSize.Width / 2, 0, this.ClientSize.Width / 2, this.ClientSize.Height);

            // Debug finished
            // -----------------------------------------------------------------

            // Debug only 1 Kreis
            // -----------------------------------------------------------------
            {
                if (positionen.Count < 1)
                    return;

                if (timerPlayBack.Enabled == false)
                {
                    return;
                }

                var pos = positionen[nIdx];
                var xPosProzent = Math.Round(pos.XPos, 3);
                var yPosProzent = Math.Round(pos.YPos, 3);

                // Fliege zeichnen
                DrawFly(g, ProzentInPixelUmwandeln(xPosProzent, yPosProzent));

                // Debug only
                // ------------------------
                var pInPixel = ProzentInPixelUmwandeln(xPosProzent, yPosProzent);
                var p1 = new Point((int)pInPixel.X, (int)pInPixel.Y);
                var p2 = Point.Add(p1, new Size(1, 0)); // (xPosition, yPosition);

                g.DrawLine(new Pen(Color.Red, 3), p1, p2);
                g.DrawEllipse(Pens.Red, p1.X, p1.Y, 5, 5);

                // ------------------------------------
                // Debug end
            }

            // Debug finished
            // ----------------------------------------------------------------------
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

        /// <summary>
        /// Fliege zeichnen in Pixel
        /// </summary>
        /// <param name="g">Grafikschnittstelle</param>
        /// <param name="pos">Position in Pixel</param>
        /// <param name="nIdx">Index in der Positionenliste</param>
        private void DrawFly(Graphics g, Pos pos)
        {
            var bmpPic = svgDoc.Draw();
            var pic = bmpPic;

            // http://svg-net.github.io/SVG/doc/Q&A.html#how-to-render-an-svg-document-to-a-bitmap-in-another-size

            var b = SvgDocument.SystemIsGdiPlusCapable();

            // Groesse des SVG
            svgDoc.Height = pic.Height - idxDeltas; // e.ClipRectangle.Height / 4;

            svgDoc.Width = pic.Width - idxDeltas;

            // svg zu bitmap umwandeln.

            // g.DrawImage(bmpPic, 100, 100, bmpPic.Width, bmpPic.Height);

            if (svgDoc.Width <= 50)
            {
                svgDoc.Width = 389;
            }

            if (svgDoc.Height <= 50)
            {
                svgDoc.Height = 233;
            }

            var delta = deltas[idxDeltas];
            idxDeltas++;
            if (idxDeltas >= deltas.Count)
            {
                idxDeltas = 0;
            }

            //var pic = Properties.Resources.fly_icon3;
            var winkel = 0f;
            var eQuadrant = Quadrant.NONE;

            // wenn es min. 2 Positionen hat
            if (positionen.Count > 1 && nIdx < positionen.Count - 1)
            {
                //var posFirst = positionen[idx];
                var posFirst = pos;
                //var posNext = positionen[idx + 1];
                var posNext = ProzentInPixelUmwandeln(positionen[nIdx + 1].XPos, positionen[nIdx + 1].YPos);

                Debug.WriteLine("Position: " + GetQuadrant(posFirst, posNext));

                // Sonderfaelle fuer unendliche Steigung  (waagrecht, senkrecht)
                if ((int)posFirst.X == (int)posNext.X || (int)posFirst.Y == (int)posNext.Y)
                {
                    //var bild = Properties.Resources.fly_icon3;
                    var bild = bmpPic;

                    // gleiche X-Position, aber ungleiche Y-Position
                    if ((int)posFirst.X == (int)posNext.X && (int)posFirst.Y != (int)posNext.Y)
                    {
                        if ((int)posFirst.Y < (int)posNext.Y)
                        {
                            // nach unten
                            Debug.WriteLine("SONDERFALL: UNTEN gleiche X-Position, aber ungleiche Y-Position");
                            bild.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        else
                        {   // nach oben
                            Debug.WriteLine("SONDERFALL: OBEN gleiche X-Position, aber ungleiche Y-Position");
                            bild.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                        }
                    }
                    // gleiche Y-Position, aber ungleiche X-Position
                    else if ((int)posFirst.Y == (int)posNext.Y && (int)posFirst.X != (int)posNext.X)
                    {
                        if ((int)posFirst.X <= ((int)posNext.X))
                        {
                            // nach rechts
                            Debug.WriteLine("SONDERFALL: RECHTS gleiche Y-Position, aber ungleiche X-Position");
                            bild.RotateFlip(RotateFlipType.Rotate270FlipXY);
                        }
                        else
                        {
                            // nach links
                            Debug.WriteLine("SONDERFALL: LINKS gleiche Y-Position, aber ungleiche X-Position");
                            bild.RotateFlip(RotateFlipType.Rotate90FlipXY);
                        }
                    }

                    g.DrawImage(bild, pos.X - bild.Width / 2f, pos.Y - bild.Height / 2f);
                }
                else
                {
                    // Normalfall
                    winkel = GetWinkelDerPositionen(posFirst, posNext);
                    eQuadrant = GetQuadrant(posFirst, posNext);
                    DrawPic(g, pos, eQuadrant, winkel);
                }
            }

            g.DrawEllipse(new Pen(Color.Black), pos.Mitte().X, pos.Mitte().Y, pos.Breite, pos.Hoehe);
        }

        /// <summary>
        /// Liefert den Quadrant im Koordinatensystem anhand von zwei verschiedenen Positionen
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
        /// Zeichnet Objekte im richtigen Winkel anhand des Quadranten
        /// im kartesischen X/Y-Diagramm
        /// </summary>
        /// <param name="g">Grafik Engine</param>
        /// <param name="p">Position in Pixel</param>
        /// <param name="quadrant">Quadrant zwischen zwei Punkten</param>
        /// <param name="fWinkel">Winkel zwischen zwei Punkten in Grad</param>
        private void DrawPic(Graphics g, Pos p, Quadrant quadrant, float fWinkel)
        {
            var bmpPic = svgDoc.Draw();
            var pic = bmpPic;

            // http://svg-net.github.io/SVG/doc/Q&A.html#how-to-render-an-svg-document-to-a-bitmap-in-another-size

            var b = SvgDocument.SystemIsGdiPlusCapable();

            // Groesse des SVG
            svgDoc.Height = pic.Height + idxDeltas; // e.ClipRectangle.Height / 4;

            svgDoc.Width = pic.Width + idxDeltas;

            if (svgDoc.Width <= 50)
            {
                svgDoc.Width = 389;
            }

            if (svgDoc.Height <= 50)
            {
                svgDoc.Height = 233;
            }

            // svg zu bitmap umwandeln.

            // g.DrawImage(bmpPic, 100, 100, bmpPic.Width, bmpPic.Height);

            var delta = deltas[idxDeltas];
            idxDeltas++;
            if (idxDeltas >= deltas.Count)
            {
                idxDeltas = 0;
            }

            //var pic = Properties.Resources.fly_icon3;

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

        /// <summary>
        /// Fuegt die Mausposition zur Liste und wandelt den Wert von
        /// Pixel zu Prozent
        /// </summary>
        /// <param name="x">X-Pos in Pixel</param>
        /// <param name="y">Y-Pos in Pixel</param>
        private void AddMousePosToList(float x, float y)
        {
            float xPosProzent = x / this.ClientSize.Width;
            float yPosProzent = y / this.ClientSize.Height;

            float xPosProzentRunden = (float)Math.Round(xPosProzent, 3);
            float yPosProzentRunden = (float)Math.Round(yPosProzent, 3);

            lblMausPosition.Text = "X/Y: " + x + "/" + y;
            positionen.Add(new Pos(xPosProzentRunden, yPosProzentRunden));
        }


        private void timerRecording_Tick(object sender, EventArgs e)
        {
            if (bRecording == true)
            {
                var point = this.PointToClient(Cursor.Position);
                float xPos = 0;
                if (point.X < 0)
                {
                    xPos = 0;
                }
                else if (point.X > this.ClientRectangle.Width)
                {
                    xPos = this.ClientRectangle.Width;
                }
                else
                {
                    xPos = point.X;
                }

                float yPos = 0;
                if (point.Y < 0)
                {
                    yPos = 0;
                }
                else if (point.Y > this.ClientRectangle.Height)
                {
                    yPos = this.ClientRectangle.Height;
                }
                else
                {
                    yPos = point.Y;
                }

                AddMousePosToList(xPos, yPos);
            }
        }

        private void timerPlayBack_Tick(object sender, EventArgs e)
        {
            if (positionen.Count < 1)
            {
                Invalidate();
                return; // Liste ist leer
            }

            if (nIdx < positionen.Count - 1)
            {
                // Normalfall
            }
            else
            {
                // Wir sind am Ende der Liste, also den Index wieder
                // auf 0 stellen
                nIdx = 0;

                var bmpPic = svgDoc.Draw();
                var pic = bmpPic;

                // http://svg-net.github.io/SVG/doc/Q&A.html#how-to-render-an-svg-document-to-a-bitmap-in-another-size

                var b = SvgDocument.SystemIsGdiPlusCapable();

                // Groesse des SVG
                svgDoc.Height = pic.Height + idxDeltas; // e.ClipRectangle.Height / 4;

                svgDoc.Width = pic.Width + idxDeltas;

                if (svgDoc.Width <= 50)
                {
                    svgDoc.Width = 389;
                }

                if (svgDoc.Height <= 50)
                {
                    svgDoc.Height = 233;
                }
            }

            Invalidate();
            nIdx++; // Positionen-Index erhoehen
        }

        private void FlugbahnDlg_MouseEnter(object sender, EventArgs e)
        {
            Refresh();
        }

        private void FlugbahnDlg_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void FlugbahnDlg_ResizeEnd(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}