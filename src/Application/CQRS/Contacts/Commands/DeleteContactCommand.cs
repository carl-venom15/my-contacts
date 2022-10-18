using Application.Interfaces;
using MediatR;

namespace Application.CQRS.Contacts.Commands
{
    public class DeleteContactCommand : IRequest<bool>
    {
        public int ID { get; set; }
    }

    public class DeleteContactHandler : IRequestHandler<DeleteContactCommand, bool>
    {
        private IApplicationDbContext _applicationDbContext;

        public DeleteContactHandler(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<bool> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            // Get contact
            var contactResults = _applicationDbContext.Contact.Where(x => x.ID == request.ID).First();

            // Update contact
            contactResults.is_deleted = 1;

            // save contact
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
