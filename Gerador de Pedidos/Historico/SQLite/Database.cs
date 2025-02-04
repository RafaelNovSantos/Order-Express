using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Database
{
    private readonly SQLiteAsyncConnection _database;

    public Database(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);

        // Criação da tabela de pedidos de forma assíncrona
        _database.CreateTableAsync<Pedido>().Wait();
    }

    public Task<int> SalvarPedidoAsync(Pedido pedido)
    {
        return _database.InsertAsync(pedido);  // Insere um novo pedido assíncronamente
    }

    public Task<List<Pedido>> ObterPedidosAsync()
    {
        return _database.Table<Pedido>().ToListAsync();  // Obtém todos os pedidos de forma assíncrona
    }

    // Método que retorna a conexão SQLite
    public SQLiteAsyncConnection GetConnection()
    {
        return _database;  // Retorna a instância da conexão assíncrona
    }
}
