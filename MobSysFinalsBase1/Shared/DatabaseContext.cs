using MobSysFinalsBase1.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobSysFinalsBase1.Shared
{
    public class DatabaseContext
    {
        SQLiteAsyncConnection database;
        public static DatabaseContext Instance { set; get; }
        public DatabaseContext()
        {
            DatabaseContext.Instance = this;
        }

        public async Task Init()
        {
            if (database is not null)
                return;

            database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await database.CreateTableAsync<User>();
            await database.CreateTableAsync<Favourite>();
            await database.CreateTableAsync<Book>();
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
                return await database.UpdateAsync(incoming);
            else
                return await database.InsertAsync(incoming);
        }

        public async Task<int> DeleteUser(User incoming)
        {
            await Init();
            return await database.DeleteAsync(incoming);
        }

        public async Task<List<Favourite>> GetFavourites(int userId)
        {
            await Init();
            return await database.Table<Favourite>().Where(f => f.UserID == userId).ToListAsync();
        }

        public async Task<int> AddFavourite(Favourite favourite)
        {
            await Init();
            return await database.InsertAsync(favourite);
        }

        public async Task<int> RemoveFavourite(int id)
        {
            await Init();
            return await database.DeleteAsync<Favourite>(id);
        }

        public async Task<Favourite> GetFavourite(int userId, string title, string author)
        {
            await Init();
            return await database.Table<Favourite>()
                .Where(f => f.UserID == userId && f.Title == title && f.Author == author)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Book>> GetBooks()
        {
            await Init();
            return await database.Table<Book>().ToListAsync();
        }

        public async Task<Book> GetBook(int id)
        {
            await Init();
            return await database.Table<Book>().Where(x => x.ID == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveBook(Book book)
        {
            await Init();
            if (book.ID != 0)
                return await database.UpdateAsync(book);
            else
                return await database.InsertAsync(book);
        }

        public async Task<int> DeleteBook(int id)
        {
            await Init();
            return await database.DeleteAsync<Book>(id);
        }
    }
}