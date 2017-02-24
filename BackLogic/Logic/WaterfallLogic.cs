using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackLogic.Model;

namespace BackLogic.Logic
{
    public class WaterfallLogic : IDisposable
    {
        public static List<FreqDataViewModel> NormaliseWaterfallValues(List<FreqDataViewModel> amplAverage)
        {
            float Max = amplAverage.Max(p => p.Ampl);
            float Min = amplAverage.Min(m => m.Ampl);
            const float max = 1;
            const float min = 0;
            List<FreqDataViewModel> amplNormal = new List<FreqDataViewModel>();
            for (int i = 0; i < 1000; i++)
            {
                amplNormal.Add(new FreqDataViewModel { Ampl = (float)(min + ((amplAverage[i].Ampl - Min) * (max - min)) / (Max - Min)), Freq = amplAverage[i].Freq });
            }
            return amplNormal;
        }

        public List<FreqDataViewModel> ConvertWaterfallValues(List<FreqDataViewModel> Amplitudes)
        {

            float sum = 0;
            int Count = 0, den = 0;
            List<FreqDataViewModel> amplAverage = new List<FreqDataViewModel>();
            List<FreqDataViewModel> AmplNormal = new List<FreqDataViewModel>();
            decimal range = 0;
            decimal ratio;
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
                 ratio = range/1000;
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
            return AmplNormal = NormaliseWaterfallValues(amplAverage);
        }

        public void Dispose()
        {
          
        }
    }
}
