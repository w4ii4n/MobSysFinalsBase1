using Microsoft.AspNetCore.Components;
using MobSysFinalsBase1.Shared;
using MobSysFinalsBase1.Models;
using MobSysFinalsBase1.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobSysFinalsBase1.Components.Pages
{
    public partial class Home : ComponentBase
    {
        public string Status { get; set; } = "";
        public string StatusMessage { get; set; } = "";

        [Inject] public AppShellContext AppShell { get; set; }
        [Inject] public NavigationManager Nav { get; set; }
        [Inject] public DatabaseContext DB { get; set; }
        [Inject] public BookService BookService { get; set; }

        public HomeViewModel Model { get; set; }
        public bool IsGridView { get; set; } = true;
        public int _page = 1;
        public bool IsLoading = false;
        public List<string> BookGenres;
        public List<BookInfo> BooksForPage;
        public BookInfo SelectedBook { get; set; }
        public bool IsBookFavourited { get; set; }

        protected override async void OnInitialized()
        {
            Model = new HomeViewModel();

            var loggedUser = AppShell.GetSessionUser();
            if (loggedUser != null)
            {
                AppShell.CurrentUser = loggedUser;
                AppShell.IsUserLoggedIn = true;
            }
            else
            {
                Nav.NavigateTo("/login");
                return;
            }

            var queryParams = ParseQueryString(Nav.Uri);
            if (queryParams.TryGetValue("status", out var status))
                Status = status;
            if (queryParams.TryGetValue("message", out var msg))
                StatusMessage = msg;

            BookGenres = BookService.Genres;
            await LoadBooks();
            await InvokeAsync(StateHasChanged);
        }

        private Dictionary<string, string> ParseQueryString(string uri)
        {
            var result = new Dictionary<string, string>();
            var queryIndex = uri.IndexOf('?');
            if (queryIndex >= 0 && queryIndex < uri.Length - 1)
            {
                var query = uri.Substring(queryIndex + 1);
                var pairs = query.Split('&', StringSplitOptions.RemoveEmptyEntries);
                foreach (var pair in pairs)
                {
                    var kvp = pair.Split('=', 2);
                    if (kvp.Length == 2)
                    {
                        var key = Uri.UnescapeDataString(kvp[0]);
                        var value = Uri.UnescapeDataString(kvp[1]);
                        result[key] = value;
                    }
                }
            }
            return result;
        }

        public void SetGrid(bool grid)
        {
            IsGridView = grid;
        }

        public async Task LoadBooks()
        {
            IsLoading = true;
            StateHasChanged();
            var randomGenres = BookService.GetRandomGenres(3);
            var books = new List<BookInfo>();
            foreach (var genre in randomGenres)
            {
                var booksForGenre = await BookService.GetBooksForGenreWithRetry(genre, 3);
                books.AddRange(booksForGenre);
            }
            BooksForPage = books;
            IsLoading = false;
            StateHasChanged();
        }

        public async Task NextPage()
        {
            _page++;
            await LoadBooks();
        }
        public async Task PrevPage()
        {
            if (_page > 1)
            {
                _page--;
                await LoadBooks();
            }
        }

        public async void ShowDetails(BookInfo book)
        {
            SelectedBook = book;
            var user = AppShell.GetSessionUser();
            if (user != null)
            {
                var fav = await DB.GetFavourite(user.ID, book.Title, book.Author);
                IsBookFavourited = fav != null;
            }
            else
            {
                IsBookFavourited = false;
            }
            StateHasChanged();
        }

        public void CloseDetails()
        {
            SelectedBook = null;
            StateHasChanged();
        }

        public void OpenAmazon()
        {
            if (SelectedBook == null) return;
            var amazonLink = $"https://www.amazon.com/s?k={Uri.EscapeDataString(SelectedBook.Title + " " + SelectedBook.Author)}&i=stripbooks";
            Nav.NavigateTo(amazonLink, true);
        }

        public async void AddToFavourites()
        {
            var user = AppShell.GetSessionUser();
            if (user != null && SelectedBook != null)
            {
                var fav = new Favourite
                {
                    UserID = user.ID,
                    Title = SelectedBook.Title,
                    Author = SelectedBook.Author,
                    PublishedDate = SelectedBook.PublishedDate,
                    Genre = SelectedBook.Genre,
                    Description = SelectedBook.Description,
                    AmazonLink = $"https://www.amazon.com/s?k={Uri.EscapeDataString(SelectedBook.Title + " " + SelectedBook.Author)}&i=stripbooks",
                    Thumbnail = SelectedBook.Thumbnail,
                    SmallThumbnail = SelectedBook.SmallThumbnail,
                    AddedDate = DateTime.Now
                };
                await DB.AddFavourite(fav);
                IsBookFavourited = true;
                StateHasChanged();
            }
        }

        public async void RemoveFromFavourites()
        {
            var user = AppShell.GetSessionUser();
            if (user != null && SelectedBook != null)
            {
                var fav = await DB.GetFavourite(user.ID, SelectedBook.Title, SelectedBook.Author);
                if (fav != null)
                {
                    await DB.RemoveFavourite(fav.ID);
                    IsBookFavourited = false;
                    StateHasChanged();
                }
            }
        }
    }
}