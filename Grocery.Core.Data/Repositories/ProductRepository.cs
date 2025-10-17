using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class ProductRepository : DatabaseConnection, IProductRepository
    {
        public ProductRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS Product (
                                [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                [Name] NVARCHAR(80) NOT NULL,
                                [Stock] INTEGER NOT NULL,
                                [ShelfLife] DATE NOT NULL,
                                [Price] DECIMAL(6,2) NOT NULL)");
            List<string> insertQueries = [
                @"INSERT OR IGNORE INTO Product(Id, Name, Stock, ShelfLife, Price) VALUES(1, 'Melk', 300, '2025-09-25', 0.95)",
                    @"INSERT OR IGNORE INTO Product(Id, Name, Stock, ShelfLife, Price) VALUES(2, 'Kaas', 100, '2025-09-30', 7.98)",
                    @"INSERT OR IGNORE INTO Product(Id, Name, Stock, ShelfLife, Price) VALUES(3, 'Brood', 400, '2025-09-12', 2.19)",
                    @"INSERT OR IGNORE INTO Product(Id, Name, Stock, ShelfLife, Price) VALUES(4, 'Cornflakes', 0, '2025-12-31', 1.48)"
            ];
            InsertMultipleWithTransaction(insertQueries);
        }

        public List<Product> GetAll()
        {
            var products = new List<Product>();
            string selectQuery = "SELECT Id, Name, Stock, ShelfLife, Price FROM Product";
            OpenConnection();
            using (var command = new SqliteCommand(selectQuery, Connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    int stock = reader.GetInt32(2);
                    DateOnly shelfLife = DateOnly.FromDateTime(reader.GetDateTime(3));
                    decimal price = reader.GetDecimal(4);
                    products.Add(new Product(id, name, stock, shelfLife, price));
                }
            }
            CloseConnection();
            return products;
        }

        public Product? Get(int id)
        {
            Product? product = null;
            string selectQuery = "SELECT Id, Name, Stock, ShelfLife, Price FROM Product WHERE Id = @Id";
            OpenConnection();
            using (var command = new SqliteCommand(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int pid = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int stock = reader.GetInt32(2);
                        DateOnly shelfLife = DateOnly.FromDateTime(reader.GetDateTime(3));
                        decimal price = reader.GetDecimal(4);
                        product = new Product(pid, name, stock, shelfLife, price);
                    }
                }
            }
            CloseConnection();
            return product;
        }

        public Product Add(Product item)
        {
            string insertQuery = @"INSERT INTO Product(Name, Stock, ShelfLife, Price) VALUES(@Name, @Stock, @ShelfLife, @Price); SELECT last_insert_rowid();";
            OpenConnection();
            using (var command = new SqliteCommand(insertQuery, Connection))
            {
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@Stock", item.Stock);
                command.Parameters.AddWithValue("@ShelfLife", item.ShelfLife.ToDateTime(TimeOnly.MinValue));
                command.Parameters.AddWithValue("@Price", item.Price);
                item.Id = Convert.ToInt32(command.ExecuteScalar());
            }
            CloseConnection();
            return item;
        }

        public Product? Delete(Product item)
        {
            string deleteQuery = "DELETE FROM Product WHERE Id = @Id";
            OpenConnection();
            using (var command = new SqliteCommand(deleteQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", item.Id);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }

        public Product? Update(Product item)
        {
            string updateQuery = @"UPDATE Product SET Name = @Name, Stock = @Stock, ShelfLife = @ShelfLife, Price = @Price WHERE Id = @Id";
            OpenConnection();
            using (var command = new SqliteCommand(updateQuery, Connection))
            {
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@Stock", item.Stock);
                command.Parameters.AddWithValue("@ShelfLife", item.ShelfLife.ToDateTime(TimeOnly.MinValue));
                command.Parameters.AddWithValue("@Price", item.Price);
                command.Parameters.AddWithValue("@Id", item.Id);
                command.ExecuteNonQuery();
            }
            CloseConnection();
            return item;
        }
    }
}
