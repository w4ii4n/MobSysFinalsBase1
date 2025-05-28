using Microsoft.AspNetCore.Components;
using MobSysFinalsBase1.Shared;
using MobSysFinalsBase1.Models;
using MobSysFinalsBase1.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace MobSysFinalsBase1.Components.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject] public AppShellContext AppShell { get; set; }
        [Inject] public NavigationManager Nav { get; set; }
        [Inject] public DatabaseContext DB { get; set; }
        [Inject] public BookService BookService { get; set; }

        public bool IsGridView { get; set; } = true;
        public bool IsLoading { get; set; } = false;
        public List<Book> BooksForPage { get; set; }
        public Book SelectedBook { get; set; }
        public bool IsBookFavourited { get; set; }

        public void GoToBooks()
        {
            Nav.NavigateTo("/books");
        }

        protected override async Task OnInitializedAsync()
        {
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

            await LoadBooks();
        }

        public void SetGrid(bool grid)
        {
            IsGridView = grid;
        }

        public async Task LoadBooks()
        {
            IsLoading = true;
            StateHasChanged();
            BooksForPage = await BookService.GetBooks();
            IsLoading = false;
            StateHasChanged();
        }

        public async void ShowDetails(Book book)
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
            var amazonLink = !string.IsNullOrWhiteSpace(SelectedBook.AmazonLink)
                ? SelectedBook.AmazonLink
                : $"https://www.amazon.com/s?k={Uri.EscapeDataString(SelectedBook.Title + " " + SelectedBook.Author)}&i=stripbooks";
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
                    AmazonLink = SelectedBook.AmazonLink,
                    CoverImagePath = SelectedBook.CoverImagePath,
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

        public async void DeleteBook()
        {
            if (SelectedBook == null) return;
            await DB.DeleteBook(SelectedBook.ID);
            await LoadBooks();
            SelectedBook = null;
            StateHasChanged();
        }
    }
}