using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;
using studakDesktopCore.Model;

namespace studakDesktopCore.Pages;

public partial class EventPage : UserControl
{
    private string token;
    private ProgressBar _loader;
    private readonly HttpClient _httpClient;

    public EventPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        token = ApplicationState.GetValue<string>("token");
        SetAllData();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _loader = this.FindControl<ProgressBar>("Loader");
        EventListBox = this.FindControl<ListBox>("EventListBox");
        AddEventBtn = this.FindControl<Button>("AddEventBtn");
    }

    
    private async void SetAllData()
    {
        _loader.IsVisible = true;

        // Добавляем токен в заголовок Authorization
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwt = tokenHandler.ReadJwtToken(token);
        
        string role = jwt.Claims.First(c =>
            c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;

        if (role == "1")
        {
            AddEventBtn.IsVisible = true;
        }

        // Отправляем GET-запрос к API
        var response = await _httpClient.GetAsync("https://localhost:7156/api/Event/AllEvents");
        
        
        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            List<EventData> eventData = JsonConvert.DeserializeObject<List<EventData>>(responseData);
            EventListBox.Items = eventData;
        }
        _loader.IsVisible = false;
    }
    
    private void HandleListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ListBox listBox = (ListBox)sender;
        var a = listBox.SelectedItem.ToString();
        EventData selectedItem = (EventData)listBox.SelectedItem;
        string selectedName = selectedItem.Name;
        
        Navigation.NavigateTo(new EventDataPage(selectedName));
    }

    private void HandleListBoxItemPointerPressed(object sender, PointerPressedEventArgs e)
    {
        ListBox listBox = (ListBox)sender;
        var a = listBox.SelectedItem.ToString();
        EventData selectedItem = (EventData)listBox.SelectedItem;
        string selectedName = selectedItem.Name;
        
        Navigation.NavigateTo(new EventDataPage(selectedName));
    }

    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new UserPage());
    }

    private void AddEventBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new EventAddPage());
    }
}