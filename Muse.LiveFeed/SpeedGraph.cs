using Harthoorn.MuseClient;
using Muse.Net.Services;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
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
            public Pen Pen;
            public bool Hide;
            public bool ShowFFTRange;
        }

        private readonly MuseSamplerService _museSamplerService;
        private readonly FFTSamplerService _fftSamplerService;
        private readonly PlotterService _plotterService;
        private readonly BrainFrequencyAnalyser _bainFrequencyAnalyser;

        Bitmap _bitmapBuffer;
        Graphics _graphicsBuffer;

        public float Zoom { get; set; } = 1;

        private PlotInfo[] _rawPlots = new PlotInfo[]
        {
            new PlotInfo
            {
                Channel = Channel.EEG_AF7,
                XOffset = 10,
                YOffset = 20,
                Pen = new Pen(Color.DodgerBlue)
            },
            new PlotInfo
            {
                Channel = Channel.EEG_AF8,
                XOffset = 10,
                YOffset = 140,
                Pen = new Pen(Color.Green)
            },
            new PlotInfo
            {
                Channel = Channel.EEG_TP9,
                XOffset = 10,
                YOffset = 260,
                Pen = new Pen(Color.Orange)
            },
            new PlotInfo
            {
                Channel = Channel.EEG_TP10,
                XOffset = 10,
                YOffset = 380,
                Pen = new Pen(Color.Yellow)
            }
        };

        private PlotInfo[] _fftPlots = new PlotInfo[]
        {
            new PlotInfo
            {
                Channel = Channel.EEG_AF7,
                XOffset = 10,
                YOffset = 500,
                Pen = new Pen(Color.DodgerBlue),
                Hide = false,
                ShowFFTRange = true,
            },
            new PlotInfo
            {
                Channel = Channel.EEG_AF8,
                XOffset = 10,
                YOffset = 500,
                Pen = new Pen(Color.Green),
                Hide = true,
                ShowFFTRange = false
            },
            new PlotInfo
            {
                Channel = Channel.EEG_TP9,
                XOffset = 10,
                YOffset = 500,
                Pen = new Pen(Color.Orange),
                Hide = true,
                ShowFFTRange = false
            },
            new PlotInfo
            {
                Channel = Channel.EEG_TP10,
                XOffset = 10,
                YOffset = 500,
                Pen = new Pen(Color.Yellow),
                Hide = true,
                ShowFFTRange = false
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
            _plotterService = new PlotterService();
            _bainFrequencyAnalyser = new BrainFrequencyAnalyser();

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
                    _plotterService.DrawPlotAxis(
                        _graphicsBuffer,
                        Pens.Gray,
                        curPlot.YOffset,
                        Width,
                        PLOTHEIGHT,
                        true);
                    _plotterService.Plot(
                        _graphicsBuffer,
                        data,
                        curPlot.Pen,
                        curPlot.XOffset,
                        curPlot.YOffset,
                        PLOTHEIGHT,
                        AMPLITUDE,
                        Zoom);
                }
            }

            // Plot FFTs & Calculate dominant frequency ranges
            foreach (var curPlot in _fftPlots)
            {
                if (_fftSamplerService.TryGetFFTSample(
                    _museSamplerService,
                    curPlot.Channel,
                    PLOTPERIOD,
                    out var data))
                {
                    _plotterService.DrawPlotAxis(
                        _graphicsBuffer,
                        Pens.Gray,
                        curPlot.YOffset,
                        Width,
                        PLOTHEIGHT,
                        false);

                    if(!curPlot.Hide)
                    {
                        if (curPlot.ShowFFTRange)
                        {
                            var ranges = _bainFrequencyAnalyser.GetFrequencyRangeAverages(
                                data,
                                150,
                                62);
                            if (ranges != null)
                            {
                                _plotterService.HighlightDominantFrequencyRange(
                                    _graphicsBuffer,
                                    ranges,
                                    Brushes.Red,
                                    curPlot.XOffset,
                                    curPlot.YOffset,
                                    PLOTHEIGHT);
                            }

                        }

                        _plotterService.PlotFFT(
                            _graphicsBuffer,
                            data,
                            curPlot.Pen,
                            curPlot.XOffset,
                            curPlot.YOffset,
                            PLOTHEIGHT,
                            AMPLITUDE);
                    }
                }
            }

            e.Graphics.DrawImageUnscaled(
                _bitmapBuffer,
                0,
                0);
        }

        public void Append(
            Channel channel,
            float[] values)
        {
            _museSamplerService.Sample(
                channel,
                values);
            Invalidate();
        }
    }  
}
