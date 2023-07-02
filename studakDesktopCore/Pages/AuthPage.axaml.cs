using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;


namespace studakDesktopCore.Pages;

public partial class AuthPage : UserControl
{
    private ProgressBar _loader;
    private readonly HttpClient _httpClient;

    public AuthPage()
    {
        _httpClient = new HttpClient();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _loader = this.FindControl<ProgressBar>("Loader");
        
        UsernameInput = this.Find<TextBox>("UsernameInput");
        PasswordInput = this.Find<TextBox>("PasswordInput");
        ErrorsLine = this.Find<TextBlock>("ErrorsLine");
    }

    private async void LoginBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        // Показать индикатор загрузки перед началом запроса
        _loader.IsVisible = true;
        ErrorsLine.Text = "";
        
        // Создаем контент, который будет отправлен в POST-запросе
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"username", UsernameInput.Text},
            {"password", PasswordInput.Text}
        });
        
        // Выполняем POST-запрос к указанному URL-адресу, используя переданный контент
        var response = await _httpClient.PostAsync("https://localhost:7156/api/Auth/login", content);
        _loader.IsVisible = false;

        if (response.IsSuccessStatusCode)
        {
            string token = await response.Content.ReadAsStringAsync();
            ApplicationState.SetValue("token", token);
            Navigation.NavigateTo(new UserPage());
        }
        else
        {
            ErrorsLine.Text = "Неверное имя пользователя или пароль!";
        }
    }

    private void ExitBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }

    private async void SingUp_OnClick(object? sender, RoutedEventArgs e)
    {
        // Показать индикатор загрузки перед началом запроса
        _loader.IsVisible = true;
        ErrorsLine.Text = "";
        
        // Создаем контент, который будет отправлен в POST-запросе
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"username", UsernameInput.Text},
            {"password", PasswordInput.Text}
        });
        
        // Выполняем POST-запрос к указанному URL-адресу, используя переданный контент
        var responseRegister = await _httpClient.PostAsync("https://localhost:7156/api/Auth/register", content);
        

        if (responseRegister.IsSuccessStatusCode)
        {
            // Выполняем POST-запрос к указанному URL-адресу, используя переданный контент
            var responseLogin = await _httpClient.PostAsync("https://localhost:7156/api/Auth/login", content);
            _loader.IsVisible = false;

            if (responseLogin.IsSuccessStatusCode)
            {
                string token = await responseLogin.Content.ReadAsStringAsync();
                ApplicationState.SetValue("token", token);
                _loader.IsVisible = false;
                Navigation.NavigateTo(new UserPage());
            }
            else
            {
                ErrorsLine.Text = "Неверное имя пользователя или пароль!";
            }
        }
        else
        {
            ErrorsLine.Text = "Невозможно создать аккаунт!";
        }
        _loader.IsVisible = false;
    }
}