using System;
using System.Collections.Generic;
using System.Drawing;

namespace Muse.Net.Services
{
    public class PlotterService : IPlotterService
    {
        public void Plot(
            Graphics graphics,
            IList<float> data,
            Color color,
            int xOffset,
            int yOffset,
            int width,
            int height,
            float amplitude,
            float zoom)
        {
            var axispen = new Pen(Color.Gray, 1);
            graphics.DrawLine(axispen, 10, yOffset, 10, yOffset + height);
            int ymax = height / 2;
            int y0 = yOffset + (int)ymax;
            graphics.DrawLine(axispen, 0, y0, width, y0);

            float factor = zoom * (float)ymax / amplitude;
            int xa = 0, ya = 0;
            Pen pen = new Pen(color);
            for (int x = 0; x < data.Count; x++)
            {
                float actual = data[x] - amplitude;
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

        public void PlotFFT(
            Graphics graphics,
            IList<float> data,
            Color color,
            int xOffset,
            int yOffset,
            int width,
            int height,
            float amplitude)
        {
            var axispen = new Pen(Color.Gray, 1);
            graphics.DrawLine(axispen, 10, yOffset, 10, yOffset + height);
            int y0 = yOffset + height;
            graphics.DrawLine(axispen, 0, yOffset + height, width, yOffset + height);

            float factor = (float)height / amplitude;
            int xa = 0, ya = height;
            Pen pen = new Pen(color, 2);
            for (int x = 0; x < data.Count / 2; x += 3)
            {
                float actual = data[x] / 6;
                int v = (int)(factor * actual);
                v = Math.Min(height, v); //v = Math.Max(0, v);

                int y = y0 - v;

                if (x > 0)
                {
                    graphics.DrawLine(pen, xOffset + xa * 2, ya, xOffset + x * 2, y);
                }
                xa = x; ya = y;
            }
        }
    }
}
