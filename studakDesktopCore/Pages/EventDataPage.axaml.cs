using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;
using studakDesktopCore.Model;

namespace studakDesktopCore.Pages;

public partial class EventDataPage : UserControl
{
    private string name;
    private string token;
    private readonly HttpClient _httpClient;

    public EventDataPage() : this(null)
    {
    }
    public EventDataPage(string name)
    {
        InitializeComponent();

        this.name = name;
        token = ApplicationState.GetValue<string>("token");
        _httpClient = new HttpClient();

        PageName = this.FindControl<TextBlock>("PageName");
        Organization = this.FindControl<TextBlock>("Organization");
        Responsible = this.FindControl<TextBlock>("Responsible");
        Description = this.FindControl<TextBlock>("Description");
        Date = this.FindControl<TextBlock>("Date");
        Time = this.FindControl<TextBlock>("Time");
        Rate = this.FindControl<TextBlock>("Rate");
        
        SetAddData();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void SetAddData()
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"name", name}
        });
        
        // Отправляем POST-запрос к API
        var response = await _httpClient.PostAsync("https://localhost:7156/api/Event/GetEvent", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            List<EventData> eventData = JsonConvert.DeserializeObject<List<EventData>>(responseData);

            foreach (var e in eventData)
            {
                PageName.Text = e.Name;
                Organization.Text = "Организация: " + e.Organization;
                Responsible.Text = "Ответственный: " + e.Responsible;
                Description.Text = e.Description;
                Date.Text = "Дата и время: " + e.StartDate.ToString().Split(" ")[0] + " - " + e.EndDate.ToString().Split(" ")[0];
                Time.Text = e.StartTime.ToString().Split(" ")[1]
                            + "-" + e.EndTime.ToString().Split(" ")[1];
                Rate.Text = "Баллы за мероприятие: " + e.Rate;
            }
        }
    }

    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new EventPage());
    }
}














