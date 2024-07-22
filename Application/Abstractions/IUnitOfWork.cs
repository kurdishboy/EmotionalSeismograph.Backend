namespace Application.Abstractions;

public interface IUnitOfWork
{
    int SaveChanges();
}