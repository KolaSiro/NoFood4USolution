namespace FlugbahnLib
{
    public class GlobalValues
    {
        /// <summary>
        /// Position der Pizza oder Torte
        /// </summary>
        public const float LANDING_ZONE_POS_X_PERCENT = 0.5f;
        public const float LANDING_ZONE_POS_Y_PERCENT = 0.5f;

        public const float LANDING_ZONE_WIDTH_AND_HEIGHT_VALUE = 10.0f;

        /// <summary>
        /// Wandelt Radiant in Grad um
        /// </summary>
        /// <param name="angle">Wert in Radiant</param>
        /// <returns>Ergebnis in Grad</returns>
        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }
}
