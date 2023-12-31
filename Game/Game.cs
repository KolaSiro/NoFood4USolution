﻿namespace Game
{
    public class Game
    {
        /// <summary>
        /// Maximale Spielzeit
        /// </summary>
        public const int MAX_SPIEL_ZEIT = 50;

        /// <summary>
        /// Maximale Anzahl Schlaege
        /// </summary>
        public const int MAX_SCHLAEGE = 10;

        /// <summary>
        /// HitBox-Breite vom Essen
        /// </summary>
        public const double ESSEN_HITBOX_WIDTH = 0.15;

        /// <summary>
        /// HitBox-Hoehe vom Essen
        /// </summary>
        public const double ESSEN_HITBOX_HEIGHT = 0.15;

        public const int MOUSE_POS_X_INIT = 9999;
        public const int MOUSE_POS_Y_INIT = 9999;

        /// <summary>
        /// So viel mal darf das Insekt durch die Landingzone
        /// fliegen, bis das Spiel fertig ist
        /// </summary>
        public const int MAX_FLIEGE_LANDINGZONE = 9;

        public const int MAX_INSEKTEN_COUNT = 5;

        /// <summary>
        /// HitBox-Hoehe vom Insekt
        /// </summary>
        public static int InsektHitBoxWidth { get; set; } = 70;

        /// <summary>
        /// HitBox-Breite vom Insekt
        /// </summary>
        public static int InsektHitBoxHeight { get; set; } = 70;
    }
}