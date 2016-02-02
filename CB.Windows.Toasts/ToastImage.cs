using System;
using Windows.Data.Xml.Dom;
using CB.Windows.UI;


namespace CB.Windows.Toasts
{
    public class ToastImage
    {
        #region  Properties & Indexers
        public string Alt { get; set; }
        public string Path { get; set; }
        public SourceType SourceType { get; set; }
        #endregion


        #region Methods
        public string GetImageSource()
        {
            return string.IsNullOrWhiteSpace(Path) ? "" : CreateImageSource();
        }

        public void SetImageAttribute(XmlElement imageElement)
        {
            if (imageElement == null) return;

            imageElement.SetAttribute("src", GetImageSource());
            if (!string.IsNullOrWhiteSpace(Alt))
            {
                imageElement.SetAttribute("alt", Alt);
            }
        }
        #endregion


        #region Implementation
        private string AddPathPrefix(string prefix, string path)
            => Path.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase) ? path : prefix + path;

        private string CreateImageSource()
        {
            var path = Path;

            switch (SourceType)
            {
                case SourceType.WebPage:
                    return AddPathPrefix("http://", path);
                case SourceType.SecuredWebPage:
                    return AddPathPrefix("https://", path);
                case SourceType.AppPackage:
                    return AddPathPrefix("ms-appx:///", path);
                case SourceType.LocalStorage:
                    return AddPathPrefix("ms-appdata:///local/", path);
                case SourceType.LocalImage:
                    path = System.IO.Path.GetFullPath(Path);
                    return AddPathPrefix("file:///", path);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion
    }
}