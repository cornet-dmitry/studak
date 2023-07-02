using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Newtonsoft.Json;
using studakDesktopCore.Model;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;

namespace studakDesktopCore.Pages;

public partial class UserPage : UserControl
{
    private string token;
    private readonly HttpClient _httpClient;

    public UserPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        token = ApplicationState.GetValue<string>("token");
        RoleLine = this.FindControl<TextBlock>("RoleLine");
        Control = this.FindControl<ItemsControl>("Control");

        SetAllData();
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void SetAllData()
    {
        // Добавляем токен в заголовок Authorization
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwt = tokenHandler.ReadJwtToken(token);
        
        // Чтение значения конкретного клейма (claim)
        string userId = jwt.Claims.First(c =>
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
        
        string role = jwt.Claims.First(c =>
            c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
        switch (role)
        {
            case "0": RoleLine.Text = "User"; break;
            case "1": RoleLine.Text = "Admin"; break;
        }

        // Создаем контент, который будет отправлен в POST-запросе
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"userId", userId}
        });
        
        // Отправляем POST-запрос к API
        var response = await _httpClient.PostAsync("https://localhost:7156/api/UserData/GetUser", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<IEnumerable<UserData>>(responseData);
            
            Control.Items = user;
        }
    }

    private void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new UserEditPage());
    }

    private void EventsBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new EventPage());
    }

    private void LogOutBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        ApplicationState.ClearValue("token");
        Navigation.NavigateTo(new AuthPage());
    }
}