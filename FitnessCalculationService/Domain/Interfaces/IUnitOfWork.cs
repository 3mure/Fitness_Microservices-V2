using Microsoft.EntityFrameworkCore.Storage;

namespace FitnessCalculationService.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Transaction Methods
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}

