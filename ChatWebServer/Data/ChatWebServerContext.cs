using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChatWebServer.Models;

namespace ChatWebServer.Data
{
    public class ChatWebServerContext : DbContext
    {
        public ChatWebServerContext (DbContextOptions<ChatWebServerContext> options)
            : base(options)
        {
        }

        public DbSet<ChatWebServer.Models.Contact>? Contact { get; set; }

        public DbSet<ChatWebServer.Models.Conversation>? Conversation { get; set; }

        public DbSet<ChatWebServer.Models.Message>? Message { get; set; }

        public DbSet<ChatWebServer.Models.User>? User { get; set; }

        public DbSet<ChatWebServer.Models.Rate> Rate { get; set; }
    }
}
