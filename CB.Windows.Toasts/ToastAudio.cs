using Windows.Data.Xml.Dom;


namespace CB.Windows.Toasts
{
    public class ToastAudio: ToastElement
    {
        #region Fields
        private const string TOAST_AUDIO_NAMESPACE = "ms-winsoundevent:Notification";
        #endregion


        #region  Properties & Indexers
        public ToastAudioType AudioType { get; set; }

        public bool Loop { get; set; }

        public bool Silent { get; set; }
        #endregion


        #region Override
        public override void AddToToastContent(XmlDocument toastContent)
        {
            if (AudioType == ToastAudioType.None) return;

            var audioElement = GetParentElement(toastContent, "audio");
            audioElement.SetAttribute("src", ToString());
            if (Loop)
            {
                audioElement.SetAttribute("loop", "true");
                toastContent.DocumentElement.SetAttribute("duration", "long");
            }
            if (Silent)
            {
                audioElement.SetAttribute("silent", "true");
            }
        }

        public override string ToString()
        {
            var audioName = AudioType.ToString();
            return AudioType == ToastAudioType.None
                       ? audioName
                       : (audioName.StartsWith("Call") || audioName.StartsWith("Alarm")
                              ? $"{TOAST_AUDIO_NAMESPACE}.Looping.{audioName}"
                              : $"{TOAST_AUDIO_NAMESPACE}.{audioName}");
        }
        #endregion
    }
}