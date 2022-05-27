namespace ChatWebServer.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }


        //Relations
        public ICollection<Conversation> Conversations { get; set; }
    }
}
