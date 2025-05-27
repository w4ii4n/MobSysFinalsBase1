using Microsoft.AspNetCore.Components;
using MobSysFinalsBase1.Shared;
using MobSysFinalsBase1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobSysFinalsBase1.Components.Pages
{
    public partial class Home : ComponentBase
    {
        public string Status { get; set; } = "";
        public string StatusMessage { get; set; } = "";

        [Inject]
        public AppShellContext AppShell { get; set; }

        [Inject]
        public NavigationManager Nav { get; set; }

        [Inject]
        public DatabaseContext DB { get; set; }

        public HomeViewModel Model { get; set; }

        protected override async void OnInitialized()
        {
            Model = new HomeViewModel();

            // Check if user is logged in
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

            // Parse query string for status and message
            var queryParams = ParseQueryString(Nav.Uri);
            if (queryParams.TryGetValue("status", out var status))
                Status = status;
            if (queryParams.TryGetValue("message", out var msg))
                StatusMessage = msg;

            await InvokeAsync(StateHasChanged);
        }

        // Simple built-in query string parser (no dependencies!)
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
    }
}