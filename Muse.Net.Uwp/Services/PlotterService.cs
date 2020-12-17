namespace Muse.Net.Services
{
    //public class PlotterService : IPlotterService
    //{
    //    public void DrawPlotAxis(
    //        Graphics graphics,
    //        Pen pen,
    //        int yOffset,
    //        int width,
    //        int height,
    //        bool centreLine)
    //    {
    //        graphics.DrawLine(
    //            pen,
    //            10,
    //            yOffset,
    //            10,
    //            yOffset + height);
    //        if(centreLine)
    //        {
    //            int ymax = height / 2;
    //            int y0 = yOffset + (int)ymax;
    //            graphics.DrawLine(
    //                pen,
    //                0,
    //                y0,
    //                width,
    //                y0);
    //        }

    //        graphics.DrawLine(
    //            pen,
    //            10,
    //            yOffset + height,
    //            width,
    //            yOffset + height);
    //    }

    //    public void Plot(
    //        Graphics graphics,
    //        IList<float> data,
    //        Pen pen,
    //        int xOffset,
    //        int yOffset,
    //        int height,
    //        float amplitude,
    //        float zoom)
    //    {
    //        int ymax = height / 2;
    //        int y0 = yOffset + (int)ymax;
    //        float factor = zoom * (float)ymax / amplitude;
    //        int prevX = 0;
    //        int prevY = 0;
    //        for (int x = 0; x < data.Count; x++)
    //        {
    //            float actual = data[x] - amplitude;
    //            int v = (int)(factor * actual);
    //            v = Math.Min(ymax, v); v = Math.Max(-ymax, v);
    //            int y = y0 - v;

    //            if (x > 0)
    //            {
    //                graphics.DrawLine(
    //                    pen,
    //                    prevX + xOffset,
    //                    prevY,
    //                    x + xOffset,
    //                    y);
    //            }
    //            prevX = x; prevY = y;
    //        }
    //    }

    //    public void PlotFFT(
    //        Graphics graphics,
    //        IList<float> data,
    //        Pen pen,
    //        int xOffset,
    //        int yOffset,
    //        int height,
    //        float amplitude)
    //    {
    //        int y0 = yOffset + height;
    //        float factor = (float)height / amplitude;
    //        int prevX = 0;
    //        int prevY = height;
    //        for (int x = 0; x < data.Count / 2; x++)
    //        {
    //            float actual = data[x] / 6;
    //            int v = (int)(factor * actual);
    //            v = Math.Min(height, v);
    //            int y = y0 - v;
    //            if (x > 0)
    //            {
    //                graphics.DrawLine(
    //                    pen,
    //                    xOffset + prevX * 2,
    //                    prevY,
    //                    xOffset + x * 2,
    //                    y);
    //            }

    //            prevX = x; prevY = y;
    //        }
    //    }
    //}
}
