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
    using SoluiNet.DevTools.Core.Tools.String;
    using SoluiNet.DevTools.Core.Tools.UI;
    using SoluiNet.DevTools.Core.UI;
    using SoluiNet.DevTools.Core.UI.General;
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
        /// The database context where time tracking will be stored.
        /// </summary>
        private TimeTrackingContext context;

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
            this.context = new TimeTrackingContext();

            var lowerDayLimit = DateTime.UtcNow.Date;
            var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

            if (this.AssignmentDate.SelectedDate.HasValue)
            {
                lowerDayLimit = this.AssignmentDate.SelectedDate.Value.Date;
                upperDayLimit = this.AssignmentDate.SelectedDate.Value.AddDays(1).Date;
            }

            this.PrepareSourceDataView(lowerDayLimit, upperDayLimit, this.context);

            this.PrepareAssignmentView(lowerDayLimit, upperDayLimit, this.context);

            this.PrepareStatisticsView(lowerDayLimit, upperDayLimit, this.context);
        }

        private void PrepareStatisticsView(DateTime lowerDayLimit, DateTime upperDayLimit, TimeTrackingContext context)
        {
            var weightedTimes = context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit).GroupBy(x => x.ApplicationIdentification).OrderBy(x => x.Key).Select(x => new { Weight = x.Sum(y => y.Duration), Target = x.Key });

            var barsChart = new CartesianChart();

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
        }

        private void DropOnApplicationElement(object dropApplicationSender, DragEventArgs dropApplicationEvents)
        {
            var dataObject = dropApplicationEvents.Data as DataObject;
            var data = dataObject.GetData(typeof(IGrouping<string, UsageTime>)) as IGrouping<string, UsageTime>;

            foreach (var usageTime in data)
            {
                usageTime.ApplicationId = ((dropApplicationSender as UI.AssignmentTarget).Tag as Entities.Application).ApplicationId;
            }
        }

        private void DropOnCategoryElement(object dropSender, DragEventArgs dropEvents)
        {
            var categories = this.context.Category;

            var dataObject = dropEvents.Data as DataObject;
            var data = dataObject.GetData(typeof(IGrouping<string, UsageTime>)) as IGrouping<string, UsageTime>;

            var distributionDictionary = new Dictionary<string, double>();
            var sumDuration = Convert.ToDouble(data.Sum(x => x.Duration));
            sumDuration -= data.Sum(x => x.CategoryUsageTime != null ? x.CategoryUsageTime.Sum(y => y.Duration) : 0);

            if (dropEvents.KeyStates.HasFlag(DragDropKeyStates.ShiftKey))
            {
                var assignableDuration = Prompt.ShowDialog(string.Format("Which time frame should be assigned? (max. {0})", sumDuration.ToDurationString()), "Select time frame");

                if (assignableDuration.GetSecondsFromDurationString() <= sumDuration)
                {
                    sumDuration = Convert.ToInt32(assignableDuration.GetSecondsFromDurationString());
                }
            }

            if ((dropSender as UI.AssignmentTargetExtended).Label.Equals("Distribute evenly"))
            {
                foreach (var category in categories)
                {
                    distributionDictionary.Add(category.CategoryName, Convert.ToDouble(sumDuration) / categories.Count());
                }
            }
            else
            {
                distributionDictionary.Add((dropSender as UI.AssignmentTargetExtended).Label, sumDuration);
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

                    var categoryToAssign = this.context.Category.Where(x => x.CategoryName == categoryDistribution.Key).FirstOrDefault();

                    if (categoryToAssign == null)
                    {
                        continue;
                    }

                    if (duration <= categoryDuration)
                    {
                        this.context.CategoryUsageTime.Add(new CategoryUsageTime()
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
                        this.context.CategoryUsageTime.Add(new CategoryUsageTime()
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

            this.context.SaveChanges();
        }

        private void PrepareAssignmentView(DateTime lowerDayLimit, DateTime upperDayLimit, TimeTrackingContext context, bool showOnlyUnassigned = false)
        {
            var timeTargets = context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit)
                .GroupBy(x => x.ApplicationIdentification);

            this.FillTimeTrackingOverview(timeTargets);

            var applications = context.Application;

            DragEventHandler dropApplicationDelegate = this.DropOnApplicationElement;

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

            DragEventHandler dropCategoryDelegate = this.DropOnCategoryElement;

            this.CategoryAssignmentGrid.CreateNewElement = () =>
            {
                var categoryName = Prompt.ShowDialog("Please provide an category name", "Category Assignment");

                if (!context.Category.Any(x => x.CategoryName == categoryName))
                {
                    var category = new Category() { CategoryName = categoryName };

                    context.Category.Add(category);
                    context.SaveChanges();

                    var newElement = new UI.AssignmentTargetExtended() { Label = categoryName };

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

            var distributeEvenlyElement = new UI.AssignmentTargetExtended()
            {
                AllowDrop = true,
                Label = "Distribute evenly",
            };

            distributeEvenlyElement.Drop += dropCategoryDelegate;

            this.CategoryAssignmentGrid.AddElement(distributeEvenlyElement);

            foreach (var category in categories)
            {
                var categoryTarget = new UI.AssignmentTargetExtended();

                categoryTarget.Label = category.CategoryName;

                categoryTarget.Tag = category;

                categoryTarget.AllowDrop = true;
                categoryTarget.Drop += dropCategoryDelegate;

                this.CategoryAssignmentGrid.AddElement(categoryTarget);
            }
        }

        private void PrepareSourceDataView(DateTime lowerDayLimit, DateTime upperDayLimit, TimeTrackingContext context)
        {
            context.UsageTime.Load();

            this.SourceData.AutoGenerateColumns = true;
            this.SourceData.ItemsSource = context.UsageTime.Local.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit);
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

        private void FillTimeTrackingOverview(IQueryable<IGrouping<string, UsageTime>> timeTargets)
        {
            this.TimeTrackingAssignmentOverview.Children.Clear();
            this.TimeTrackingAssignmentOverview.RowDefinitions.Clear();

            var highestDuration = timeTargets.Any() ? timeTargets.Max(x => x.Any() ? x.Sum(y => y != null ? y.Duration : 0) : 0) : 0;
            this.TimeTrackingAssignmentOverview.Tag = highestDuration;

            foreach (var timeTarget in timeTargets)
            {
                this.TimeTrackingAssignmentOverview.RowDefinitions.Add(new RowDefinition());

                var label = string.Format(
                    "{0} ({1})",
                    timeTarget.Key,
                    (Convert.ToDouble(timeTarget.Sum(x => x.Duration)) - timeTarget.Sum(x => x.CategoryUsageTime != null ? x.CategoryUsageTime.Sum(y => y.Duration) : 0)).ToDurationString());

                var timeTargetButton = new Button() { Content = label, HorizontalAlignment = HorizontalAlignment.Left };
                timeTargetButton.ToolTip = label;
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
        }

        private void TimeTrackingAssignmentTargetTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as TabControl).SelectedItem != null && ((sender as TabControl).SelectedItem as TabItem).Header.ToString() == "Application")
            {
                this.ShowAll.RemoveEvent("Click");
                this.ShowAll.Click += (showAllButton, eventArgs) =>
                {
                    var lowerDayLimit = DateTime.UtcNow.Date;
                    var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

                    if (this.AssignmentDate.SelectedDate.HasValue)
                    {
                        lowerDayLimit = this.AssignmentDate.SelectedDate.Value.Date;
                        upperDayLimit = this.AssignmentDate.SelectedDate.Value.AddDays(1).Date;
                    }

                    var timeTargets = this.context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit).GroupBy(x => x.ApplicationIdentification);

                    this.FillTimeTrackingOverview(timeTargets);
                };

                this.ShowOnlyUnassigned.RemoveEvent("Click");
                this.ShowOnlyUnassigned.Click += (showAllButton, eventArgs) =>
                {
                    var lowerDayLimit = DateTime.UtcNow.Date;
                    var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

                    if (this.AssignmentDate.SelectedDate.HasValue)
                    {
                        lowerDayLimit = this.AssignmentDate.SelectedDate.Value.Date;
                        upperDayLimit = this.AssignmentDate.SelectedDate.Value.AddDays(1).Date;
                    }

                    var timeTargets = this.context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit && x.ApplicationId == null).GroupBy(x => x.ApplicationIdentification);

                    this.FillTimeTrackingOverview(timeTargets);
                };
            }
        }

        private void FillQueryResults(List<UsageTime> queryResults)
        {
            this.QueryData.Items.Clear();

            if (this.QueryData.Columns.Count == 0)
            {
                this.QueryData.Columns.Add(new DataGridTextColumn() { Header = "ApplicationIdentification", Binding = new Binding("ApplicationIdentification") });
                this.QueryData.Columns.Add(new DataGridTextColumn() { Header = "StartTime", Binding = new Binding("StartTime") });
                this.QueryData.Columns.Add(new DataGridTextColumn() { Header = "EndTime", Binding = new Binding("EndTime") });
                this.QueryData.Columns.Add(new DataGridTextColumn() { Header = "Duration", Binding = new Binding("Duration") });
            }

            foreach (var item in queryResults)
            {
                this.QueryData.Items.Add(item);
            }
        }

        private void StartQuery_Click(object sender, RoutedEventArgs e)
        {
            var lowerDayLimit = DateTime.UtcNow.Date;
            var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

            if (this.QueryDateBegin.SelectedDate.HasValue && this.QueryDateEnd.SelectedDate.HasValue)
            {
                lowerDayLimit = this.QueryDateBegin.SelectedDate.Value.Date;
                upperDayLimit = this.QueryDateBegin.SelectedDate.Value.AddDays(1).Date;
            }

            var queryResults = this.context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit).ToList();

            this.FillQueryResults(queryResults);
        }
    }
}
