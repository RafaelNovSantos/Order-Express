﻿using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Database
{
    private readonly SQLiteAsyncConnection _database;

    public Database(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);

        // Criação da tabela de pedidos de forma assíncrona
        _database.CreateTableAsync<ProdutosPedido>().Wait();
        _database.CreateTableAsync<Planilha>().Wait();
        _database.CreateTableAsync<InfoPedido>().Wait();
    }

    // PRODUTOS NO PEDIDO

    public Task<int> SalvarProdutosAsync(ProdutosPedido produtosPedido)
    {
        return _database.InsertAsync(produtosPedido);  // Insere um novo pedido assíncronamente
    }


    public Task<List<ProdutosPedido>> ObterPedidosAsync()
    {
        return _database.Table<ProdutosPedido>().ToListAsync();  // Obtém todos os pedidos de forma assíncrona
    }

    public Task<int> DeletarProdutoPorNumeroPedidoAsync(int produtoId)
    {
        return _database.Table<ProdutosPedido>()
                        .Where(p => p.NumeroPedido == produtoId)  // Filtra o produto pelo ID
                        .DeleteAsync();  // Deleta todos os produtos que correspondem ao critério
    }

    public Task<int> AtualizarProdutoAsync(ProdutosPedido produtosPedido)
    {
        return _database.UpdateAsync(produtosPedido);  // Atualiza o produto existente
    }


    // PLANILHA
    public Task<int> SalvarPlanilhaAsync(Planilha planilha)
    {
        return _database.InsertAsync(planilha);  // Insere um novo pedido assíncronamente
    }


    public Task<List<Planilha>> ObterPlanilhaAsync()
    {
        return _database.Table<Planilha>().ToListAsync();  // Obtém todos os pedidos de forma assíncrona
    }



    // INFO PEDIDOS
    public Task<int> SalvarInfoPedidoAsync(InfoPedido infoPedido)
    {
        return _database.InsertAsync(infoPedido);  // Insere um novo pedido assíncronamente
    }

    public Task<List<InfoPedido>> ObterInfoPedidoAsync()
    {
        return _database.Table<InfoPedido>().ToListAsync();  // Obtém todos os pedidos de forma assíncrona
    }

    public Task<int> DeletarPedidoPorNumeroPedidoAsync(int pedidoId)
    {
        return _database.Table<InfoPedido>()
                        .Where(p => p.NumeroPedido == pedidoId)  // Filtra o produto pelo ID
                        .DeleteAsync();  // Deleta todos os produtos que correspondem ao critério
    }

    public Task<int> AtualizarInfoPedidoAsync(InfoPedido infoPedido)
    {
        return _database.UpdateAsync(infoPedido);  // Atualiza o produto existente
    }
    // Método que retorna a conexão SQLite
    public SQLiteAsyncConnection GetConnection()
    {
        return _database;  // Retorna a instância da conexão assíncrona
    }
}
