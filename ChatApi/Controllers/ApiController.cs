using ChatWebServer.Models;
using ChatWebServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private readonly Service _service;
        /*private readonly string username = "admin";*/

        public ApiController(Service service)
        {
            _service = service;
        }

        /**
         * Get all contacts of the user. GET /api/contacts
         */
        [HttpGet("contacts")]
        public async Task<IActionResult> GetAllContacts(string username)
        {
            if (username == null || username == "")
                return NotFound();
            return Ok(await _service.GetAllApiContacts(username));
        }

        /**
         * Create new contact. POST /api/contacts
         */
        [HttpPost("contacts")]
        public async Task<IActionResult> CreateContact([Bind("id,name,server")] ApiContact apiContact, string username)
        {
            if (username == null || username == "")
                return NotFound();
            if (await _service.CreateNewConversation(apiContact, username))
                return StatusCode(201);
            return NotFound();
        }

        /**
         * Get contact by ID. GET /api/contacts/{id}
         */
        [HttpGet("contacts/{contactId}")]
        public async Task<IActionResult> GetContact(string contactId, string username)
        {
            if (username == null || username == "" || contactId == null || contactId == "")
                return NotFound();
            ApiContact? apiContact = (await _service.GetAllApiContacts(username)).Find(apiContact => apiContact.id == contactId);
            return (apiContact == null) ? NotFound() : Ok(apiContact);
        }

        /**
         * Edit contact by ID. PUT /api/contacts/{id}
         */
        [HttpPut("contacts/{contactId}")]
        public async Task<IActionResult> EditContact([Bind("name,server")] ApiContact apiContact, string contactId, string username)
        {
            apiContact.id = contactId;
            if (username == null || username == "" || contactId == null || contactId == "")
                return NotFound();
            return (await _service.EditContact(apiContact, username)) ? StatusCode(204) : NotFound();
        }

        /**
         * Delete exist contact. DELETE /api/contacts/{id}
         */
        [HttpDelete("contacts/{contactId}")]
        public async Task<IActionResult> DeleteContact(string contactId, string username)
        {
            if (username == null || username == "" || contactId == null || contactId == "")
                return NotFound();
            return (await _service.DeleteContact(username, contactId)) ? StatusCode(204) : NotFound();
        }

        /**
         * Get all message from contactId. GET /api/contacts/{id}/messages
         */
        [HttpGet("contacts/{contactId}/messages")]
        public async Task<IActionResult> GetAllContactMessage(string contactId, string username)
        {
            if (username == null || username == "" || contactId == null || contactId == "")
                return NotFound();
            var answer = _service.GetAllMessages(username, contactId);

            return (answer == null) ? NotFound() : Ok(answer);

        }

        /**
         * Create new message. POST /api/contacts/{id}/messages
         */
        [HttpPost("contacts/{contactId}/messages")]
        public async Task<IActionResult> CreateMessage([Bind("content")] ApiMessage apiMessage, string contactId, string username)
        {
            if (username == null || username == ""
                || contactId == null || contactId == ""
                || apiMessage == null || apiMessage.content == null)
                return NotFound();

            return (await _service.CreateNewMessage(apiMessage, username, contactId)) ? StatusCode(201) : NotFound();
        }


        /**
         * Get specific message by Id from contactId conversation. GET /api/contacts/{ContactId}/messages/{MessageId}
         */
        [HttpGet("contacts/{contactId}/messages/{messageId}")]
        public async Task<IActionResult> GetMessageById(string contactId, int messageId, string username)
        {
            if (username == null || username == "" || contactId == null || contactId == "" || messageId == null)
                return NotFound();
            List<ApiMessage> apiMessages = (_service.GetAllMessages(username, contactId));
            if (apiMessages == null)
                return NotFound();
            ApiMessage? apiMessage = apiMessages.Find(apiMessage => apiMessage.id == messageId);
            return (apiMessage == null) ? NotFound() : Ok(apiMessage);
        }

        /**
        * Edit specific message by Id from contactId conversation. PUT /api/contacts/{ContactId}/messages/{MessageId}
        */
        [HttpPut("contacts/{contactId}/messages/{messageId}")]
        public async Task<IActionResult> EditMessageById([Bind("content")] ApiMessage apiMessage, string contactId, int messageId, string username)
        {
            if (username == null || username == "" || contactId == null || contactId == "" || messageId == null)
                return NotFound();
            List<ApiMessage> apiMessages = (_service.GetAllMessages(username, contactId));
            if (apiMessages == null || apiMessage.content == null)
                return NotFound();
            ApiMessage? message = apiMessages.Find(m => m.id == messageId);
            if (message != null)
            {
                message.content = apiMessage.content;
                return (_service.EditMessage(message) == null) ? NotFound() : StatusCode(204);
            }
            return NotFound();
        }

        /**
        * Delete specific message by Id from contactId conversation. DELETE /api/contacts/{ContactId}/messages/{MessageId}
        */
        [HttpDelete("contacts/{contactId}/messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(string contactId, int messageId, string username)
        {
            if (username == null || username == "" || contactId == null || contactId == "" || messageId == null)
                return NotFound();
            return (await _service.DeleteMessage(messageId)) ? StatusCode(204) : NotFound();
        }

        /**
         * Invitation for a new conversation. POST /api/invitations
         */
        [HttpPost("invitations")]
        
        public async Task<IActionResult> PostInvitation([Bind("from,to,server")] ApiInvitation apiInvitation)
        {
/*            if (apiInvitation == null || apiInvitation.from == null
                || apiInvitation.to == null || apiInvitation.server == null)
                return NotFound();*/
            User to = await _service.GetUser(apiInvitation.to);
            if (to == null)
                return NotFound();
            ApiContact apiContact = new ApiContact { id = apiInvitation.from, name = apiInvitation.from, server = apiInvitation.server };
            if (await _service.CreateNewConversation(apiContact, to.Id))
                return StatusCode(201);
            return NotFound();
        }

        /**
         * Create new message from 'from' to 'to' with the given content.
         * POST, /api/transfer
         */
        [HttpPost("transfer")]
        public async Task<IActionResult> PostTransfer([Bind("from,to,content")] ApiTransfer apiTransfer)
        {
            if (apiTransfer == null || apiTransfer.from == null
                || apiTransfer.to == null || apiTransfer.content == null)
                return NotFound();
            User to = await _service.GetUser(apiTransfer.to);
            if (to == null)
                return NotFound();
            ApiMessage apiMessage = new ApiMessage { content = apiTransfer.content };
            return (await _service.CreateTransferMessage(apiMessage, apiTransfer.to, apiTransfer.from)) ? StatusCode(201) : NotFound();
        }
    }
}
