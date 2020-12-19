using Muse.Net.Models;
using System.Windows.Forms;

namespace Muse.LiveFeed
{
    public partial class frmDevices : Form
    {
        public MuseDevice SelectedDevice
        {
            get
            {
                return (MuseDevice)lisDevices.SelectedItem;
            }
        }

        public frmDevices()
        {
            InitializeComponent();
        }

        public void AddDeviceToList(MuseDevice museDevice)
        {
            lisDevices.Items.Add(museDevice);
        }

        private void butOk_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
