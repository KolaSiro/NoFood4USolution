using System.Drawing;

namespace FlugbahnLib
{
    [Serializable]
    public class Pos
    {
        /// <summary>
        /// X in Pixel
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y in Pixel
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// X in Prozent
        /// </summary>
        public float XPos { get; set; }

        /// <summary>
        /// Y in Prozent
        /// </summary>
        public float YPos { get; set; }

        /// <summary>
        /// Breite in Pixel
        /// </summary>
        public float Breite { get; set; }

        /// <summary>
        /// Hoehe in Pixel
        /// </summary>
        public float Hoehe { get; set; }

        public Pos()
        {
            XPos = 0;
            YPos = 0;

            X = 0;
            Y = 0;

            Breite = 0;
            Hoehe = 0;
        }

        /// <summary>
        /// Konstruktor in Positions-Prozenten
        /// </summary>
        /// <param name="x">X in Prozent</param>
        /// <param name="y">Y in Prozent</param>
        public Pos(float x, float y)
        {
            XPos = x;
            YPos = y;
        }

        /// <summary>
        /// Mitte in Pixel
        /// </summary>
        /// <returns></returns>
        public PointF Mitte()
        {
            return new PointF(X - Breite / 2.0f, Y - Hoehe / 2.0f);
        }

        public static implicit operator Point(Pos v)
        {
            throw new NotImplementedException();
        }
    }
}