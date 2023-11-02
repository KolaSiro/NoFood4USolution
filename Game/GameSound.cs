namespace Game
{
    /// <summary>
    /// Die Klasse GameSound ist ein Wrapper um den Windows Media Player
    /// </summary>
    public  class GameSound
    {
        private WMPLib.WindowsMediaPlayer mediaPlayer = new WMPLib.WindowsMediaPlayer();


        /// <summary>
        /// Konstruktor
        /// </summary>
        public GameSound()
        {
            mediaPlayer.uiMode = "invisible";

            mediaPlayer.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
            mediaPlayer.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
        }

        /// <summary>
        /// Start des Sounds
        /// </summary>
        /// <param name="sPathToAudio">Pfad zum Sound: im Verzeichnis 'sounds\'</param>
        public void Play(String sPathToAudio)
        {
            try
            {
                string s = Directory.GetCurrentDirectory();
                s += @"\sounds\" + sPathToAudio;

                mediaPlayer.URL = s;
                mediaPlayer.controls.play();
            }
            catch
            {
                // nix tun
            }
        }

        /// <summary>
        /// Wiederholung
        /// </summary>
        /// <param name="bLoop">false= ohne Wiederholung, true= mit Wdh. </param>
        public void Loop(bool bLoop)
        {
            mediaPlayer.settings.setMode("loop", bLoop);
        }

        /// <summary>
        /// Stoppt die Soundwiedergabe
        /// </summary>
        public void Stop()
        {
            mediaPlayer.controls.stop();
        }

        private void Player_PlayStateChange(int NewState)
        {
            switch ((WMPLib.WMPPlayState)NewState)
            {
                case WMPLib.WMPPlayState.wmppsStopped:
                case WMPLib.WMPPlayState.wmppsUndefined:
                case WMPLib.WMPPlayState.wmppsPaused:
                    mediaPlayer.URL = null;
                    mediaPlayer.close();
                    break;
            }
        }

        private void Player_MediaError(object pMediaObject)
        {
            //MessageBox.Show("Cannot play Audio file.");
            mediaPlayer.close();
        }

    }
}
