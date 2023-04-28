namespace Common
{
    public class Response
    {
        string message;
        bool accepted;
        public Response(string message, bool accepted)
        {
            this.message = message;
            this.accepted = accepted;
        }
        public string Message
        {
            get { return message; }
        }
        public bool Accepted
        {
            get { return accepted; }
        }
    }
}