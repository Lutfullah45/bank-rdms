namespace BankRDMS.Repository.Abstract;

public interface IUnitOfWork
{
    Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default);
}
