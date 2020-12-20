using Muse.Net.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Muse.LiveFeed.Uwp
{
    public sealed partial class MainPage : Page
    {
        private readonly IMuseSamplerService _museSamplerService = new MuseSamplerService(new MuseSamplerServiceConfiguration { SamplePeriod = new System.TimeSpan(0, 0, 5) });
        private readonly IMuseClient _museClient;

        public MainPage()
        {
            this.InitializeComponent();
            _museClient = (IMuseClient)AppServices.Instance.ServiceProvider.GetService(typeof(IMuseClient));
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            /*var connected = await _museClient.Connect();
            if(connected)
            {
                await _museClient.Subscribe(
                    Channel.EEG_AF7,
                    Channel.EEG_AF8,
                    Channel.EEG_TP10,
                    Channel.EEG_TP9);

                _museClient.NotifyEeg += _museClient_NotifyEeg1;
                await _museClient.Resume();
            }*/
        }

        private void _museClient_NotifyEeg1(object sender, MuseClientNotifyEegEventArgs e)
        {
            _museSamplerService.Sample(
                e.Channel,
                e.Encefalogram.Samples);
        }
    }
}
