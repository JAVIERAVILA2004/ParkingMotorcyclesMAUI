namespace ParkingMotorcycles
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void btnIniciar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Parking());
        }
    }

}
