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
            public int XOffset;
            public int YOffset;
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
                XOffset = 10,
                YOffset = 20,
                Color = Color.DodgerBlue
            },
            new PlotInfo
            {
                Channel = Channel.EEG_AF8,
                XOffset = 10,
                YOffset = 140,
                Color = Color.Green
            },
            new PlotInfo
            {
                Channel = Channel.EEG_TP9,
                XOffset = 10,
                YOffset = 260,
                Color = Color.Orange
            },
            new PlotInfo
            {
                Channel = Channel.EEG_TP10,
                XOffset = 10,
                YOffset = 380,
                Color = Color.Yellow
            }
        };

        private PlotInfo[] _fftPlots = new PlotInfo[]
{
            new PlotInfo
            {
                Channel = Channel.EEG_AF7,
                XOffset = 10,
                YOffset = 500,
                Color = Color.DodgerBlue
            },
            new PlotInfo
            {
                Channel = Channel.EEG_TP9,
                XOffset = 10,
                YOffset = 500,
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
                            curPlot.XOffset,
                            curPlot.YOffset,
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
                            curPlot.XOffset,
                            curPlot.YOffset,
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
            int xOffset,
            int yOffset,
            int height,
            float zoom)
        {
            var axispen = new Pen(Color.Gray, 1);
            graphics.DrawLine(axispen, 10, yOffset, 10, yOffset + height);
            int ymax = height / 2;
            int y0 = yOffset + (int)ymax;
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
                    graphics.DrawLine(pen, xa + xOffset, ya, x + xOffset, y);
                }
                xa = x; ya = y;
            }   
        }

        public void DrawFFT(
            Graphics graphics,
            IList<float> data,
            Color color,
            int xOffset,
            int yOffset,
            int height)
        {
            var axispen = new Pen(Color.Gray, 1);
            graphics.DrawLine(axispen, 10, yOffset, 10, yOffset + height);
            int y0 = yOffset + height;
            graphics.DrawLine(axispen, 0, yOffset + height, this.Width, yOffset + height);

            float factor = (float)height / AMPLITUDE;
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
                    graphics.DrawLine(pen, xOffset + xa*2, ya, xOffset + x*2, y);
                }
                xa = x; ya = y;
            }
        }
    }

    
}
