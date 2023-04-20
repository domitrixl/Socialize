namespace SocialMediaApp;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.PlatformConfiguration.GTKSpecific;
using Microsoft.Maui.Graphics;

public partial class page_messages : ContentPage
{
    public static string name;
    public static int Row = 1;
    public page_messages()
    {
        InitializeComponent();

    }

    private async void btn_newmsg_ClickedAsync(object sender, EventArgs e)
    {
        try
        {
            name = await DisplayPromptAsync("Who do you want to contact?", "Name of your friend:");
            if (name != null)
            {
                adduserstoboard(name);
                await Navigation.PushAsync(new page_inchats());
            }
        }
        catch (Exception)
        {

            throw;
        }


    }
    private async void adduserstoboard(string name)
    {
        msggrid.AddRowDefinition(new RowDefinition { Height = 40 });
        Button b = new Button();
        b.Text = name;
        b.Background = new SolidColorBrush(Colors.Transparent);
        b.TextColor = Colors.White;
        b.Clicked += async (sender, e) =>
        {
            await Navigation.PushAsync(new page_inchats());
        };

        Button c = new Button();
        c.Text = "X";
        c.Background = new SolidColorBrush(Colors.Transparent);
        c.TextColor = Colors.White;
        c.Clicked += async (sender, e) =>
        {
            int i = msggrid.IndexOf(b) - 1;
            msggrid.RowDefinitions[i].Height = 0;
            msggrid.Remove(b);
            msggrid.Remove(c);
        };

        msggrid.Add(b, 0, Row);
        msggrid.Add(c, 1, Row);

        Row++;
    }

    private async void btn_showfriendlist_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new page_friends());
    }
}