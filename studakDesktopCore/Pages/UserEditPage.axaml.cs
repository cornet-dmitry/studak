using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;
using studakDesktopCore.Model;

namespace studakDesktopCore.Pages;

public partial class UserEditPage : UserControl
{
    private ProgressBar _loader;
    private readonly HttpClient _httpClient;
    private string token;

    public UserEditPage()
    {
        InitializeComponent();

        _httpClient = new HttpClient();
        _loader = this.FindControl<ProgressBar>("Loader");

        token = ApplicationState.GetValue<string>("token");

        Surname = this.FindControl<TextBox>("Surname");
        Name = this.FindControl<TextBox>("Name");
        Patronymic = this.FindControl<TextBox>("Patronymic");
        BirthDay = this.FindControl<TextBox>("BirthDay");
        Phone = this.FindControl<TextBox>("Phone");
        Email = this.FindControl<TextBox>("Email");
        Messenger = this.FindControl<TextBox>("Messenger");
        ErrorsLine = this.FindControl<TextBlock>("ErrorsLine");
        
        SetUserData();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void SetUserData()
    {
        _loader.IsVisible = true;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwt = tokenHandler.ReadJwtToken(token);
        
        string userId = jwt.Claims.First(c =>
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
        
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"userId", userId}
        });
        
        var response = await _httpClient.PostAsync("https://localhost:7156/api/UserData/GetUser", content);
        
        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<IEnumerable<UserData>>(responseData);

            foreach (var u in user)
            {
                Surname.Text = u.Surname;
                Name.Text = u.Name;
                Patronymic.Text = u.Patronymic;
                BirthDay.Text = u.DateBirth.ToString();
                Phone.Text = u.Phone;
                Email.Text = u.Email;
                Messenger.Text = u.Messenger;
            }
            _loader.IsVisible = false;
        }
        
    }

    private async void SaveUserBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        
        /*Блок проверок входных данных на соответствие перед отправкой
         1) Проверка на наличие пустых строк*/
        if (Surname.Text is null | Name.Text is null | Patronymic.Text is null
            | BirthDay.Text is null | Phone.Text is null | Email.Text is null | Messenger.Text is null)
        {
            ErrorsLine.Text = "Ошибка заполнения данных: проверьте, все ли поля были заполнены!";
            return;
        }
        
        _loader.IsVisible = true;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwt = tokenHandler.ReadJwtToken(token);
        
        // Чтение значения конкретного клейма (claim)
        string userId = jwt.Claims.First(c =>
            c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
        
        // Создаем контент, который будет отправлен в POST-запросе
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"userId", userId},
            {"surname", Surname.Text},
            {"name", Name.Text},
            {"patronymic", Patronymic.Text},
            {"phone", Phone.Text},
            {"email", Email.Text},
            {"messenger", Messenger.Text},
            {"dateBirth", BirthDay.Text}
        });
        
        // Отправляем POST-запрос к API
        var response = await _httpClient.PostAsync("https://localhost:7156/api/UserData/addUserData", content);
        
        _loader.IsVisible = false;
        if (response.IsSuccessStatusCode)
        {
            Navigation.NavigateTo(new UserPage());
        }

    }

    private void BackBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Navigation.NavigateTo(new UserPage());
    }
}