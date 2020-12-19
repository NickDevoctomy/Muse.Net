using Muse.Net.Client;
using Muse.Net.Models.Enums;
using Muse.Net.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Muse.LiveFeed
{
    public partial class frmMain : Form
    {
        private IMuseDeviceDiscoveryService _museDeviceDiscoveryService;
        private IMuseClient _client;
        private frmDevices _devices;
        private Task _searching;

        public frmMain()
        {
            InitializeComponent();

            _museDeviceDiscoveryService = new WindowsDesktopMuseDeviceDiscoveryService();
            _client = new MuseClient();
        }

        private void FoundMuseDevice(string name, string id)
        {
            _devices.Invoke(new MethodInvoker(() =>
           {
               _devices.AddDeviceToList(name);
           }));     
        }

        public void Report(string text)
        {
            lblStatus.Text = text;
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (_client.Connected)
            {
                await _client.Resume();
                Report("Running.");
            }
            else
            {
                _searching = StartFeed(Report);
            }
        }

        private async void btnStop_Click(object sender, EventArgs e)
        {
            await _client.Pause();
            Report("Paused.");
        }

        public Task StartFeed(Action<string> report)
        {
            report("Searching...");
            _devices = new frmDevices();
            _devices.FormClosing += _devices_FormClosing;
            _devices.Show();
            return _museDeviceDiscoveryService.GetMuseDevices(FoundMuseDevice);

            //var ok = await client.Connect();
            //if (ok)
            //{
            //    await client.Subscribe(
            //        Channel.EEG_AF7,
            //        Channel.EEG_AF8,
            //        Channel.EEG_TP10,
            //        Channel.EEG_TP9);

            //    client.NotifyEeg += Client_NotifyEeg1;
            //    report("Starting...");
            //    await client.Resume();
            //    report("Running.");
            //    btnStart.Text = "Start";
            //}
        }

        private void _devices_FormClosing(object sender, FormClosingEventArgs e)
        {
            //cancel searching task
            //_searching;
        }

        private void Client_NotifyEeg1(object sender, MuseClientNotifyEegEventArgs e)
        {
            graph.Append(e.Channel, e.Encefalogram.Samples);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(graph.Zoom < 10)
            {
                graph.Zoom += 1;
            }
            else
            {
                graph.Zoom = 1;
            }
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            await _client.Pause();

        }
    }
}
