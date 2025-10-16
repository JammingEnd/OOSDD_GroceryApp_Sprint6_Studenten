using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        public GroceryListItemsRepository()
        {
            CreateTable(@"CREATE TABLE IF NOT EXISTS GroceryListItems (
                                [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                [GroceryListId] INTEGER NOT NULL,
                                [ProductId] INTEGER NOT NULL,
                                [Amount] INTEGER NOT NULL)");
            List<string> insertQueries =
                [@"INSERT OR IGNORE INTO GroceryListItems(Id, GroceryListId, ProductId, Amount) VALUES(1,1,1,3)",
                     @"INSERT OR IGNORE INTO GroceryListItems(Id, GroceryListId, ProductId, Amount) VALUES(2,1,2,1)",
                     @"INSERT OR IGNORE INTO GroceryListItems(Id, GroceryListId, ProductId, Amount) VALUES(3,1,3,4)",
                     @"INSERT OR IGNORE INTO GroceryListItems(Id, GroceryListId, ProductId, Amount) VALUES(4,2,1,2)",
                     @"INSERT OR IGNORE INTO GroceryListItems(Id, GroceryListId, ProductId, Amount) VALUES(5,2,2,5)"];
            InsertMultipleWithTransaction(insertQueries);
        }

        public List<GroceryListItem> GetAll()
        {
            var items = new List<GroceryListItem>();
            string selectQuery = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems";
            OpenConnection();
            using (var command = new SqliteCommand(selectQuery, Connection))
            {
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    items.Add(new GroceryListItem(id, groceryListId, productId, amount));
                }
            }
            CloseConnection();
            return items;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int id)
        {
            var items = new List<GroceryListItem>();
            string selectQuery = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems WHERE GroceryListId = @GroceryListId";
            OpenConnection();
            using (var command = new SqliteCommand(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@GroceryListId", id);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int itemId = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    items.Add(new GroceryListItem(itemId, groceryListId, productId, amount));
                }
            }
            CloseConnection();
            return items;
        }

        public GroceryListItem? Get(int id)
        {
            GroceryListItem? item = null;
            string selectQuery = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems WHERE Id = @Id";
            OpenConnection();
            using (var command = new SqliteCommand(selectQuery, Connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int itemId = reader.GetInt32(0);
                    int groceryListId = reader.GetInt32(1);
                    int productId = reader.GetInt32(2);
                    int amount = reader.GetInt32(3);
                    item = new GroceryListItem(itemId, groceryListId, productId, amount);
                }
            }
            CloseConnection();
            return item;
        }

        GroceryListItem IGroceryListItemsRepository.Add(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        GroceryListItem? IGroceryListItemsRepository.Delete(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        GroceryListItem? IGroceryListItemsRepository.Update(GroceryListItem item)
        {
            throw new NotImplementedException();
        }

        // Add, Update, and Delete methods should also be updated to use database calls for full consistency.
    }
}
