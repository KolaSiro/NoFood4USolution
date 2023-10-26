using FlugbahnLib;
using Game;
using Svg;
using System.Diagnostics;

namespace Game
{
    public class Insekt
    {
        public int Id { get; set; }

        public SvgDocument Pic { get; set; }

        // x/y Positionen des Insekts
        public List<Pos> positionen = new();

        // Groessenaenderungen des Insekts
        public List<int> deltas = new();

        public int IndexOfPosition { get; set; }

        // Zeigt an ob das Insekt Tod ist
        public bool Tod { get; set; }

        // Zeigt die Todesposition an   
        public Pos TodesPosition { get; set; }

        public bool IsStarted { get; set; } = false;

        public Rectangle HitBox { get; set; } = new Rectangle();

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

        public Insekt()
        {
            CreateDeltaValues();
            Tod = false;
            TodesPosition = new Pos(-1, -1);
            TodesPosition.X = -1;
            TodesPosition.Y = -1;
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