using Microsoft.AspNetCore.Components;
using MobSysFinalsBase1.Shared;
using MobSysFinalsBase1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobSysFinalsBase1.Components.Pages
{
    public partial class FavouritesPage : ComponentBase
    {
        [Inject] public DatabaseContext DB { get; set; }
        [Inject] public AppShellContext AppShell { get; set; }
        [Inject] public NavigationManager Nav { get; set; }

        public List<Favourite> Favourites { get; set; }
        public Favourite SelectedFavourite { get; set; }
        public bool IsLoading { get; set; } = true;
        public bool IsGridView { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            var user = AppShell.GetSessionUser();
            if (user == null)
            {
                Nav.NavigateTo("/login");
                return;
            }
            IsLoading = true;
            Favourites = await DB.GetFavourites(user.ID);
            IsLoading = false;
        }

        public void ShowDetails(Favourite fav)
        {
            SelectedFavourite = fav;
            StateHasChanged();
        }

        public void CloseDetails()
        {
            SelectedFavourite = null;
            StateHasChanged();
        }

        public void OpenAmazon(Favourite fav)
        {
            if (!string.IsNullOrWhiteSpace(fav.AmazonLink))
                Nav.NavigateTo(fav.AmazonLink, true);
        }

        public async Task RemoveFavourite(int id)
        {
            await DB.RemoveFavourite(id);
            var user = AppShell.GetSessionUser();
            Favourites = await DB.GetFavourites(user.ID);
            SelectedFavourite = null;
            StateHasChanged();
        }

        public void SetGrid(bool grid)
        {
            IsGridView = grid;
        }
    }
}