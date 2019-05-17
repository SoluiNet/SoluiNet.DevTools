using LiveCharts;
using LiveCharts.Wpf;
using SoluiNet.DevTools.Utils.TimeTracking.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoluiNet.DevTools.Utils.TimeTracking
{
    /// <summary>
    /// Interaktionslogik für TimeTrackingToolsUserControl.xaml
    /// </summary>
    public partial class TimeTrackingToolsUserControl : UserControl
    {
        public TimeTrackingToolsUserControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var context = new TimeTrackingContext();

            #region Fill Source Data
            context.UsageTime.Load();

            SourceData.AutoGenerateColumns = true;
            SourceData.ItemsSource = context.UsageTime.Local;
            #endregion

            #region Prepare Assignment View
            var lowerDayLimit = DateTime.UtcNow.Date;
            var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

            var timeTargets = context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit)
                .GroupBy(x => x.ApplicationIdentification);

            var highestDuration = timeTargets.Max(x => x.Sum(y => y.Duration));

            foreach (var timeTarget in timeTargets)
            {
                TimeTrackingAssignment.RowDefinitions.Add(new RowDefinition());

                var timeTargetButton = new Button() { Content = timeTarget.Key };

                timeTargetButton.Width = Convert.ToDouble(timeTarget.Sum(x => x.Duration)) / highestDuration * this.ActualWidth;

                timeTargetButton.Background = ApplicationIdentificationTools.GetBackgroundAccent(timeTarget.Key.ExtractApplicationName());

                TimeTrackingAssignment.Children.Add(timeTargetButton);

                Grid.SetRow(timeTargetButton, TimeTrackingAssignment.RowDefinitions.Count - 1);
            }
            #endregion

            #region Show Statistics
            var weightedTimes = context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit).GroupBy(x => x.ApplicationIdentification).Select(x => new { Weight = x.Sum(y => y.Duration), Target = x.Key });

            var barsChart = new CartesianChart();

            /*var targetBinding = new Binding("Target");
            targetBinding.Source = weightedTimes;

            var weightBinding = new Binding("Weight");
            weightBinding.Source = weightedTimes;

            var generalBinding = new Binding("weightedTimes");
            weightBinding.Source = weightedTimes;

            BindingOperations.SetBinding(barsChart, CartesianChart.SeriesProperty, generalBinding);*/

            barsChart.Name = "WeightedTargets";

            var seriesCollection = new SeriesCollection();

            var preparedDatabaseList = weightedTimes
                .Select(x => new { Label = x.Target, Weight = new ChartValues<int> { x.Weight } }).ToList();

            var columnSeriesList = preparedDatabaseList
                .Select(x => new ColumnSeries() { Title = x.Label?.Substring(0, x.Label.Length >= 30 ? 30 : x.Label.Length), Values = x.Weight }).ToList();

            var dataSource = columnSeriesList;
            seriesCollection.AddRange(dataSource);

            barsChart.Series = seriesCollection;

            var xAxis = new Axis();
            xAxis.Title = "Targets";
            xAxis.Labels = weightedTimes.Select(x => x.Target.Substring(0, 30)).ToList();

            var yAxis = new Axis();
            yAxis.Title = "Weights";
            yAxis.Labels = weightedTimes.Select(x => x.Weight).ToList().Select(x => x.ToString()).ToList();

            barsChart.AxisX.Add(xAxis);
            barsChart.AxisY.Add(yAxis);

            TimeTrackingStatistics.Children.Add(barsChart);
            #endregion
        }
    }
}
