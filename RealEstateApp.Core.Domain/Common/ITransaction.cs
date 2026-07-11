namespace RealEstateApp.Core.Domain.Common;

public interface ITransaction : IDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
}
