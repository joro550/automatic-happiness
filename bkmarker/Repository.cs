using System.Data;
using Microsoft.Data.Sqlite;

namespace bkmarker;

public interface IRepository { }


internal sealed class Repository : IRepository
{
  private readonly ConnectionString _connectionString;

  public Repository(ConnectionString connectionString)
  {
    _connectionString = connectionString;
  }

  public async Task<T> WithConnection<T>(Func<IDbConnection, Task<T>> action)
  {
    await using var connnection = new SqliteConnection(_connectionString.Value);
    await connnection.OpenAsync();
    return await action(connnection);
  }

  public async Task WithConnection(Func<IDbConnection, Task> action)
  {
    await using var connnection = new SqliteConnection(_connectionString.Value);
    await connnection.OpenAsync();
    await action(connnection);
  }
}
