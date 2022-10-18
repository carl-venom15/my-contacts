using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.CQRS.Contacts.Queries
{
    public class GetContactsQuery : IRequest<DataAndCount<Contact>>
    {
        public int ID { get; set; }
        public string? mobile { get; set; }
        public string? search { get; set; }
        public int? is_starred { get; set; }
        public int page { get; set; } = 1;
        public int per_page { get; set; } = int.MaxValue;
    }
    public class GetContactsHandler : IRequestHandler<GetContactsQuery, DataAndCount<Contact>>
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public GetContactsHandler(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<DataAndCount<Contact>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
        {
            var contactResults = await _applicationDbContext.Contact.Where(x => x.is_deleted == 0).ToListAsync();
            contactResults = contactResults.OrderByDescending(x => x.is_starred).ThenByDescending(x => x.ID).ToList();
            if (request.ID > 0) contactResults = contactResults.Where(x => x.ID == request.ID).ToList();
            if (request.is_starred != null) contactResults = contactResults.Where(x => x.is_starred == request.is_starred).ToList();
            if (!string.IsNullOrEmpty(request.mobile)) contactResults = contactResults.Where(x => x.mobile == request.mobile).ToList();
            if (!string.IsNullOrEmpty(request.search)) contactResults = contactResults.Where(x => x.fname.Contains(request.search) || x.lname.Contains(request.search) || x.mobile.Contains(request.search) ||  (x.email ?? "").Contains(request.search) || (x.group ?? "").Contains(request.search) ).ToList();
            var contactResultsCount = contactResults.Count();
            contactResults = contactResults.Skip((request.page - 1) * request.per_page).Take(request.per_page).ToList();     
            var contactDataAndCount = new DataAndCount<Contact>() { Count = contactResultsCount, Data = contactResults };
            return contactDataAndCount;
        }
    }
}
