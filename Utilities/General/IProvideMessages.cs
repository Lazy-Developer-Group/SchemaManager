using System;

namespace Utilities.General
{
    public interface IProvideMessages
    {
        event EventHandler<MessageEventArgs> SendMessage;
    }

    public class MessageEventArgs : EventArgs
    {
        public string       Message;
        public MessageLevel Level; 
    }

    public enum MessageLevel
    {
        None = 0,
        Debug = 1,
        Info = 2,
        Warn = 3,
        Error = 4
    }
}