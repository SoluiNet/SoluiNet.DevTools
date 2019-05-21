using LiveCharts;
using LiveCharts.Wpf;
using SoluiNet.DevTools.Core.Tools.Json;
using SoluiNet.DevTools.Core.Tools.Number;
using SoluiNet.DevTools.Core.UI;
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
        private bool _mouseMoving = false;

        public TimeTrackingToolsUserControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var context = new TimeTrackingContext();

            var lowerDayLimit = DateTime.UtcNow.Date;
            var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

            #region Fill Source Data
            context.UsageTime.Load();

            SourceData.AutoGenerateColumns = true;
            SourceData.ItemsSource = context.UsageTime.Local.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit);
            #endregion

            #region Prepare Assignment View
            var timeTargets = context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit)
                .GroupBy(x => x.ApplicationIdentification);

            var highestDuration = timeTargets.Any() ? timeTargets.Max(x => x.Any() ? x.Sum(y => y != null ? y.Duration : 0) : 0) : 0;

            foreach (var timeTarget in timeTargets)
            {
                TimeTrackingAssignmentOverview.RowDefinitions.Add(new RowDefinition());

                var timeTargetButton = new Button() { Content = timeTarget.Key, HorizontalAlignment = HorizontalAlignment.Left };
                timeTargetButton.ToolTip = timeTarget.Key;
                timeTargetButton.Tag = timeTarget;

                timeTargetButton.Width = Convert.ToDouble(timeTarget.Sum(x => x.Duration)) / highestDuration * this.ActualWidth;

                timeTargetButton.Background = ApplicationIdentificationTools.GetBackgroundAccent(timeTarget.Key.ExtractApplicationName());

                timeTargetButton.PreviewMouseMove += (dragSender, dragEvents) =>
                {
                    if (!_mouseMoving)
                    {
                        _mouseMoving = true;
                        if (dragEvents.LeftButton == MouseButtonState.Pressed)
                        {
                            var usageTimeObject = (dragSender as Button).Tag as IGrouping<string, UsageTime>;

                            Console.WriteLine("Dragged: " + JsonTools.Serialize(usageTimeObject));

                            var dataObject = new DataObject(typeof(IGrouping<string, UsageTime>), usageTimeObject);

                            DragDrop.DoDragDrop(dragSender as Button, dataObject, DragDropEffects.All);
                            dragEvents.Handled = true;
                        }
                        _mouseMoving = false;
                    }
                };

                TimeTrackingAssignmentOverview.Children.Add(timeTargetButton);

                Grid.SetRow(timeTargetButton, TimeTrackingAssignmentOverview.RowDefinitions.Count - 1);
            }

            var applications = context.Application;

            DragEventHandler dropApplicationDelegate = (dropApplicationSender, dropApplicationEvents) =>
            {
                var dataObject = dropApplicationEvents.Data as DataObject;
                var data = dataObject.GetData(typeof(IGrouping<string, UsageTime>)) as IGrouping<string, UsageTime>;

                foreach (var usageTime in data)
                {
                    usageTime.ApplicationId = ((dropApplicationSender as UI.AssignmentTarget).Tag as Entities.Application).ApplicationId;
                }
            };

            ApplicationAssignmentGrid.CreateNewElement = () =>
            {
                var applicationName = Prompt.ShowDialog("Please provide an application name", "Application Assignment");

                if (!context.Application.Any(x => x.ApplicationName == applicationName))
                {
                    var application = new Entities.Application() { ApplicationName = applicationName };

                    context.Application.Add(application);
                    context.SaveChanges();

                    var newElement = new UI.AssignmentTarget() { Label = applicationName };
                    newElement.Target.Background = ApplicationIdentificationTools.GetBackgroundAccent(applicationName);

                    newElement.Tag = application;

                    newElement.AllowDrop = true;
                    newElement.Drop += dropApplicationDelegate;

                    return newElement;
                }
                else
                {
                    MessageBox.Show("Overgiven application name already exists");

                    return null;
                }
            };

            foreach (var application in applications)
            {
                var applicationTarget = new UI.AssignmentTarget();

                applicationTarget.Label = application.ApplicationName;

                applicationTarget.Target.Background = ApplicationIdentificationTools.GetBackgroundAccent(application.ApplicationName);

                applicationTarget.Tag = application;

                applicationTarget.AllowDrop = true;
                applicationTarget.Drop += dropApplicationDelegate;

                ApplicationAssignmentGrid.AddElement(applicationTarget);
            }

            var categories = context.Category;

            DragEventHandler dropCategoryDelegate = (dropSender, dropEvents) =>
            {
                var dataObject = dropEvents.Data as DataObject;
                var data = dataObject.GetData(typeof(IGrouping<string, UsageTime>)) as IGrouping<string, UsageTime>;

                var distributionDictionary = new Dictionary<string, double>();
                var sumDuration = data.Sum(x => x.Duration);

                if ((dropSender as UI.AssignmentTarget).Label.Equals("Distribute evenly"))
                {
                    foreach (var category in categories)
                    {
                        distributionDictionary.Add(category.CategoryName, Convert.ToDouble(sumDuration) / categories.Count());
                    }
                }
                else
                {
                    distributionDictionary.Add((dropSender as UI.AssignmentTarget).Label, sumDuration);
                }

                foreach (var usageTime in data)
                {
                    foreach (var categoryDistribution in distributionDictionary)
                    {
                        if (categoryDistribution.Value == 0)
                        {
                            break;
                        }

                        if (usageTime.Duration <= categoryDistribution.Value)
                        {
                            context.CategoryUsageTime.Add(new CategoryUsageTime()
                            {
                                CategoryId = ((dropSender as UI.AssignmentTarget).Tag as Category).CategoryId,
                                UsageTimeId = usageTime.UsageTimeId,
                                Duration = usageTime.Duration
                            });

                            // categoryDistribution.Value -= usageTime.Duration;
                        }
                        else
                        {

                        }
                    }
                }
            };

        CategoryAssignmentGrid.CreateNewElement = () =>
            {
                var categoryName = Prompt.ShowDialog("Please provide an application name", "Application Assignment");

                if (!context.Category.Any(x => x.CategoryName == categoryName))
                {
                    var category = new Category() { CategoryName = categoryName };

        context.Category.Add(category);
                    context.SaveChanges();

                    var newElement = new UI.AssignmentTarget() { Label = categoryName };

        newElement.Tag = category;

                    newElement.AllowDrop = true;
                    newElement.Drop += dropCategoryDelegate;

                    return newElement;
                }
                else
                {
                    MessageBox.Show("Overgiven application name already exists");

                    return null;
                }
            };

            foreach (var category in categories)
            {
                var categoryTarget = new UI.AssignmentTarget();

categoryTarget.Label = category.CategoryName;

                categoryTarget.Tag = category;

                categoryTarget.AllowDrop = true;
                categoryTarget.Drop += dropCategoryDelegate;

                CategoryAssignmentGrid.AddElement(categoryTarget);
            }
            #endregion

            #region Show Statistics
            var weightedTimes = context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit).GroupBy(x => x.ApplicationIdentification).OrderBy(x => x.Key).Select(x => new { Weight = x.Sum(y => y.Duration), Target = x.Key });

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
            yAxis.Labels = weightedTimes.Max(x => x.Weight).CountFrom(start: 0).Select(x => x.ToString()).ToList();

barsChart.AxisX.Add(xAxis);
            barsChart.AxisY.Add(yAxis);

            TimeTrackingStatistics.Children.Add(barsChart);
            #endregion
        }
    }
}
