using DashBoard.Core.Entities;

namespace DashBoard.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Usuario> Usuarios { get; }
    IRepository<Analisis> Analisis { get; }
    Task<int> CompleteAsync();
}
