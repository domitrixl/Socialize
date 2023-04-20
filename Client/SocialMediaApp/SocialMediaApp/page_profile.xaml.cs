using Microsoft.Maui.Controls;

namespace SocialMediaApp;

public partial class page_profile : ContentPage
{
    public static int Row = 2;
    public static int Col = 1;
    public static int likedRow = 2;
    public static int likedCol = 1;
    RowDefinition rowDef1;
    List<ImageButton> imageButtons = new List<ImageButton>();


    public page_profile()
    {
        InitializeComponent();
        editor_aboutme.Text = Preferences.Default.Get("aboutme", "");
    }


    private async void btn_profilepicture_Clicked(object sender, EventArgs e)
    {
        var res = await PickAndShow(PickOptions.Default);
        btn_profilepicture.Source = res.FullPath;

    }



    private async void btn_addpics_Clicked(object sender, EventArgs e)
    {
        var res = await PickAndShow(PickOptions.Default);
        if (res != null)
        {


            ImageButton imageButton = new ImageButton
            {
                Source = res.FullPath,
                Aspect = Aspect.Fill,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand

            };

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                bool result = await DisplayAlert("Remove Picture", "Do you want to remove this picture?", "Yes", "No");

                if (result)
                {
                    grid_maingrid.Remove(imageButton);
                }
            };
            imageButton.GestureRecognizers.Add(tapGestureRecognizer);
            grid_maingrid.Add(imageButton);
            imageButtons.Add(imageButton);
            grid_maingrid.SetColumn(imageButton, Col);
            grid_maingrid.SetRow(imageButton, Row);
            Col++;

            if (Col % 3 == 0)
            {
                rowDef1 = new RowDefinition { Height = 100 };
                grid_maingrid.RowDefinitions.Add(rowDef1);
                Row++;
                Col = 0;
            }
        }
    }

    public async Task<FileResult> PickAndShow(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = await result.OpenReadAsync();
                    var image = ImageSource.FromStream(() => stream);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }

        return null;
    }

    private void editor_aboutme_Completed(object sender, EventArgs e)
    {
        Preferences.Default.Set("aboutme", editor_aboutme.Text);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}