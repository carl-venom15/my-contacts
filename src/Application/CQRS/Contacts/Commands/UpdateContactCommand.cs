using Application.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;

namespace Application.CQRS.Contacts.Commands
{
    public class UpdateContactCommand : IRequest<Contact>
    {
        [JsonIgnore]
        public int ID { get; set; }
        public string? mobile { get; set; }
        public string? fname { get; set; }
        public string? lname { get; set; }
        public string? email { get; set; }
        public string? group { get; set; }
        public byte? is_starred { get; set; }
    }
    public class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
    {
        public UpdateContactCommandValidator()
        {
            RuleFor(t => t.mobile).NotEmpty().WithMessage("Mobile must not be empty").When(t => t.mobile != null);
            RuleFor(t => t.fname).Length(2, 50).WithMessage("First name must be between 2 and 50 characters").NotEmpty().WithMessage("First name must not be empty").When(t => t.fname != null);
            RuleFor(t => t.lname).Length(2, 50).WithMessage("Last name must be between 2 and 50 characters").NotEmpty().WithMessage("Last name must not be empty").When(t => t.lname != null);
            RuleFor(t => t.email).EmailAddress().Length(2, 50).WithMessage("Email must be between 2 and 50 characters").When(t => t.email != string.Empty);
            RuleFor(t => t.group).Length(2, 50).WithMessage("Group must be between 2 and 50 characters").When(t => t.group != string.Empty);
        }
    }
    public class UpdateContactHandler : IRequestHandler<UpdateContactCommand, Contact>
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public UpdateContactHandler(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Contact> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            // Get contact
            var contactResults = _applicationDbContext.Contact.Where(x => x.ID == request.ID).First();

            // Update contact
            if (request.fname != null) contactResults.fname = request.fname;
            if (request.lname != null) contactResults.lname = request.lname;
            if (request.mobile != null) contactResults.mobile = request.mobile;
            if (request.is_starred != null) contactResults.is_starred = (byte)request.is_starred;

            //Optional Parameters
            if (request.email != null) contactResults.email = request.email == string.Empty ? null : request.email;
            if (request.group != null) contactResults.group = request.group == string.Empty ? null : request.group;

            contactResults.updated_at = DateTime.UtcNow;

            // save contact
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return contactResults;
        }
    }
}
