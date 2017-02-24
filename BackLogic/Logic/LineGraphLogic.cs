using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BackLogic.Model;

namespace BackLogic.Logic
{
    public class LineGraphLogic : IDisposable
    {
        public List<FreqDataViewModel> AverageLineValues(List<FreqDataViewModel> Amplitudes)
        {

            float sum = 0;
            int Count = 0, den = 0;
            List<FreqDataViewModel> amplAverage = new List<FreqDataViewModel>();
            decimal range = 0;
            decimal ratio;
            if (Amplitudes == null)
            {
                MessageBox.Show("NO MORE DATA");
                return null;
            }
            var first = Amplitudes.FirstOrDefault();
            var last = Amplitudes.LastOrDefault();
            if (first != null && last != null)
            {
                range = last.Freq - first.Freq;
            }
            else
            {
                return null;
            }
            if (range != 0)
            {
                ratio = range / 1000;
            }
            else
            {
                ratio = 1;
            }

            for (decimal s = first.Freq; s < last.Freq; s += ratio)
            {
                for (int i = Count; i < Amplitudes.Count && Amplitudes[i].Freq <= (s + ratio); Count++, i++)
                {
                    sum += Amplitudes[i].Ampl;
                    den++;
                }
                if (sum == 0)
                {
                    sum = amplAverage[amplAverage.Count - 1].Ampl;
                    den = 1;
                }
                amplAverage.Add(new FreqDataViewModel { Ampl = sum / den, Freq = s });
                sum = 0;
                den = 0;
            }
            return amplAverage;
        }


        public void Dispose()
        {
           
        }
    }
}
