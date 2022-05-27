using ChatWebServer.Data;
using ChatWebServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatWebServer.Services
{
    public class Service : Controller
    {
        private readonly ChatWebServerContext _context;

        public Service(ChatWebServerContext context)
        {
            _context = context;
        }

        /**
         * Get user by username.
         */
        public async Task<User> GetUser(string username)
        {
            return await _context.User.Where(u => u.Id == username).FirstOrDefaultAsync();
        }
        /**
         * User validation.
         */
        public async Task<User> UserValidation(User user)
        {
            return await _context.User.Where(u => u.Id == user.Id && u.Password == user.Password).FirstOrDefaultAsync();
        }

        /**
         * Get the last message in the conversation.
         */
        private async Task<Message> GetLastMessage(int conversationId)
        {
            return await _context.Message.Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.Created)
                .LastOrDefaultAsync();
        }

        /**
         * Get a list of all contacts ia ApiContacts.
         */
        public async Task<List<ApiContact>> GetAllApiContacts(string username)
        {
            List<ApiContact> apiContacts = new List<ApiContact>();
            List<Conversation> conversations = await _context.Conversation.Where(c => c.UserId == username).ToListAsync();
            foreach (Conversation conversation in conversations)
            {
                Contact contact = await _context.Contact.Where(c => c.Id == conversation.ContactId).FirstOrDefaultAsync();
                Message lastMsg = await GetLastMessage(conversation.Id);
                apiContacts.Add(
                new ApiContact
                {
                    id = contact.Username,
                    name = contact.Name,
                    server = contact.Server,
                    last = (lastMsg == null) ? null : lastMsg.Content,
                    lastdate = (lastMsg == null) ? null : lastMsg.Created.ToString()
                });
            }
            apiContacts.Sort((a, b) => DateTime.Compare((b.lastdate == null) ? DateTime.MinValue : DateTime.Parse(b.lastdate),
                (a.lastdate == null) ? DateTime.MinValue : DateTime.Parse(a.lastdate)));
            return apiContacts;
        }

        /**
         * Check if there is already conversation with this contact.
         */
        public bool isConversationExists(string userId, string contactId)
        {
            var query = from conversation in _context.Conversation
                        join contact in _context.Contact
                        on conversation.ContactId equals contact.Id
                        where conversation.UserId == userId && contact.Username == contactId
                        select conversation;
            return query.Any();
        }

        /**
         * Create new conversation with the given contact.
         */
        public async Task<Boolean> CreateNewConversation(ApiContact apiContact, string username)
        {
            if (isConversationExists(username, apiContact.id) || apiContact == null)
                return false;
            User user = _context.User.Find(username);
            Conversation c = new Conversation
            {
                Contact = new Contact { Name = apiContact.name, Server = apiContact.server, Username = apiContact.id },
                Messages = new List<Message>(),
                User = user,
                UserId = user.Id
            };
            _context.Conversation.Add(c);
            await _context.SaveChangesAsync();
            return true;
        }

        /**
         * Edit contact.
         */
        public async Task<bool> EditContact(ApiContact apiContact, string username)
        {
            if (!isConversationExists(username, apiContact.id))
                return false;
            var query = (from conversation in _context.Conversation
                         join contact in _context.Contact
                         on conversation.ContactId equals contact.Id
                         where contact.Username == apiContact.id
                         select contact).First();
            if (query == null)
                return false;
            query.Server = apiContact.server;
            query.Name = apiContact.name;
            _context.Contact.Update(query);
            _context.SaveChanges();
            return true;
        }

        /**
         * Delete contact.
         */
        public async Task<bool> DeleteContact(string username, string contactId)
        {
            if (!isConversationExists(username, contactId))
                return false;
            var query = (from conversation in _context.Conversation
                         join contact in _context.Contact
                         on conversation.ContactId equals contact.Id
                         where contact.Username == contactId
                         select new { conversation, contact }).FirstOrDefault();
            _context.Conversation.Remove(query.conversation);
            _context.Contact.Remove(query.contact);
            _context.SaveChanges();
            return true;
        }

        /**
         * Get a list of all the messages with contactId.
         */
        public List<ApiMessage> GetAllMessages(string userId, string contactId)
        {
            List<ApiMessage> messages = new List<ApiMessage>();
            IQueryable m;
            var query = (from conversation in _context.Conversation
                         join contact in _context.Contact
                         on conversation.ContactId equals contact.Id
                         where conversation.UserId == userId && contact.Username == contactId
                         select conversation).FirstOrDefault();
            if (query != null)
            {
                m = from message in _context.Message
                    where message.ConversationId == query.Id
                    select new ApiMessage { id = message.Id, content = message.Content, created = message.Created.ToString(), sent = message.Sent };
                foreach (ApiMessage message in m)
                    messages.Add(message);
                return messages;
            }
            return null;
        }


        /**
         * Create new message with contactId. 
         */
        public async Task<Boolean> CreateNewMessage(ApiMessage apiMessage, string userId, string contactId)
        {
            Conversation conv = await (from conversation in _context.Conversation
                                       join contact in _context.Contact
                                       on conversation.ContactId equals contact.Id
                                       where conversation.UserId == userId && contact.Username == contactId
                                       select conversation).FirstOrDefaultAsync();
            if (conv == null)
                return false;
            await _context.Message.AddAsync(new Message { Conversation = conv, Content = apiMessage.content, Sent = true });
            await _context.SaveChangesAsync();
            return true;
        }

        /**
         * Create new message with contactId. 
        */
        public async Task<Boolean> CreateTransferMessage(ApiMessage apiMessage, string userId, string contactId)
        {
            Conversation conv = await (from conversation in _context.Conversation
                                       join contact in _context.Contact
                                       on conversation.ContactId equals contact.Id
                                       where conversation.UserId == userId && contact.Username == contactId
                                       select conversation).FirstOrDefaultAsync();
            if (conv == null)
                return false;
            await _context.Message.AddAsync(new Message { Conversation = conv, Content = apiMessage.content, Sent = false });
            await _context.SaveChangesAsync();
            return true;
        }

        /**
         * Edit content of messageId. 
         */
        public async Task<bool> EditMessage(ApiMessage apiMessage)
        {
            if (apiMessage == null)
                return false;
            Message message = _context.Message.Where(m => m.Id == apiMessage.id).FirstOrDefault();
            message.Content = apiMessage.content;
            _context.Message.Update(message);
            _context.SaveChanges();
            return true;
        }

        /**
         * Delete message.
         */
        public async Task<bool> DeleteMessage(int messageId)
        {
            var message = _context.Message.Where(m => m.Id == messageId).FirstOrDefault();
            if (message == null)
                return false;
            _context.Message.Remove(message);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> RegisterNewUser(User user)
        {
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
