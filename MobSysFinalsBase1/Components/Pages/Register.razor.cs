using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MobSysFinalsBase1.Shared;
using System;
using System.Threading.Tasks;

namespace MobSysFinalsBase1.Components.Pages
{
    public partial class Register : ComponentBase
    {
        public string Status = "";
        public string StatusMessage = "";

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public DatabaseContext DB { get; set; }

        public Models.User Model { get; set; } = new Models.User();

        protected override void OnInitialized()
        {
            Status = "";
            StatusMessage = "";
        }

        public async Task RegisterUser()
        {
            try
            {
                Model.Username = Model.Username ?? "";
                Model.Password = Model.Password ?? "";
                Model.FirstName = Model.FirstName ?? "";
                Model.LastName = Model.LastName ?? "";

                await DB.SaveUser(Model);
                Status = "success";
                StatusMessage = "User has been registered successfully!";
                Model = new Models.User();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Status = "danger";
                StatusMessage = $"Registration failed: {ex.Message}";
                StateHasChanged();
            }
        }
    }
}