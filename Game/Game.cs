using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
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

        /// <summary>
        /// 
        /// </summary>
        public const int FLIEGE_GESCHWINDIGKEIT_AENDERUNG = 20;

        /// <summary>
        /// 
        /// </summary>
        public const int FLIEGE_MIN_GESCHWINDIGKEIT = 20;

        /// <summary>
        /// 
        /// </summary>
        public const int FLIEGE_MAX_GESCHWINDIGKEIT = 240;

        public const int MOUSE_POS_X_INIT = 9999; 
        public const int MOUSE_POS_Y_INIT = 9999;

        /// <summary>
        /// So viel mal darf das Insekt durch die Landingzone fliegen, bis das Spiel fertig ist
        /// </summary>
        public const int MAX_FLIEGE_LANDINGZONE = 10;

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
