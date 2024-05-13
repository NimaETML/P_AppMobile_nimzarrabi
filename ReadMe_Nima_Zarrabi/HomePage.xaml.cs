namespace ReadMe_Nima_Zarrabi
{
    public partial class HomePage : ContentPage
    {
        int count = 0;


        public HomePage()
        {
            InitializeComponent();

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            CounterBtn.Text = $" 5935354546266 ";
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}
