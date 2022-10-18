using Application.CQRS.Contacts.Commands;
using Application.CQRS.Contacts.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MyContacts.Controllers
{
    [Route("v1/contact/")]
    public class ContactController : BaseAPIController
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMediator _mediator;

        public ContactController(IMediator mediator, IWebHostEnvironment env)
        {
            _env = env;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetContactAsync([FromQuery] GetContactsQuery request)
        {
            try
            {
                var contactList = await _mediator.Send(request);
                return Ok(new { status = "success", message = "Get Contact Success", data = contactList.Data, count = contactList.Count });
            }
            catch (Exception e)
            {
                if (_env.IsProduction()) return StatusCode(500, new { status = "failed", message = "Get Contact Error" });
                return StatusCode(500, new { status = "failed", message = "Get Contact Error", error = e.ToString() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertContactAsync([FromBody] InsertContactCommand request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }

                // check if mobile is in e164 format
                var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
                var isMobileValid = phoneNumberUtil.IsPossibleNumber(request.mobile, null);
                if (!isMobileValid) return BadRequest(new { status = "failed", message = "Invalid mobile number. Enter in e164 format." });

                // check if mobile already exist
                request.mobile = request.mobile.Replace(" ", string.Empty);
                var contactList = await _mediator.Send(new GetContactsQuery { mobile = request.mobile });
                if (contactList.Data.Count > 0) return Conflict(new { status = "failed", message = "Mobile number already exist." });

                //  insert new contact 
                var insertContact = await _mediator.Send(request);
                return Ok(new { status = "success", message = "Insert Contact Success", data = insertContact });
            }
            catch (Exception e)
            {
                if (_env.IsProduction()) return StatusCode(500, new { status = "failed", message = "Insert Contact Error" });
                return StatusCode(500, new { status = "failed", message = "Insert Contact Error", error = e.ToString() });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContactAsync([FromBody] UpdateContactCommand request, [FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }

                // check if mobile is in e164 format
                if (request.mobile != null)
                {
                    var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
                    var isMobileValid = phoneNumberUtil.IsPossibleNumber(request.mobile, null);
                    if (!isMobileValid) return BadRequest(new { status = "failed", message = "Invalid mobile number. Enter in e164 format." });

                    // check if mobile already exist
                    request.mobile = request.mobile.Replace(" ", string.Empty);
                    var contactList = await _mediator.Send(new GetContactsQuery { mobile = request.mobile });
                    if (contactList.Data.Count > 0 && id != contactList.Data.First().ID) return Conflict(new { status = "failed", message = "Mobile number already exist." });
                }

                // check if contact exist
                var contact = await _mediator.Send(new GetContactsQuery { ID = id });
                if (contact.Data.Count == 0) return NotFound(new { status = "failed", message = "Contact Not Found" });

                //  update contact
                request.ID = id;
                var updateContactData = await _mediator.Send(request);

                return Ok(new { status = "success", message = "Update Contact Success", data = updateContactData });
            }
            catch (Exception e)
            {
                if (_env.IsProduction()) return StatusCode(500, new { status = "failed", message = "Update Contact Error" });
                return StatusCode(500, new { status = "failed", message = "Update Contact Error", error = e.ToString() });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactAsync([FromRoute] int id)
        {
            try
            {
                // check if contact exist
                var contact = await _mediator.Send(new GetContactsQuery { ID = id });
                if (contact.Data.Count == 0) return NotFound(new { status = "failed", message = "Contact Not Found" });

                //  delete contact
                var deleteContact = await _mediator.Send(new DeleteContactCommand { ID = id });

                return Ok(new { status = "success", message = "Delete Contact Success" });
            }
            catch (Exception e)
            {
                if (_env.IsProduction()) return StatusCode(500, new { status = "failed", message = "Delete Contact Error" });
                return StatusCode(500, new { status = "failed", message = "Delete Contact Error", error = e.ToString() });
            }
        }
    }
}
