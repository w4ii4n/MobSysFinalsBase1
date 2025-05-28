using System.Collections.Generic;
using System.Threading.Tasks;
using MobSysFinalsBase1.Models;
using MobSysFinalsBase1.Shared;

namespace MobSysFinalsBase1.Services
{
    public class BookService
    {
        private readonly DatabaseContext _db;
        public BookService(DatabaseContext db)
        {
            _db = db;
        }

        public async Task<List<Book>> GetBooks()
        {
            return await _db.GetBooks();
        }

        public async Task<Book> GetBook(int id)
        {
            return await _db.GetBook(id);
        }

        public async Task<int> AddOrUpdateBook(Book book)
        {
            return await _db.SaveBook(book);
        }

        public async Task<int> DeleteBook(int id)
        {
            return await _db.DeleteBook(id);
        }
    }
}