using Muse.Net.Models;
using Muse.Net.Models.Enums;
using Muse.Net.Services;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Muse.LiveFeed.Uwp
{
    public sealed partial class MainPage : Page
    {
        private readonly IMuseSamplerService _museSamplerService = new MuseSamplerService(new MuseSamplerServiceConfiguration { SamplePeriod = new System.TimeSpan(0, 0, 5) });
        private DevicesDialog _devicesDialog;
        private readonly IMuseDeviceDiscoveryService _museDeviceDiscoveryService;
        private readonly IMuseClient _museClient;
        private Task _searching;

        public MainPage()
        {
            this.InitializeComponent();
            _museDeviceDiscoveryService = (IMuseDeviceDiscoveryService)AppServices.Instance.ServiceProvider.GetService(typeof(IMuseDeviceDiscoveryService));
            _museClient = (IMuseClient)AppServices.Instance.ServiceProvider.GetService(typeof(IMuseClient));
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            _devicesDialog = new DevicesDialog();
            _devicesDialog.Closing += _devicesDialog_Closing;
            var showingDevicesDialog = _devicesDialog.ShowAsync();
            _searching = _museDeviceDiscoveryService.GetMuseDevices(FoundMuseDevice);
        }

        private async void _devicesDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if(args.Cancel)
            {
                return;
            }

            var selectedDevice = _devicesDialog.SelectedDevice;
            if(selectedDevice != null)
            {
                var connected = await _museClient.Connect(ulong.Parse(selectedDevice.Id));
                if (connected)
                {
                    await _museClient.Subscribe(
                        Channel.EEG_AF7,
                        Channel.EEG_AF8,
                        Channel.EEG_TP10,
                        Channel.EEG_TP9);

                    _museClient.NotifyEeg += _museClient_NotifyEeg1;
                    await _museClient.Resume();
                }
            }
        }

        private async void FoundMuseDevice(MuseDevice museDevice)
        {
            await _devicesDialog.AddDeviceToList(museDevice);
        }

        private void _museClient_NotifyEeg1(object sender, MuseClientNotifyEegEventArgs e)
        {
            _museSamplerService.Sample(
                e.Channel,
                e.Encefalogram.Samples);
        }
    }
}
