using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Contact> Contact { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
