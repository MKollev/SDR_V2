using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BackLogic.Model;

namespace BackLogic.DataBase
{
    public class GetData : IDisposable
    {
        static DateTime? CurrentDateTime = null;

        public List<FreqDataViewModel> DataLoader()
        {
         
            using (var data = new airscanEntities())
            {
                if (GetData.CurrentDateTime == null)
                {
                    var firstOrDefault = data.measurings.FirstOrDefault();
                    if (firstOrDefault != null)
                    {
                        var firstDate = firstOrDefault.date_time;
                        CurrentDateTime = firstDate;
                        var lstValues = data.measurings.Where(s => s.date_time == firstDate).Select(s => new FreqDataViewModel { Ampl = s.pdbm, Freq = s.frequency }).ToList();

                        return lstValues;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {

                    try
                    {
                        var firstOrDefault = data.measurings.FirstOrDefault(s => s.date_time > CurrentDateTime);
                        if (firstOrDefault != null)
                        {
                            var nextdate = firstOrDefault.date_time;
                            CurrentDateTime = nextdate;
                            var lstValues = data.measurings.Where(s => s.date_time == nextdate).Select(s => new FreqDataViewModel { Ampl = s.pdbm, Freq = s.frequency }).ToList();
                            return lstValues;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("NO MORE DATA");
                        
                        return null;
                    }

                }

            }
        }

        public void Dispose()
        {

        }
    }
}
