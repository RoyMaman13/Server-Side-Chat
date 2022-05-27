namespace ChatWebServer.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;   

        public bool Sent { get; set; }

        //Relations
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
    }
}
