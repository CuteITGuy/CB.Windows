using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;
using CB.Windows.UI;


namespace CB.Windows.Toasts
{
    public class Toast
    {
        #region Fields
        private readonly string _applicationId;
        private ToastNotification _toast;
        #endregion


        #region  Constructors & Destructor
        public Toast(string applicationId)
        {
            _applicationId = applicationId;
        }

        public Toast(): this(CreateUniqueId()) { }
        #endregion


        #region  Properties & Indexers
        public ToastAudio Audio { get; set; }
        public IEnumerable<ToastCommand> Commands { get; set; }
        public SynchronizationContext Context { get; set; }
        public DateTimeOffset? ExpirationTime { get; set; }
        public ToastImage Image { get; set; }
        public string Launch { get; set; }
        public string[] Lines { get; set; }
        #endregion


        #region Events
        public event TypedEventHandler<ToastNotification, object> Activated;
        public event TypedEventHandler<ToastNotification, ToastDismissedEventArgs> Dismissed;
        public event TypedEventHandler<ToastNotification, ToastFailedEventArgs> Failed;
        #endregion


        #region Methods
        public void Hide()
        {
            if (_toast == null) return;

            ToastNotificationManager.CreateToastNotifier(_applicationId).Hide(_toast);
            _toast = null;
        }

        public void Show()
        {
            Show(CreateToastContent());
        }

        public void Show(string xmlContent)
        {
            Show(CreateToastContent(xmlContent));
        }

        public void Show(XmlDocument content)
        {
            _toast = CreateToast(content);
            ToastNotificationManager.CreateToastNotifier(_applicationId).Show(_toast);
        }
        #endregion


        #region Event Handlers
        protected virtual void OnActivated(ToastNotification sender, object args)
        {
            InvokeEvent(Activated, sender, args);
        }

        protected virtual void OnDismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
            InvokeEvent(Dismissed, sender, args);
        }

        protected virtual void OnFailed(ToastNotification sender, ToastFailedEventArgs args)
        {
            InvokeEvent(Failed, sender, args);
        }
        #endregion


        #region Implementation
        private ToastNotification CreateToast(XmlDocument content)
        {
            Debug.WriteLine(content.GetXml());
            var toast = new ToastNotification(content);
            if (ExpirationTime.HasValue)
            {
                toast.ExpirationTime = ExpirationTime;
            }
            toast.Activated += OnActivated;
            toast.Dismissed += OnDismissed;
            toast.Failed += OnFailed;
            return toast;
        }

        private static XmlDocument CreateToastContent(string xmlContent)
        {
            var content = new XmlDocument();
            content.LoadXml(xmlContent);
            return content;
        }

        private XmlDocument CreateToastContent()
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(GetToastTemplateType());

            var txtElements = toastXml.GetElementsByTagName("text");
            for (var i = 0; i < txtElements.Length; i++)
            {
                txtElements[i].AppendChild(toastXml.CreateTextNode(Lines[i]));
            }

            if (Image != null)
            {
                var imgElement = toastXml.GetElementsByTagName("image")[0] as XmlElement;
                Image.SetImageAttribute(imgElement);
            }

            Audio?.AddToToastContent(toastXml);

            if (Commands != null)
            {
                foreach (var command in Commands)
                {
                    command?.AddToToastContent(toastXml);
                }
            }

            if (!string.IsNullOrWhiteSpace(Launch))
            {
                toastXml.DocumentElement.SetAttribute("launch", Launch);
            }
            return toastXml;
        }

        private static string CreateUniqueId()
        {
            return new Guid().ToString();
        }

        private ToastTemplateType GetToastTemplateType()
        {
            if (Image == null)
            {
                switch (Lines.Length)
                {
                    case 1:
                        return ToastTemplateType.ToastText01;
                    case 2:
                        return IsLine1Longer() ? ToastTemplateType.ToastText03 : ToastTemplateType.ToastText02;
                    case 3:
                        return ToastTemplateType.ToastText04;
                    default:
                        throw new Exception();
                }
            }
            switch (Lines.Length)
            {
                case 1:
                    return ToastTemplateType.ToastImageAndText01;
                case 2:
                    return IsLine1Longer()
                               ? ToastTemplateType.ToastImageAndText03 : ToastTemplateType.ToastImageAndText02;
                case 3:
                    return ToastTemplateType.ToastImageAndText04;
                default:
                    throw new Exception();
            }
        }

        private void InvokeEvent<TArgs>(TypedEventHandler<ToastNotification, TArgs> eventHandler,
            ToastNotification sender, TArgs args)
        {
            if (eventHandler == null) return;
            if (Context == null)
            {
                eventHandler.Invoke(sender, args);
            }
            else
            {
                Context.Send(state => eventHandler.Invoke(sender, args), null);
            }
        }

        private bool IsLine1Longer()
        {
            return !string.IsNullOrWhiteSpace(Lines[0]) &&
                   (string.IsNullOrWhiteSpace(Lines[1]) || Lines[0].Length > Lines[1].Length);
        }
        #endregion
    }
}


//TODO: Add ToastCommand
//TODO: Complete TestToast