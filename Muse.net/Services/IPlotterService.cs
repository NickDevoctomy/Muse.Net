using System.Collections.Generic;
using System.Drawing;

namespace Muse.Net.Services
{
    public interface IPlotterService
    {
        void Plot(
            Graphics graphics,
            IList<float> data,
            Color color,
            int xOffset,
            int yOffset,
            int width,
            int height,
            float amplitude,
            float zoom);

        void PlotFFT(
            Graphics graphics,
            IList<float> data,
            Color color,
            int xOffset,
            int yOffset,
            int width,
            int height,
            float amplitude);
    }
}
