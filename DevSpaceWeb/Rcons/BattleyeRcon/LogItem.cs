namespace DaRT
{
    struct LogItem
    {
        public LogType Type
        {
            get { return _type; }
        }
        public string Message
        {
            get { return _message.ToString(); }
        }
        public bool Important
        {
            get { return _important; }
        }

        private LogType _type;
        private object _message;
        private bool _important;

        public LogItem(LogType type, object message, bool important)
        {
            _type = type;
            _message = message;
            _important = important;
        }
    }
}
