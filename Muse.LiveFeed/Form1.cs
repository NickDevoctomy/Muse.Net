using Harthoorn.MuseClient;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Muse.LiveFeed
{
    public partial class Form1 : Form
    {
        MuseClient client = new MuseClient();

        public Form1()
        {
            InitializeComponent();
        }

        public void Report(string text)
        {
            lblStatus.Text = text;
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (client.Connected)
            {
                await client.Resume();
                Report("Running.");
            }
            else
            {
                await StartFeed(Report);

            }
        }

        private async void btnStop_Click(object sender, EventArgs e)
        {
            await client.Pause();
            Report("Paused.");
        }

        public async Task StartFeed(Action<string> report)
        {
            
            report("Connecting...");
            var ok = await client.Connect();
            if (ok)
            {
                await client.Subscribe(
                    Channel.EEG_AF7,
                    Channel.EEG_AF8,
                    Channel.EEG_TP10,
                    Channel.EEG_TP9,
                    Channel.EEG_AUX
                    );

                client.NotifyEeg += Client_NotifyEeg;
                report("Starting...");
                await client.Resume();
                report("Running.");
                btnStart.Text = "Start";
            }
        }

        private void Client_NotifyEeg(Channel channel, Encefalogram gram)
        {
            graph.Append(channel, gram.Samples);
            
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
            await client.Pause();

        }
    }
}
