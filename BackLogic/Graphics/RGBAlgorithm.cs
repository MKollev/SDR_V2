using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackLogic.Graphics
{
    public class RGBAlgorithm : IDisposable
    {
        public void ColorFunction(double value, out int r, out int g, out int b)
        {

            double R = 0, G = 0, B = 0;

            if (value <= 0.25 && value >= 0)
            {
                R = 0;
                G = 2 * value;
                B = 1 - (4 / 3) * value;
            }
            else if (value <= 0.50 && value >= 0.25)
            {
                R = (4 / 3) * value - (1 / 3);
                G = 2 * value;
                B = 1 - (4 / 3) * value;
            }
            else if (value <= 0.750 && value >= 0.50)
            {
                R = (4 / 3) * value - (1 / 3);
                G = 2 - 2 * value;
                B = 1 - (4 / 3) * value;
            }
            else if (value <= 1.00 && value >= 0.75)
            {
                R = (4 / 3) * value - (1 / 3);
                G = 2 - 2 * value;
                B = 0;
            }

            r = (int)(R * 255);
            g = (int)(G * 255);
            b = (int)(B * 255);

        }

        public void Dispose()
        {
           
        }
    }
}
