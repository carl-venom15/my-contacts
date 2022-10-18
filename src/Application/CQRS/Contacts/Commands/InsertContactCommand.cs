using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.CQRS.Contacts.Commands
{
    public class InsertContactCommand : IRequest<List<Contact>>
    {
        public string? mobile { get; set; }
        public string? fname { get; set; }
        public string? lname { get; set; }
        public string? email { get; set; }
        public string? group { get; set; }
    }
    public class InsertContactCommandValidator : AbstractValidator<InsertContactCommand>
    {
        public InsertContactCommandValidator()
        {
            RuleFor(t => t.mobile).NotEmpty().WithMessage("Mobile must not be empty").Length(2, 50).WithMessage("Mobile number is a required field").NotNull().WithMessage("Mobile number is a required field");
            RuleFor(t => t.fname).NotEmpty().WithMessage("First name must not be empty").Length(2, 50).WithMessage("First name must be between 2 and 50 characters").NotNull().WithMessage("First name is a required field");
            RuleFor(t => t.lname).NotEmpty().WithMessage("Last name must not be empty").Length(2, 50).WithMessage("Last name must be between 2 and 50 characters").NotNull().WithMessage("Last name is a required field");
            RuleFor(t => t.email).EmailAddress().WithMessage("Enter a valid email address").Length(2, 50).WithMessage("Email must be between 2 and 50 characters").NotEmpty().WithMessage("Email must not be empty").When(t => t.email != null);
            RuleFor(t => t.group).Length(2, 50).WithMessage("Group must be between 2 and 50 characters").NotEmpty().WithMessage("Group must not be empty").When(t => t.group != null);
        }
    }
    public class InsertContactHandler : IRequestHandler<InsertContactCommand, List<Contact>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;

        public InsertContactHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
        }

        public async Task<List<Contact>> Handle(InsertContactCommand request, CancellationToken cancellationToken)
        {
            request.email = string.IsNullOrEmpty(request.email) ? null : request.email;
            request.group = string.IsNullOrEmpty(request.group) ? null : request.group;
            var convertContactModel = _mapper.Map<Contact>(request);
            convertContactModel.created_at = DateTime.UtcNow;
            await _applicationDbContext.Contact.AddAsync(convertContactModel);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            var contactData = _applicationDbContext.Contact.Where(i => i.ID == convertContactModel.ID).ToList();
            return contactData;
        }
    }
}
