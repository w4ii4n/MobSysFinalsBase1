using MobSysFinalsBase1.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobSysFinalsBase1.Shared
{
    /// <summary>
    /// Centralized Class for handing local SQLite Database things for the App
    /// Loaded in MauiProgram as Singleton (one instance only within the App)
    /// </summary>
    public class DatabaseContext
    {
        SQLiteAsyncConnection database;
        public static DatabaseContext Instance { set; get; }
        public DatabaseContext()
        {
            //init from constructor
            DatabaseContext.Instance = this;
        }

        /// <summary>
        /// Initialize Database Availability
        /// </summary>
        /// <returns></returns>
        public async Task Init()
        {
            if (database is not null)
                return;

            database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            //Create tables
            await database.CreateTableAsync<User>();
            await database.CreateTableAsync<Favourite>();
        }

        public async Task<List<User>> Users()
        {
            await Init();
            return await database.Table<User>().ToListAsync();
        }

        public async Task<int> SaveUser(User incoming)
        {
            await Init();
            if (incoming.ID != 0)
                return await database.UpdateAsync(incoming);//update existing
            else
                return await database.InsertAsync(incoming);//insert new
        }

        public async Task<int> DeleteUser(User incoming)
        {
            await Init();
            return await database.DeleteAsync(incoming);
        }

        // Get all favourites for a specific user
        public async Task<List<Favourite>> GetFavourites(int userId)
        {
            await Init();
            return await database.Table<Favourite>().Where(f => f.UserID == userId).ToListAsync();
        }

        // Add a favourite
        public async Task<int> AddFavourite(Favourite favourite)
        {
            await Init();
            return await database.InsertAsync(favourite);
        }

        // Remove a favourite by its ID
        public async Task<int> RemoveFavourite(int id)
        {
            await Init();
            return await database.DeleteAsync<Favourite>(id);
        }

        // Get a specific favourite by user id, title, and author
        public async Task<Favourite> GetFavourite(int userId, string title, string author)
        {
            await Init();
            return await database.Table<Favourite>()
                .Where(f => f.UserID == userId && f.Title == title && f.Author == author)
                .FirstOrDefaultAsync();
        }
    }
}