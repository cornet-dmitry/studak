using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HarfBuzzSharp;
using Newtonsoft.Json;
using studakDesktopCore.Model;

namespace studakDesktopCore.Pages;

public partial class EventAddPage : UserControl
{
    private string token;
    private ProgressBar _loader;
    private readonly HttpClient _httpClient;
    
    public EventAddPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        _loader = this.FindControl<ProgressBar>("Loader");
        token = ApplicationState.GetValue<string>("token");
        OrgComboBox = this.FindControl<ComboBox>("OrgComboBox");
        PersonComboBox = this.FindControl<ComboBox>("PersonComboBox");

        Name = this.FindControl<TextBox>("Name");
        Description = this.FindControl<TextBox>("Description");
        StartDate = this.FindControl<TextBox>("StartDate");
        EndDate = this.FindControl<TextBox>("EndDate");
        StartTime = this.FindControl<TextBox>("StartTime");
        EndTime = this.FindControl<TextBox>("EndTime");
        Rate = this.FindControl<TextBox>("Rate");
        ErrorsLine = this.FindControl<TextBlock>("ErrorsLine");
        
        SetComboBoxData();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void SetComboBoxData()
    {
        _loader.IsVisible = true;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var responseOrg = await _httpClient.GetAsync("https://localhost:7156/api/Organization/AllOrgInfo");

        if (responseOrg.IsSuccessStatusCode)
        {
            var responseData = await responseOrg.Content.ReadAsStringAsync();
            List<OrganizationData> organizationDataList = JsonConvert.DeserializeObject<List<OrganizationData>>(responseData);
            var comboBoxItems = organizationDataList.Select(x => x.Id + ": " + x.ShortName).ToList();
            OrgComboBox.Items = comboBoxItems;
        }
        
        var responsePerson = await _httpClient.GetAsync("https://localhost:7156/api/UserData/AllUsers");

        if (responsePerson.IsSuccessStatusCode)
        {
            var responseData = await responsePerson.Content.ReadAsStringAsync();
            List<UserData> userDataList = JsonConvert.DeserializeObject<List<UserData>>(responseData);
            
            var comboBoxItems = userDataList
                .Select(x => x.Id + ": " + x.UserLogin + " - " + x.Surname + " " + x.Name).ToList();
        
            PersonComboBox.Items = comboBoxItems;
        }
        _loader.IsVisible = false;
    }

    private async void SaveUserBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        _loader.IsVisible = true;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            var eventName = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "name", Name.Text },
            });
            var checkName = await _httpClient
                .PostAsync("https://localhost:7156/api/Event/CheckEventName", eventName);
            
            if (!checkName.IsSuccessStatusCode)
            {
                _loader.IsVisible = false;
                ErrorsLine.Text = "Ошибка: такое название уже есть!";
                return;
            }
            
            string selectedOrgValueId = OrgComboBox.SelectedItem.ToString().Split(':')[0];
            string selectedPersonValueId = PersonComboBox.SelectedItem.ToString().Split(':')[0];
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"organization", selectedOrgValueId},
                {"responsible", selectedPersonValueId},
                {"name", Name.Text},
                {"description", Description.Text},
                {"startDate", StartDate.Text},
                {"endDate", EndDate.Text},
                {"startTime", StartTime.Text},
                {"endTime", EndTime.Text},
                {"rate", Rate.Text},
            });

            var response = await _httpClient.PostAsync("https://localhost:7156/api/Event/AddEvent", content);
            
            if (response.IsSuccessStatusCode)
            {
                _loader.IsVisible = false;
                Navigation.NavigateTo(new EventPage());
            }
        }
        catch (Exception exception)
        {
            _loader.IsVisible = false;
            ErrorsLine.Text = "Ошибка заполнения данных: проверьте, все ли поля были заполнены!\n";
            return;
        }
    }

    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new EventPage());
    }
}