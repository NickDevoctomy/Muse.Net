using Harthoorn.MuseClient;
using Muse.Net.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
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

        Bitmap _bitmapBuffer;
        Graphics _graphicsBuffer;

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
                    SamplePeriod = PLOTPERIOD
                });
            _fftSamplerService = new FFTSamplerService();
            
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if(_bitmapBuffer == null || (_bitmapBuffer.Width != Width || _bitmapBuffer.Height != Height))
            {
                if(_graphicsBuffer != null)
                {
                    _graphicsBuffer.Dispose();
                    _graphicsBuffer = null;
                }
                if(_bitmapBuffer != null)
                {
                    _bitmapBuffer.Dispose();
                    _bitmapBuffer = null;
                }

                _bitmapBuffer = new Bitmap(this.Width, this.Height);
                _graphicsBuffer = Graphics.FromImage(_bitmapBuffer);
                _graphicsBuffer.SmoothingMode = SmoothingMode.AntiAlias;
                _graphicsBuffer.CompositingQuality = CompositingQuality.HighSpeed;
                _graphicsBuffer.CompositingMode = CompositingMode.SourceCopy;
            }
            else
            {
                _graphicsBuffer.Clear(Color.Black);
            }

            // Plot raw channel data
            foreach (var curPlot in _rawPlots)
            {
                if (_museSamplerService.TryGetSamples(
                    curPlot.Channel,
                    PLOTPERIOD,
                    out var data))
                {
                    Draw(
                        _graphicsBuffer,
                        data,
                        curPlot.Color,
                        curPlot.XOffset,
                        curPlot.YOffset,
                        PLOTHEIGHT,
                        Zoom);
                }
            }

            // Plot FFTs
            foreach (var curPlot in _fftPlots)
            {
                if (_fftSamplerService.TryGetFFTSample(
                    _museSamplerService,
                    curPlot.Channel,
                    PLOTPERIOD,
                    out var data))
                {
                    DrawFFT(
                        _graphicsBuffer,
                        data,
                        curPlot.Color,
                        curPlot.XOffset,
                        curPlot.YOffset,
                        PLOTHEIGHT);
                }
            }

            e.Graphics.DrawImage(_bitmapBuffer, 1, 1);
            Interlocked.Exchange(ref updates, 0);

            //Invalidate();
        }

        public void Append(
            Channel channel,
            float[] values)
        {
            _museSamplerService.Sample(
                channel,
                values);
            Interlocked.Increment(ref updates);
            Invalidate();
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
