namespace SocialMediaApp;

public partial class page_inchats : ContentPage
{
    public page_inchats()
    {
        InitializeComponent();
        lbl_name.Text = page_messages.name + "\nFriend";
    }

    private void btn_send_Clicked(object sender, EventArgs e)
    {

        Label l = new Label
        {
            BackgroundColor = Color.FromArgb("#ff91d2ff"),
            Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "\n" + entry_text.Text,
            HorizontalOptions = LayoutOptions.End,
        };
        entry_text.Text = "";
        vsl_messages.Add(l);

    }

    private async void btn_del_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Remove friend", "Do you really want to remove " + page_messages.name + " from your friend list?", "Yes", "No");
        if (answer)
        {
            await DisplayAlert("Friend removed", page_messages.name + " has been removed", "OK");
            await Navigation.PopAsync();
        }
    }
}