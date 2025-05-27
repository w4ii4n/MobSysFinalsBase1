using System.Net.Http.Json;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MobSysFinalsBase1.Services
{
    public class BookService
    {
        public List<string> Genres { get; } = new()
        {
            "romance", "fantasy", "horror", "science fiction", "mystery", "biography", "history", "comics", "adventure", "technology", "poetry"
        };

        Random _rand = new();

        public List<string> GetRandomGenres(int count)
        {
            return Genres.OrderBy(x => _rand.Next()).Take(count).ToList();
        }

        public async Task<List<BookInfo>> GetBooksForGenre(string genre, int count)
        {
            var url = $"https://www.googleapis.com/books/v1/volumes?q=subject:{Uri.EscapeDataString(genre)}&maxResults={count * 4}";
            using var client = new System.Net.Http.HttpClient();
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return new List<BookInfo>();

            using var stream = await response.Content.ReadAsStreamAsync();
            var json = await JsonDocument.ParseAsync(stream);

            var books = new List<BookInfo>();
            if (json.RootElement.TryGetProperty("items", out var items))
            {
                foreach (var item in items.EnumerateArray())
                {
                    var volumeInfo = item.GetProperty("volumeInfo");
                    var title = volumeInfo.GetProperty("title").GetString() ?? "";
                    string author = "";
                    if (volumeInfo.TryGetProperty("authors", out var authors))
                        author = string.Join(", ", authors.EnumerateArray().Select(a => a.GetString()));

                    string thumbnail = "";
                    string smallThumbnail = "";
                    if (volumeInfo.TryGetProperty("imageLinks", out var img))
                    {
                        if (img.TryGetProperty("thumbnail", out var t))
                            thumbnail = t.GetString() ?? "";
                        if (img.TryGetProperty("smallThumbnail", out var st))
                            smallThumbnail = st.GetString() ?? "";
                    }

                    string description = volumeInfo.TryGetProperty("description", out var desc) ? desc.GetString() ?? "" : "";
                    string publishedDate = volumeInfo.TryGetProperty("publishedDate", out var pd) ? pd.GetString() ?? "" : "";

                    if (!string.IsNullOrEmpty(thumbnail))
                    {
                        books.Add(new BookInfo
                        {
                            Title = title,
                            Author = author,
                            Thumbnail = thumbnail,
                            SmallThumbnail = string.IsNullOrEmpty(smallThumbnail) ? thumbnail : smallThumbnail,
                            PublishedDate = publishedDate,
                            Genre = genre,
                            Description = description
                        });
                    }

                    if (books.Count >= count)
                        break;
                }
            }
            return books;
        }

        public async Task<List<BookInfo>> GetBooksForGenreWithRetry(string genre, int count, int maxTries = 6)
        {
            int tries = 0;
            while (tries < maxTries)
            {
                var books = await GetBooksForGenre(genre, count);
                if (books.Count == count)
                    return books;
                tries++;
            }
            return new List<BookInfo>();
        }
    }

    public class BookInfo
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Thumbnail { get; set; } = "";
        public string SmallThumbnail { get; set; } = "";
        public string PublishedDate { get; set; } = "";
        public string Genre { get; set; } = "";
        public string Description { get; set; } = "";
    }
}