﻿using Microsoft.AspNetCore.Components;
using MobSysFinalsBase1.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobSysFinalsBase1.Components.Pages
{
    public partial class Login : ComponentBase
    {
        public string Status = "";
        public string StatusMessage = "";

        [Inject]
        public AppShellContext AppShell { get; set; }

        /// <summary>
        /// This injects a preloaded copy of NavigationManager from MauiProgram.cs
        /// </summary>
        [Inject]
        public NavigationManager Nav { set; get; }

        /// <summary>
        /// This injects a preloaded copy of DatabaseContext from MauiProgram.cs
        /// </summary>
        [Inject]
        public DatabaseContext DB { set; get; }

        /// <summary>
        /// User model bound to the page
        /// </summary>
        public Models.User Model = new Models.User();

        public async void LoginUser()
        {
            if (string.IsNullOrWhiteSpace(Model.Username) || string.IsNullOrWhiteSpace(Model.Password))
            {
                Status = "danger";
                StatusMessage = "Username/Password cannot be blank or only spaces!";
            }
            else
            {
                var users = await DB.Users();
                var targetUser = (from row in users
                                  where row.Username == Model.Username
                                  && row.Password == Model.Password
                                  select row).FirstOrDefault();

                if (targetUser != null)
                {
                    Status = "success";
                    StatusMessage = "Logged in Successfully!";
                    AppShell.CurrentUser = targetUser;
                    AppShell.IsUserLoggedIn = true;
                    AppShell.SetSessionUser(targetUser);
                    await InvokeAsync(StateHasChanged);
                    Nav.NavigateTo("/?status=success&message=Logged%20in%20Successfully!");
                }
                else
                {
                    Status = "danger";
                    StatusMessage = "Invalid Username or Password!";
                }
            }
            await InvokeAsync(StateHasChanged);
        }

    }
}
