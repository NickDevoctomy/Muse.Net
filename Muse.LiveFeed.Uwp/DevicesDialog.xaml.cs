using Muse.Net.Models;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Muse.LiveFeed.Uwp
{
    public sealed partial class DevicesDialog : ContentDialog
    {
        public MuseDevice SelectedDevice
        {
            get
            {
                return (MuseDevice)DevicesListBox.SelectedItem;
            }
        }

        public DevicesDialog()
        {
            this.InitializeComponent();
        }

        public async Task AddDeviceToList(MuseDevice museDevice)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                DevicesListBox.Items.Add(museDevice);
            });
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
