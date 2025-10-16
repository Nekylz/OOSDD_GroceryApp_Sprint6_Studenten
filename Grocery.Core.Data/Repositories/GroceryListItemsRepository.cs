using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Models;
using Grocery.Core.Data;
using Microsoft.Data.Sqlite;

namespace Grocery.Core.Data.Repositories
{
    public class GroceryListItemsRepository : DatabaseConnection, IGroceryListItemsRepository
    {
        public List<GroceryListItem> GetAll()
        {
            OpenConnection();
            List<GroceryListItem> items = new List<GroceryListItem>();

            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, GroceryListId, ProductId, Amount FROM GroceryListItems";

                using (var reader = command.ExecuteReader())
                {
                    // Lees alle items uit de database
                    while (reader.Read())
                    {
                        items.Add(new GroceryListItem(
                            reader.GetInt32(0), // Id
                            reader.GetInt32(1), // GroceryListId
                            reader.GetInt32(2), // ProductId
                            reader.GetInt32(3)  // Amount
                        ));
                    }
                }
            }

            return items;
        }

        public List<GroceryListItem> GetAllOnGroceryListId(int id)
        {
            // Haal alle items op en filter met LINQ op GroceryListId
            var allItems = GetAll();
            return (from item in allItems
                    where item.GroceryListId == id
                    select item).ToList();
        }

        public GroceryListItem Add(GroceryListItem item)
        {
            OpenConnection();

            using (var command = Connection.CreateCommand())
            {
                // Voeg nieuw item toe en haal de gegenereerde Id op
                command.CommandText = @"
                    INSERT INTO GroceryListItems (GroceryListId, ProductId, Amount) 
                    VALUES (@groceryListId, @productId, @amount);
                    SELECT last_insert_rowid();";

                command.Parameters.AddWithValue("@groceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("@productId", item.ProductId);
                command.Parameters.AddWithValue("@amount", item.Amount);

                int newId = Convert.ToInt32(command.ExecuteScalar());
                item.Id = newId;
            }

            return Get(item.Id);
        }

        public GroceryListItem? Delete(GroceryListItem item)
        {
            // Controleer eerst of het item bestaat
            var existingItem = Get(item.Id);

            if (existingItem == null)
            {
                return null;
            }

            OpenConnection();

            using (var command = Connection.CreateCommand())
            {
                // Verwijder het item uit de database
                command.CommandText = "DELETE FROM GroceryListItems WHERE Id = @id";
                command.Parameters.AddWithValue("@id", item.Id);
                command.ExecuteNonQuery();
            }

            return existingItem;
        }

        public GroceryListItem? Get(int id)
        {
            // Haal alle items op en zoek met LINQ naar het specifieke Id
            var allItems = GetAll();
            return (from item in allItems
                    where item.Id == id
                    select item).FirstOrDefault();
        }

        public GroceryListItem? Update(GroceryListItem item)
        {
            // Controleer eerst of het item bestaat
            var existingItem = Get(item.Id);

            if (existingItem == null)
            {
                return null;
            }

            OpenConnection();

            using (var command = Connection.CreateCommand())
            {
                // Update het item in de database
                command.CommandText = @"
                    UPDATE GroceryListItems 
                    SET GroceryListId = @groceryListId, 
                        ProductId = @productId, 
                        Amount = @amount 
                    WHERE Id = @id";

                command.Parameters.AddWithValue("@groceryListId", item.GroceryListId);
                command.Parameters.AddWithValue("@productId", item.ProductId);
                command.Parameters.AddWithValue("@amount", item.Amount);
                command.Parameters.AddWithValue("@id", item.Id);

                command.ExecuteNonQuery();
            }

            return Get(item.Id);
        }
    }
}