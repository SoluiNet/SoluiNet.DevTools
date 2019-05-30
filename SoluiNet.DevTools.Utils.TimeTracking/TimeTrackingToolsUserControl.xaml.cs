// <copyright file="TimeTrackingToolsUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking
{
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
    using LiveCharts;
    using LiveCharts.Wpf;
    using SoluiNet.DevTools.Core.Tools.Json;
    using SoluiNet.DevTools.Core.Tools.Number;
    using SoluiNet.DevTools.Core.UI;
    using SoluiNet.DevTools.Utils.TimeTracking.Entities;

    /// <summary>
    /// Interaction logic for TimeTrackingToolsUserControl.xaml.
    /// </summary>
    public partial class TimeTrackingToolsUserControl : UserControl
    {
        /// <summary>
        /// A value which indicates if a mouse is moving or not.
        /// </summary>
        private bool mouseMoving = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingToolsUserControl"/> class.
        /// </summary>
        public TimeTrackingToolsUserControl()
        {
            this.InitializeComponent();

            this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            this.Arrange(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var context = new TimeTrackingContext();

            var lowerDayLimit = DateTime.UtcNow.Date;
            var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

            #region Fill Source Data
            context.UsageTime.Load();

            this.SourceData.AutoGenerateColumns = true;
            this.SourceData.ItemsSource = context.UsageTime.Local.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit);
            #endregion

            #region Prepare Assignment View
            var timeTargets = context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit)
                .GroupBy(x => x.ApplicationIdentification);

            var highestDuration = timeTargets.Any() ? timeTargets.Max(x => x.Any() ? x.Sum(y => y != null ? y.Duration : 0) : 0) : 0;
            this.TimeTrackingAssignmentOverview.Tag = highestDuration;

            foreach (var timeTarget in timeTargets)
            {
                this.TimeTrackingAssignmentOverview.RowDefinitions.Add(new RowDefinition());

                var timeTargetButton = new Button() { Content = string.Format("{0} ({1})", timeTarget.Key, timeTarget.Sum(x => x.Duration).ToDurationString()), HorizontalAlignment = HorizontalAlignment.Left };
                timeTargetButton.ToolTip = timeTarget.Key;
                timeTargetButton.Tag = timeTarget;

                if (highestDuration > 0)
                {
                    timeTargetButton.Width = Convert.ToDouble(timeTarget.Sum(x => x.Duration)) / highestDuration * this.TimeTrackingAssignmentOverview.ActualWidth;
                }

                timeTargetButton.Background = ApplicationIdentificationTools.GetBackgroundAccent(timeTarget.Key.ExtractApplicationName());

                timeTargetButton.PreviewMouseMove += (dragSender, dragEvents) =>
                {
                    if (!this.mouseMoving)
                    {
                        this.mouseMoving = true;
                        if (dragEvents.LeftButton == MouseButtonState.Pressed)
                        {
                            var usageTimeObject = (dragSender as Button).Tag as IGrouping<string, UsageTime>;

                            Console.WriteLine("Dragged: " + JsonTools.Serialize(usageTimeObject));

                            var dataObject = new DataObject(typeof(IGrouping<string, UsageTime>), usageTimeObject);

                            DragDrop.DoDragDrop(dragSender as Button, dataObject, DragDropEffects.All);
                            dragEvents.Handled = true;
                        }

                        this.mouseMoving = false;
                    }
                };

                this.TimeTrackingAssignmentOverview.Children.Add(timeTargetButton);

                Grid.SetRow(timeTargetButton, this.TimeTrackingAssignmentOverview.RowDefinitions.Count - 1);
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

            this.ApplicationAssignmentGrid.CreateNewElement = () =>
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

                this.ApplicationAssignmentGrid.AddElement(applicationTarget);
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

                var workingDistributionDictionary = new Dictionary<string, double>(distributionDictionary);

                foreach (var usageTime in data)
                {
                    var duration = Convert.ToDouble(usageTime.Duration);

                    foreach (var categoryDistribution in distributionDictionary)
                    {
                        var categoryDuration = workingDistributionDictionary[categoryDistribution.Key];

                        if (categoryDuration <= 0)
                        {
                            continue;
                        }

                        var categoryToAssign = context.Category.Where(x => x.CategoryName == categoryDistribution.Key).FirstOrDefault();

                        if (categoryToAssign == null)
                        {
                            continue;
                        }

                        if (duration <= categoryDuration)
                        {
                            context.CategoryUsageTime.Add(new CategoryUsageTime()
                            {
                                CategoryId = categoryToAssign.CategoryId,
                                UsageTimeId = usageTime.UsageTimeId,
                                Duration = duration,
                            });

                            // categoryDistribution.Value -= usageTime.Duration;
                            workingDistributionDictionary[categoryDistribution.Key] -= duration;

                            break;
                        }
                        else
                        {
                            context.CategoryUsageTime.Add(new CategoryUsageTime()
                            {
                                CategoryId = categoryToAssign.CategoryId,
                                UsageTimeId = usageTime.UsageTimeId,
                                Duration = categoryDuration,
                            });

                            // categoryDistribution.Value -= usageTime.Duration;
                            duration -= categoryDuration;
                            workingDistributionDictionary[categoryDistribution.Key] -= categoryDuration;
                        }
                    }
                }

                context.SaveChanges();
            };

            this.CategoryAssignmentGrid.CreateNewElement = () =>
                {
                    var categoryName = Prompt.ShowDialog("Please provide an category name", "Category Assignment");

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
                        MessageBox.Show("Overgiven category name already exists");

                        return null;
                    }
                };

            var distributeEvenlyElement = new UI.AssignmentTarget()
            {
                AllowDrop = true,
                Label = "Distribute evenly",
            };

            distributeEvenlyElement.Drop += dropCategoryDelegate;

            this.CategoryAssignmentGrid.AddElement(distributeEvenlyElement);

            foreach (var category in categories)
            {
                var categoryTarget = new UI.AssignmentTarget();

                categoryTarget.Label = category.CategoryName;

                categoryTarget.Tag = category;

                categoryTarget.AllowDrop = true;
                categoryTarget.Drop += dropCategoryDelegate;

                this.CategoryAssignmentGrid.AddElement(categoryTarget);
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

            this.TimeTrackingStatistics.Children.Add(barsChart);
            #endregion
        }

        private void TimeTrackingTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as TabControl).SelectedIndex != 1)
            {
                return;
            }

            var highestDuration = Convert.ToDouble(this.TimeTrackingAssignmentOverview.Tag);

            foreach (var element in this.TimeTrackingAssignmentOverview.Children)
            {
                if (!(element is Button))
                {
                    continue;
                }

                var timeTarget = (element as Button).Tag as IGrouping<string, UsageTime>;

                if (highestDuration > 0)
                {
                    (element as Button).Width = Convert.ToDouble(timeTarget.Sum(x => x.Duration)) / highestDuration * this.TimeTrackingAssignmentOverview.ActualWidth;
                }
            }
        }
    }
}
