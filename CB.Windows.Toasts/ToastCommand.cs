using Windows.Data.Xml.Dom;


namespace CB.Windows.Toasts
{
    public class ToastCommand: ToastElement
    {
        #region  Properties & Indexers
        public string Arguments { get; set; }
        public ToastCommandId CommandId { get; set; }
        #endregion


        #region Override
        public override void AddToToastContent(XmlDocument toastContent)
        {
            if (CommandId == ToastCommandId.None) return;

            var commandsElement = CreateCommandsElement(toastContent);
            var commandElement = CreateCommandElement(toastContent);
            commandsElement.AppendChild(commandElement);
        }
        #endregion


        #region Implementation
        private XmlElement CreateCommandElement(XmlDocument toastContent)
        {
            var commandElement = toastContent.CreateElement("command");
            if (CommandId != ToastCommandId.None)
            {
                commandElement.SetAttribute("id", CommandId.ToString().ToLower());
            }
            if (!string.IsNullOrWhiteSpace(Arguments))
            {
                commandElement.SetAttribute("arguments", Arguments);
            }
            return commandElement;
        }

        private XmlElement CreateCommandsElement(XmlDocument toastContent)
        {
            var commandsElement = GetParentElement(toastContent, "commands");
            string scenario = null;
            switch (CommandId)
            {
                case ToastCommandId.Snooze:
                case ToastCommandId.Dismiss:
                    scenario = "alarm";
                    break;

                case ToastCommandId.Video:
                case ToastCommandId.Voice:
                case ToastCommandId.Decline:
                    scenario = "incomingCall";
                    break;
            }
            if (!string.IsNullOrWhiteSpace(scenario))
            {
                commandsElement.SetAttribute("scenario", scenario);
            }
            return commandsElement;
        }
        #endregion
    }
}