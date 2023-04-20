using Socialize;

namespace SocialMediaApp;

public partial class page_login : ContentPage
{
    public page_login()
    {
        InitializeComponent();
    }

    private void btn_register_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new page_register();
        Preferences.Default.Set("loggedin", true);
    }
    protected override bool OnBackButtonPressed()
    {
        Application.Current.Quit();
        return true;
    }

    private async void btn_login_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new AppShell();
        Preferences.Default.Set("bearer", await APIHandler.Users.Login(entry_username.Text, entry_password.Text));
        Preferences.Default.Set("loggedin", true);
    }
}