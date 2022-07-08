using NotesContracts;
using System.Text.Json;
using System.Text;
using System.Net;
using Test.ViewModels;
using NotesContracts.AuthAPI.Requests;
using NotesContracts.AuthAPI.Responses;

namespace Test.Pages;

public partial class LoginPage : ContentPage
{

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        HttpClient client = new HttpClient ();

        if (String.IsNullOrEmpty(UsernameField.Text) || String.IsNullOrEmpty(PasswordField.Text))
        {
            await DisplayAlert("Attenzione", "Completa tutti i campi", "OK");
        }
        else
        {
            var endpoint = new Uri("https://localhost:7125/login");
            var json = JsonSerializer.Serialize<UserLoginModel>(new()
            {
                Type = 0,
                Username = UsernameField.Text,
                Password = PasswordField.Text
            });
            var payload = new StringContent(json, Encoding.UTF8, "application/json");
            var result = client.PostAsync(endpoint, payload).Result;

            if (result.IsSuccessStatusCode)
            {
                var tokens = JsonSerializer.Deserialize<LoginTokens>(result.Content.ReadAsStream());

                await SecureStorage.SetAsync("JWT_token", tokens.Token);

                await SecureStorage.SetAsync("Refresh_token", tokens.RefreshToken);

                await SecureStorage.SetAsync("isLogged", "True");

                await DisplayAlert("Avviso", "Loggatto correttamente", "OK");

                await Navigation.PopAsync();
            }
            else if (result.StatusCode == HttpStatusCode.Unauthorized && result.Headers.Contains("Token-Expired"))
            {
                await SecureStorage.SetAsync("isLogged", "False");
                await DisplayAlert("Avviso", "JWT scaduto", "OK");
            }
            else
            {
                await SecureStorage.SetAsync("isLogged", "False");
                await DisplayAlert("Avviso", "Username o password errata", "OK");
            }
        }

    }
}