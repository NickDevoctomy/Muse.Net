using Harthoorn.MuseClient;
using Muse.Net.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Muse.LiveFeed
{
    public class SpeedGraph : Panel
    {
        private const float AMPLITUDE = 0x800;
        private const int PLOTHEIGHT = 100;
        private static readonly TimeSpan PLOTPERIOD = new TimeSpan(0, 0, 5);

        private struct PlotInfo
        {
            public Channel Channel;
            public int Offset;
            public Color Color;
        }

        private readonly MuseSamplerService _museSamplerService;
        private readonly FFTSamplerService _fftSamplerService;

        Bitmap bitmap;
        Timer timer;
        int updates = 0;

        public float Zoom { get; set; } = 1;

        private PlotInfo[] _rawPlots = new PlotInfo[]
        {
            new PlotInfo
            {
                Channel = Channel.EEG_AF7,
                Offset = 20,
                Color = Color.DodgerBlue
            },
            new PlotInfo
            {
                Channel = Channel.EEG_AF8,
                Offset = 140,
                Color = Color.Green
            },
            new PlotInfo
            {
                Channel = Channel.EEG_TP9,
                Offset = 260,
                Color = Color.Orange
            },
            new PlotInfo
            {
                Channel = Channel.EEG_TP10,
                Offset = 380,
                Color = Color.Yellow
            }
        };

        private PlotInfo[] _fftPlots = new PlotInfo[]
{
            new PlotInfo
            {
                Channel = Channel.EEG_AF7,
                Offset = 500,
                Color = Color.DodgerBlue
            },
            new PlotInfo
            {
                Channel = Channel.EEG_TP9,
                Offset = 500,
                Color = Color.Orange
            }
        };

        public SpeedGraph()
        {
            _museSamplerService = new MuseSamplerService(
                new MuseSamplerServiceConfiguration
                {
                    SamplePeriod = new TimeSpan(0, 0, 10)
                });
            _fftSamplerService = new FFTSamplerService();

            this.BackColor = Color.Green;
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
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.CompositingMode = CompositingMode.SourceCopy;

                // Plot raw channel data
                foreach(var curPlot in _rawPlots)
                {
                    if (_museSamplerService.TryGetSamples(
                        curPlot.Channel,
                        PLOTPERIOD,
                        out var data))
                    {
                        Draw(
                            graphics,
                            data,
                            curPlot.Color,
                            curPlot.Offset,
                            PLOTHEIGHT,
                            Zoom);
                    }
                }

                // Plot FFTs
                foreach(var curPlot in _fftPlots)
                {
                    if (_fftSamplerService.TryGetFFTSample(
                        _museSamplerService,
                        curPlot.Channel,
                        PLOTPERIOD,
                        out var data))
                    {
                        DrawFFT(
                            graphics,
                            data,
                            curPlot.Color,
                            curPlot.Offset,
                            PLOTHEIGHT);
                    }
                }

                graphics.Flush();
            }

            e.Graphics.DrawImage(bitmap, 1, 1);
            updates = 0;
        }

        public void Append(
            Channel channel,
            float[] values)
        {
            _museSamplerService.Sample(
                channel,
                values);
            updates += 1;
        }

        public void Draw(
            Graphics graphics,
            IList<float> data,
            Color color,
            int offset,
            int height,
            float zoom)
        {
            var axispen = new Pen(Color.Gray, 1);
            graphics.DrawLine(axispen, 10, offset, 10, offset + height);
            int ymax = height / 2;
            int y0 = offset + (int)ymax;
            graphics.DrawLine(axispen, 0, y0, this.Width, y0);

            float factor = zoom * (float)ymax / AMPLITUDE;
            int xa = 0, ya = 0;
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

        public void DrawFFT(
            Graphics graphics,
            IList<float> data,
            Color color,
            int offset,
            int height)
        {
            var axispen = new Pen(Color.Gray, 1);
            graphics.DrawLine(axispen, 10, offset, 10, offset + height);
            int y0 = offset + height;
            graphics.DrawLine(axispen, 0, offset + height, this.Width, offset + height);

            float factor = (float)height / AMPLITUDE;
            int x0 = 10;
            int xa = 0, ya = height;
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
