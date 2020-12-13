using Harthoorn.MuseClient;
using Muse.Net.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Muse.LiveFeed
{
    class SpeedGraph : Panel
    {
        private readonly MuseSamplerService _museSamplerService;

        Graphics graphics;
        Bitmap bitmap;
        Timer timer;
        int updates = 0;

        public float Zoom = 1;

        List<float> FFT_A = new List<float>();
        List<float> FFT_B = new List<float>();

        public SpeedGraph()
        {
            _museSamplerService = new MuseSamplerService(
                new MuseSamplerServiceConfiguration
                {
                    SamplePeriod = new TimeSpan(0, 0, 10)
                });

            //this.DoubleBuffered = true;
            this.BackColor = Color.Green;
            graphics = this.CreateGraphics();
            timer = new Timer();
            timer.Enabled = true;
            timer.Interval = 100;
            timer.Tick += Timer_Tick;

            this.Paint += SpeedGraph_Paint;
            
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (updates > 4) this.Invalidate();
            

        }

        private void SpeedGraph_Paint(object sender, PaintEventArgs e)
        {
            bitmap = new Bitmap(this.Width, this.Height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.CompositingMode = CompositingMode.SourceCopy;

            if(_museSamplerService.TryGetSamples(Channel.EEG_AF7, new TimeSpan(0, 0, 5), out var dataAF7))
            {
                Draw(graphics, dataAF7, Color.DodgerBlue, 20, 100, Zoom);
            }

            if (_museSamplerService.TryGetSamples(Channel.EEG_AF8, new TimeSpan(0, 0, 5), out var dataAF8))
            {
                Draw(graphics, dataAF8, Color.Green, 140, 100, Zoom);
            }

            if (_museSamplerService.TryGetSamples(Channel.EEG_TP9, new TimeSpan(0, 0, 5), out var dataTP9))
            {
                Draw(graphics, dataTP9, Color.Orange, 260, 100, Zoom);
            }

            if (_museSamplerService.TryGetSamples(Channel.EEG_TP10, new TimeSpan(0, 0, 5), out var dataTP10))
            {
                Draw(graphics, dataTP10, Color.Yellow, 380, 100, Zoom);
            }

            DrawFFT(graphics, FFT_A, Color.DodgerBlue, 500, 100, 1);
            DrawFFT(graphics, FFT_B, Color.Orange, 500, 100, 1);

            e.Graphics.DrawImage(bitmap, 1, 1);
            updates = 0;
        }

        int m = 0;
        public void Append(Channel channel, float[] values)
        {
            _museSamplerService.Sample(channel, values);
            updates += 1;

            m = (m + 1) % 30;
            if (m == 1)
            {
                if (_museSamplerService.TryGetSamples(Channel.EEG_AF7, new TimeSpan(0, 0, 5), out var dataAF7))
                {
                    var len = dataAF7.Length;
                    const int SIZE = 300;
                    var d = dataAF7.Skip(len - SIZE).Take(SIZE).ToArray();
                    FFT_A = Fourier.DFT(d).Magnitudes().ToList();
                }

                if (_museSamplerService.TryGetSamples(Channel.EEG_TP9, new TimeSpan(0, 0, 5), out var dataTP9))
                {
                    var len = dataTP9.Length;
                    const int SIZE = 300;
                    var d = dataTP9.Skip(len - SIZE).Take(SIZE).ToArray();
                    FFT_B = Fourier.DFT(d).Magnitudes().ToList();
                }
            }
        }

        public void LimitFromStart<T>(List<T> list, int limit)
        {
            int cut = Math.Max(0, list.Count - limit);
            if (cut > 0) list.RemoveRange(0, cut);
        }

        const float AMPLITUDE = 0x800; 

        public void Draw(Graphics graphics, IList<float> data, Color color, int offset, int height, float zoom)
        {
            var axispen = new Pen(Color.Gray, 1);
            graphics.DrawLine(axispen, 10, offset, 10, offset + height);

            int ymax = height / 2;
            int y0 = offset + (int)ymax;
            graphics.DrawLine(axispen, 0, y0, this.Width, y0);


            float factor = zoom * (float)ymax / AMPLITUDE;

            int xa = 0, ya = 0;
            int count = data.Count;

            Pen pen = new Pen(color);
            for (int x = 0; x < data.Count; x++)
            {
                float actual = data[x] - AMPLITUDE;
                int v = (int)(factor * actual);
                v = Math.Min(ymax, v); v = Math.Max(-ymax, v);
                int y = y0 - v;

                if (x > 0)
                {
                    graphics.DrawLine(pen, xa, ya, x, y);
                }
                xa = x; ya = y;
            }
           
        }

        public void DrawFFT(Graphics graphics, IList<float> data, Color color, int offset, int height, float zoom)
        {
            var axispen = new Pen(Color.Gray, 1);
            graphics.DrawLine(axispen, 10, offset, 10, offset + height);

            int y0 = offset + height;
            graphics.DrawLine(axispen, 0, offset + height, this.Width, offset + height);


            float factor = zoom * (float)height / AMPLITUDE;

            int x0 = 10;
            int xa = 0, ya = height;
            int count = data.Count;

            Pen pen = new Pen(color, 2);
            for (int x = 0; x < data.Count /2; x+= 3)
            {
                float actual = data[x] / 6;
                int v = (int)(factor * actual);
                v = Math.Min(height, v); //v = Math.Max(0, v);
                
                int y = y0 - v;

                if (x > 0)
                {
                    graphics.DrawLine(pen, x0 + xa*2, ya, x0 + x*2, y);
                }
                xa = x; ya = y;
            }

        }
    }

    
}
