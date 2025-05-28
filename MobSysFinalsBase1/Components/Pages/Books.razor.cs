using Microsoft.AspNetCore.Components;
using MobSysFinalsBase1.Models;
using MobSysFinalsBase1.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using System;

namespace MobSysFinalsBase1.Components.Pages
{
    public partial class BooksPage : ComponentBase
    {
        [Inject] public DatabaseContext DB { get; set; }
        [Inject] public NavigationManager Nav { get; set; }

        public List<Book> Books { get; set; }
        public bool IsLoading { get; set; } = true;
        public bool IsGridView { get; set; } = true;

        public bool ShowBookDialog { get; set; } = false;
        public Book BookForm { get; set; } = new Book();
        private IBrowserFile fileInput;
        protected bool _showToast = false;

        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            Books = await DB.GetBooks();
            IsLoading = false;
        }

        public void SetGrid(bool grid)
        {
            IsGridView = grid;
        }

        public void ShowAddDialog()
        {
            BookForm = new Book();
            fileInput = null;
            ShowBookDialog = true;
        }

        public void ShowEditDialog(Book book)
        {
            if (book == null) return;
            BookForm = new Book
            {
                ID = book.ID,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                PublishedDate = book.PublishedDate,
                Description = book.Description,
                AmazonLink = book.AmazonLink,
                CoverImagePath = book.CoverImagePath,
                AddedDate = book.AddedDate
            };
            fileInput = null;
            ShowBookDialog = true;
        }

        public void CloseBookDialog()
        {
            ShowBookDialog = false;
            StateHasChanged();
        }

        public async Task OnFileChanged(InputFileChangeEventArgs e)
        {
            fileInput = e.File;
            if (fileInput != null)
            {
                var buffer = new byte[fileInput.Size];
                await fileInput.OpenReadStream(maxAllowedSize: 1024 * 1024 * 2).ReadAsync(buffer);
                var base64 = Convert.ToBase64String(buffer);
                BookForm.CoverImagePath = $"data:{fileInput.ContentType};base64,{base64}";
            }
        }

        public void ShowToast()
        {
            _showToast = true;
            StateHasChanged();
            _ = Task.Delay(2500).ContinueWith(_ => HideToast());
        }

        public void HideToast()
        {
            _showToast = false;
            StateHasChanged();
        }

        public async Task SaveBook()
        {
            if (string.IsNullOrWhiteSpace(BookForm.Title) || string.IsNullOrWhiteSpace(BookForm.Author))
                return;

            if (BookForm.AddedDate == default)
                BookForm.AddedDate = DateTime.Now;
            if (string.IsNullOrWhiteSpace(BookForm.AmazonLink))
                BookForm.AmazonLink = $"https://www.amazon.com/s?k={Uri.EscapeDataString(BookForm.Title + " " + BookForm.Author)}&i=stripbooks";

            await DB.SaveBook(BookForm);
            Books = await DB.GetBooks();
            ShowBookDialog = false;

            ShowToast();

            StateHasChanged();
        }

        public async Task DeleteBook()
        {
            if (BookForm == null || BookForm.ID == 0) return;
            await DB.DeleteBook(BookForm.ID);
            Books = await DB.GetBooks();
            ShowBookDialog = false;
            StateHasChanged();
        }
    }
}