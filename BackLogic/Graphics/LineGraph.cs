using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using BackLogic.DataBase;
using BackLogic.Logic;
using BackLogic.Model;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.NewLine;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace BackLogic.Graphics
{
    public class LineDraw : IDisposable
    {
        private DispatcherTimer lineTimer;
       
        private List<FreqDataViewModel> _currentData;
        private int DB { get; set; }
        private int Socket { get; set; }
        private static SocketDataViewModel SocketServerData { get; set; }
        private List<FreqDataViewModel> allPoints { get; set; }
        private Image Waterfall { get; set; }
        private List<FreqDataViewModel> AverageLinePoints { get; set; }
        private ChartPlotter _Plotter { get; set; }
        private static int Max { get; set; }
        private static int Min { get; set; }

        public LineDraw(int db, int sck, Image waterfall, ChartPlotter plot, SocketDataViewModel socketData)
        {
            DB = db;
            Socket = sck;
            Waterfall = waterfall;
            _Plotter = plot;
            SocketServerData = socketData;
        }
        public void SetTimer()
        {
            lineTimer = new DispatcherTimer();
            lineTimer.Tick += LineTimer_Tick;
            lineTimer.Interval = new TimeSpan(0, 0, 0, 0, 0);
        }

        public void CreateThread()
        {
            lineTimer.Start();
        }

        private void GetCurrentData()
        {
            int i = 0;
            _currentData = new List<FreqDataViewModel>();
            if (Min == 0 && Max == 0)
            {
                _currentData = allPoints;
            }
            else
            {
                while (i < allPoints.Count)
                {

                    if (allPoints[i].Freq > Min && allPoints[i].Freq < Max)
                    {
                        _currentData.Add(
                            new FreqDataViewModel
                            {
                                Ampl = allPoints[i].Ampl,
                                Freq = allPoints[i].Freq
                            });
                    }
                    i++;
                }
            }
        }
        public async void GetAllPoints()
        {
            if (DB == 1)
            {
                using (var db = new GetData())
                {
                    this.allPoints = db.DataLoader();
                    if (allPoints == null)
                    {
                        MessageBox.Show("NO MORE DATA");
                        lineTimer.Stop();
                        return;
                    }
                }
                GetCurrentData();
                using (var temp = new LineGraphLogic())
                {
                    AverageLinePoints = temp.AverageLineValues(_currentData);
                }
             
            }

            else if (Socket == 1)
            {
                using (var socket = new AsynchronousClient())
                {
                    this.allPoints = await socket.StartClient(SocketServerData);
                    Dispose();
                }
                GetCurrentData();
                using (var avg = new LineGraphLogic())
                {
                    AverageLinePoints = avg.AverageLineValues(_currentData);
                    if (AverageLinePoints == null)
                    {
                        lineTimer.Stop();
                    }
                }
               
            }
            //allPoints.Clear();
            //_currentData.Clear();
        }
        public void ClearLines()
        {
            var lgc = new Collection<IPlotterElement>();
            foreach (var x in _Plotter.Children)
            {
                if (x is LineGraph || x is ElementMarkerPointsGraph)
                    lgc.Add(x);
            }

            foreach (var x in lgc)
            {
                _Plotter.Children.Remove(x);
            }
        }
        private void LineTimer_Tick(object sender, EventArgs e)
        {
            GetAllPoints();
            ClearLines();
            IPointDataSource point = null;
            EnumerableDataSource<FreqDataViewModel> data;
            data = new EnumerableDataSource<FreqDataViewModel>(AverageLinePoints);
            data.SetXMapping(c => (double)c.Freq);
            data.SetYMapping(c => c.Ampl);
            point = data;
            
            LineGraph lineGR = new LineGraph(point);
            _Plotter.Children.Add(lineGR);


            using (var wtrfl = new WaterfallGraph(_currentData))
            {
                Waterfall.Source = wtrfl.NewWaterfallLine();
            }
            allPoints.Clear();
            _currentData.Clear();
        }



        public void Mapping(int min, int max)
        {
            Min = min;
            Max = max;
           GetAllPoints();
            var dsS = new EnumerableDataSource<FreqDataViewModel>(AverageLinePoints);
            dsS.SetXMapping(ci => (double)ci.Freq);
            dsS.SetYMapping(y => y.Ampl);

            // plotter.AddLineChart(dsS).with;

            _Plotter.FitToView();
        }

        public void Dispose()
        {

        }
    }
}
