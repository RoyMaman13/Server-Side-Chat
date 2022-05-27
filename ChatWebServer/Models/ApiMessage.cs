namespace ChatWebServer.Models
{
    public class ApiMessage
    {
      public int id { get; set; }
        public string content { get; set; }
        public string created { get; set; }

        public bool sent { get; set; }

    }
}
