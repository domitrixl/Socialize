using Socialize;
namespace SocialMediaApp;

public partial class page_register : ContentPage
{
    public page_register()
    {
        InitializeComponent();
    }

    private async void btn_register_Clicked(object sender, EventArgs e)
    {
        bool temp = IsValidEmail(entry_mail.Text);
        if (temp)
        {
            if (entry_password.Text == entry_re.Text)
            {
                Application.Current.MainPage = new AppShell();
                string username, password;
                username = entry_username.Text;
                password = entry_password.Text;

                Preferences.Default.Set("privKey", await APIHandler.Users.Register(username, password));
                Preferences.Default.Set("bearer", await APIHandler.Users.Login(username, password));
                Preferences.Default.Set("loggedin", true);
            }
            else
            {
                await DisplayAlert("Error", "The Passwords you have entered do not match!", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "The Mail you have entered is not valid.", "OK");
        }
    }
    bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
        {
            return false; // suggested by @TK-421
        }
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }
}