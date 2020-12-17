using Harthoorn.MuseClient;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Muse.LiveFeed.Uwp
{
    public sealed partial class MainPage : Page
    {
        private MuseClient _museClient = new MuseClient();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var connected = await _museClient.Connect();
            if(connected)
            {
                await _museClient.Subscribe(
                    Channel.EEG_AF7,
                    Channel.EEG_AF8,
                    Channel.EEG_TP10,
                    Channel.EEG_TP9);

                _museClient.NotifyEeg += _museClient_NotifyEeg;
                await _museClient.Resume();
            }
        }

        private void _museClient_NotifyEeg(Channel arg1, Encefalogram arg2)
        {
            System.Diagnostics.Debug.WriteLine($"{arg1} : {arg2.Samples.Length}");
        }
    }
}
