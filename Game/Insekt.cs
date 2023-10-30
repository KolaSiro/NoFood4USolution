using FlugbahnLib;
using Game;
using Svg;
using System.Diagnostics;
using System.Media;

namespace Game
{
    public class Insekt
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// SVG-Bild des Insekts
        /// </summary>
        public SvgDocument Pic { get; set; }

        /// <summary>
        /// x/y Flugbahn-Positionen des Insekts
        /// </summary>
        public List<Pos> positionen = new();

        /// <summary>
        /// NOT USED YET: Groessenaenderungen des Insekts
        /// </summary>
        public List<int> deltas = new();

        /// <summary>
        /// Momentarer Index im Flugbahn Array
        /// </summary>
        public int IndexOfPosition { get; set; }

        /// <summary>
        /// Zeigt an ob das Insekt Tod ist
        /// </summary>
        public bool Tod { get; set; }

        /// <summary>
        /// Zeigt die Todesposition an. Wird fuer den Absturz genutzt.
        /// </summary>
        public Pos TodesPosition { get; set; }

        /// <summary>
        /// Zeigt an ob das Insekt gestartet ist.
        /// </summary>
        public bool IsStarted { get; set; } = false;

        /// <summary>
        /// Groesse der Hit-Zone des Insekts
        /// </summary>
        public Rectangle HitBox { get; set; } = new Rectangle();

        /// <summary>
        /// Status eines Insekts, wenn es ueber das Essen fliegt
        /// </summary>
        public enum InsektStatus
        {
            NONE,
            ENTER,
            LEAVE,
        }

        /// <summary>
        /// Fuer Intersect: Insekt -- Landingzone
        /// Wir erkennen dadurch, ob das Insekt sich gerade in der Landingzone aufhaelt.
        /// </summary>
        public InsektStatus StatusNow { get; set; } = InsektStatus.NONE;
        public InsektStatus StatusBefore { get; set; } = InsektStatus.NONE;

        public  SoundPlayer sound = new();

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Insekt()
        {
            CreateDeltaValues();
            Tod = false;
            TodesPosition = new Pos(-1, -1);
            TodesPosition.X = -1;
            TodesPosition.Y = -1;
        }

        /// <summary>
        /// Not used yet
        /// </summary>
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

                    Debug.WriteLine("Sinus Winkel : " + sinAngle + " von " + grad + " Grad");
                    deltas.Add((int)sinAngle); // explicit cast
                    //idxDeltas++;
                }
                grad += 10;
            }
            //idxDeltas = 0;
        }
    }
}