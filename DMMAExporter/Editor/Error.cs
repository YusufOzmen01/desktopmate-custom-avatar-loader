namespace DMMAExporter
{
    public enum ErrorType {
        Error,
        Warning,
        Info,
        Success,
    }
    public struct Error {
        public string msg;
        public ErrorType type;
        public Error(string msg, ErrorType type)
        {
            this.msg = msg;
            this.type = type;
        }
    }
}