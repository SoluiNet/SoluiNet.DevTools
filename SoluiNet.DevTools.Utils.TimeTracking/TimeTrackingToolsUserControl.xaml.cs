// <copyright file="TimeTrackingToolsUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.TimeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Reflection;
    using System.Resources;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using LiveCharts;
    using LiveCharts.Wpf;
    using NLog;
    using SoluiNet.DevTools.Core.Constants;
    using SoluiNet.DevTools.Core.Tools.Dictionary;
    using SoluiNet.DevTools.Core.Tools.Json;
    using SoluiNet.DevTools.Core.Tools.Number;
    using SoluiNet.DevTools.Core.Tools.String;
    using SoluiNet.DevTools.Core.Tools.XML;
    using SoluiNet.DevTools.Core.UI.WPF.General;
    using SoluiNet.DevTools.Core.UI.WPF.Tools.UI;
    using SoluiNet.DevTools.Core.UI.WPF.UIElement;
    using SoluiNet.DevTools.Core.UI.WPF.Window;
    using SoluiNet.DevTools.Core.UI.WPF.XmlData;
    using SoluiNet.DevTools.Core.XmlData;
    using SoluiNet.DevTools.Utils.TimeTracking.Entities;
    using SoluiNet.DevTools.Utils.TimeTracking.UI;

    /// <summary>
    /// Interaction logic for TimeTrackingToolsUserControl.xaml.
    /// </summary>
    public partial class TimeTrackingToolsUserControl : UserControl, IDisposable
    {
        /// <summary>
        /// A value which indicates if a mouse is moving or not.
        /// </summary>
        private bool mouseMoving;

        /// <summary>
        /// A value which indicates if the overview has already been loaded.
        /// </summary>
        private bool overviewLoaded;

        /// <summary>
        /// The database context where time tracking will be stored.
        /// </summary>
        private TimeTrackingContext context;

        /// <summary>
        /// A value which indicates if this instance has been disposed already.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeTrackingToolsUserControl"/> class.
        /// </summary>
        public TimeTrackingToolsUserControl()
        {
            this.InitializeComponent();

            this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            this.Arrange(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        private static Logger Logger
        {
            get
            {
                return LogManager.GetCurrentClassLogger();
            }
        }

        private static new ResourceManager Resources
        {
            get
            {
                return new ResourceManager("SoluiNet.DevTools.Utils.TimeTracking.Properties.Resources", Assembly.GetExecutingAssembly());
            }
        }

        /// <summary>
        /// Dispose time tracking tool user control.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose time tracking tool user control.
        /// </summary>
        /// <param name="disposing">A value which indicates if managed objects should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.context?.Dispose();
            }

            this.disposed = true;
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

            this.PrepareQueryView(this.context);
        }

        private void PrepareQueryView(TimeTrackingContext localContext)
        {
            this.QueryFilter.Items.Clear();

            foreach (var filterHistoryItem in localContext.FilterHistory)
            {
                this.QueryFilter.Items.Add(filterHistoryItem.FilterString);
            }
        }

        private void PrepareStatisticsView(DateTime lowerDayLimit, DateTime upperDayLimit, TimeTrackingContext localContext)
        {
            var statisticTabs = new TabControl
            {
                TabStripPlacement = Dock.Bottom,
            };

            #region Weighted Targets
            var weightedTimes = localContext.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit).GroupBy(x => x.ApplicationIdentification).OrderBy(x => x.Key).Select(x => new { Weight = x.Sum(y => y.Duration), Target = x.Key });

            var barsChart = new CartesianChart
            {
                Name = "WeightedTargets",
            };

            var seriesCollection = new SeriesCollection();

            var preparedDatabaseList = weightedTimes
                .Select(x => new { Label = x.Target, Weight = new ChartValues<int> { x.Weight } }).ToList();

            var columnSeriesList = preparedDatabaseList
                .Select(x => new ColumnSeries() { Title = x.Label?.Substring(0, x.Label.Length >= 30 ? 30 : x.Label.Length), Values = x.Weight }).ToList();

            var dataSource = columnSeriesList;
            seriesCollection.AddRange(dataSource);

            barsChart.Series = seriesCollection;

            var xAxis = new Axis
            {
                Title = "Targets",
                Labels = weightedTimes.Select(x => x.Target.Substring(0, 30)).ToList(),
            };

            var yAxis = new Axis
            {
                Title = "Weights",
                Labels = weightedTimes.Max(x => x.Weight).CountFrom(start: 0).Select(x => x.ToString(CultureInfo.InvariantCulture)).ToList(),
            };

            barsChart.AxisX.Add(xAxis);
            barsChart.AxisY.Add(yAxis);

            statisticTabs.Items.Add(new TabItem() { Header = "Targets", Content = barsChart });
            #endregion

            #region Application
            var durationPerApplication = localContext.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit).GroupBy(x => x.Application.ApplicationName).OrderBy(x => x.Key).Select(x => new { Duration = x.Sum(y => y.Duration), Application = x.Key });

            var applicationChart = new CartesianChart
            {
                Name = "DurationPerApplication",
            };

            var applicationSeries = new SeriesCollection();

            var applicationChartValueList = durationPerApplication
                .Select(x => new { Label = x.Application, Weight = new ChartValues<int> { x.Duration } }).ToList();

            var applicationColumnSeriesList = applicationChartValueList
                .Select(x => new ColumnSeries() { Title = x.Label?.Substring(0, x.Label.Length >= 30 ? 30 : x.Label.Length), Values = x.Weight }).ToList();

            var applicationDataSource = applicationColumnSeriesList;
            applicationSeries.AddRange(applicationDataSource);

            applicationChart.Series = applicationSeries;

            var applications = new Axis
            {
                Title = "Applications",
                Labels = durationPerApplication.Select(x => x.Application.Substring(0, 30)).ToList(),
            };

            var durations = new Axis();
            yAxis.Title = "Durations";
            yAxis.Labels = durationPerApplication.Max(x => x.Duration).CountFrom(start: 0).Select(x => x.ToString(CultureInfo.InvariantCulture)).ToList();

            applicationChart.AxisX.Add(applications);
            applicationChart.AxisY.Add(durations);

            statisticTabs.Items.Add(new TabItem() { Header = "Applications", Content = applicationChart });
            #endregion

            this.TimeTrackingStatistics.Children.Add(statisticTabs);
        }

        private void DropOnApplicationElement(object dropApplicationSender, DragEventArgs dropApplicationEvents)
        {
            var dataObject = dropApplicationEvents.Data as DataObject;
            var data = dataObject.GetData(typeof(IGrouping<string, UsageTime>)) as IGrouping<string, UsageTime>;

            foreach (var usageTime in data)
            {
                usageTime.ApplicationId = ((dropApplicationSender as UI.AssignmentTarget).Tag as Entities.Application).ApplicationId;
            }

            foreach (var item in this.TimeTrackingAssignmentOverview.Children.OfType<ExtendedButton>().Where(x => x.Selected))
            {
                var applicationData = item.Tag as IGrouping<string, UsageTime>;

                foreach (var applicationUsageTime in applicationData)
                {
                    applicationUsageTime.ApplicationId = ((dropApplicationSender as UI.AssignmentTarget).Tag as Entities.Application).ApplicationId;
                }
            }

            this.context.SaveChanges();
        }

        private void DropOnApplicationAreaElement(object dropApplicationSender, DragEventArgs dropApplicationEvents)
        {
            var dataObject = dropApplicationEvents.Data as DataObject;

            if (!(dataObject?.GetData(typeof(IGrouping<string, UsageTime>)) is IGrouping<string, UsageTime> data))
            {
                return;
            }

            foreach (var usageTime in data)
            {
                usageTime.ApplicationAreaId = ((dropApplicationSender as UI.AssignmentTarget)?.Tag as Entities.ApplicationArea)?.ApplicationAreaId;
            }

            foreach (var item in this.TimeTrackingAssignmentOverview.Children.OfType<ExtendedButton>().Where(x => x.Selected))
            {
                if (!(item.Tag is IGrouping<string, UsageTime> applicationAreaData))
                {
                    continue;
                }

                foreach (var applicationUsageTime in applicationAreaData)
                {
                    applicationUsageTime.ApplicationAreaId = ((dropApplicationSender as UI.AssignmentTarget)?.Tag as Entities.ApplicationArea)?.ApplicationAreaId;
                }
            }

            this.context.SaveChanges();
        }

        private void DropOnCategoryElement(object dropSender, DragEventArgs dropEvents)
        {
            var distributedEvenly = (dropSender as UI.AssignmentTargetExtended).Label
                .Equals("Distribute evenly", StringComparison.Ordinal);

            var categories = this.context.Category;

            var dataObject = dropEvents.Data as DataObject;
            var data = dataObject?.GetData(typeof(IGrouping<string, UsageTime>)) as IGrouping<string, UsageTime>;

            var usageTimeList = new HashSet<IGrouping<string, UsageTime>> { data };

            foreach (var item in this.TimeTrackingAssignmentOverview.Children.OfType<ExtendedButton>().Where(x => x.Selected))
            {
                var categoryData = item.Tag as IGrouping<string, UsageTime>;

                usageTimeList.Add(categoryData);
            }

            var distributionDictionary = new Dictionary<string, double>();
            var sumDuration = Convert.ToDouble(usageTimeList.Sum(x => x.Sum(y => y.Duration)));
            sumDuration -= usageTimeList.Sum(x => x.Sum(y => y.CategoryUsageTime?.Sum(z => z.Duration) ?? 0));

            if (dropEvents.KeyStates.HasFlag(DragDropKeyStates.ShiftKey))
            {
                var assignableDuration = Prompt.ShowDialog(
                    $"Which time frame should be assigned? (max. {sumDuration.ToDurationString()})", Resources.GetString("SelectTimeFrame", CultureInfo.CurrentCulture));

                if (assignableDuration.GetSecondsFromDurationString() <= sumDuration)
                {
                    sumDuration = Convert.ToInt32(assignableDuration.GetSecondsFromDurationString());
                }
            }

            if (distributedEvenly)
            {
                var assignableCategories = categories.ToList()
                    .Where(x => x.DistributeEvenlyTarget.GetValueOrDefault(false));

                foreach (var category in assignableCategories)
                {
                    distributionDictionary.Add(
                        category.CategoryName,
                        Convert.ToDouble(sumDuration) / assignableCategories.Count());
                }
            }
            else
            {
                distributionDictionary.Add((dropSender as UI.AssignmentTargetExtended).Label, sumDuration);
            }

            var workingDistributionDictionary = new Dictionary<string, double>(distributionDictionary);

            foreach (var usageData in usageTimeList)
            {
                foreach (var usageTime in usageData)
                {
                    var duration = Convert.ToDouble(usageTime.Duration);

                    foreach (var categoryDistribution in distributionDictionary)
                    {
                        var categoryDuration = workingDistributionDictionary[categoryDistribution.Key];

                        if (categoryDuration <= 0)
                        {
                            continue;
                        }

                        var categoryToAssign = this.context.Category.FirstOrDefault(x => x.CategoryName == categoryDistribution.Key);

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
                                DistributedEvenly = distributedEvenly,
                            });

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
                                DistributedEvenly = distributedEvenly,
                            });

                            duration -= categoryDuration;
                            workingDistributionDictionary[categoryDistribution.Key] -= categoryDuration;
                        }

                        // usageTime.CategoryAutomaticAssigned = true;
                    }
                }
            }

            this.context.SaveChanges();
        }

        private void RightClickApplication(object sender, MouseButtonEventArgs eventArgs)
        {
            if (!(this.FindResource("ApplicationContextMenu") is ContextMenu applicationContextMenu))
            {
                return;
            }

            applicationContextMenu.PlacementTarget = sender as UI.AssignmentTarget;
            applicationContextMenu.IsOpen = true;
        }

        private void RightClickApplicationArea(object sender, MouseButtonEventArgs eventArgs)
        {
            if (!(this.FindResource("ApplicationAreaContextMenu") is ContextMenu applicationAreaContextMenu))
            {
                return;
            }

            applicationAreaContextMenu.PlacementTarget = sender as UI.AssignmentTarget;
            applicationAreaContextMenu.IsOpen = true;
        }

        private void RightClickCategory(object sender, MouseButtonEventArgs eventArgs)
        {
            if (!(this.FindResource("CategoryContextMenu") is ContextMenu categoryContextMenu))
            {
                return;
            }

            categoryContextMenu.PlacementTarget = sender as UI.AssignmentTargetExtended;
            categoryContextMenu.IsOpen = true;
        }

        private void RightClickAutoAssignment(object sender, MouseButtonEventArgs eventArgs)
        {
            if (!(this.FindResource("AutomaticAssignmentContextMenu") is ContextMenu categoryContextMenu))
            {
                return;
            }

            categoryContextMenu.PlacementTarget = sender as Button;
            categoryContextMenu.IsOpen = true;
        }

        private void SelectCategory(object sender, EventArgs eventArgs)
        {
            var categoryTarget = sender as UI.AssignmentTargetExtended;

            var categoryName = categoryTarget?.Label;
            var category = categoryTarget?.Tag as Entities.Category;

            var lowerDayLimit = DateTime.UtcNow.Date;
            var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

            if (this.AssignmentDate.SelectedDate.HasValue)
            {
                lowerDayLimit = this.AssignmentDate.SelectedDate.Value.Date;
                upperDayLimit = this.AssignmentDate.SelectedDate.Value.AddDays(1).Date;
            }

            this.CategoryAssignmentGrid.Reset();

            var backToOverviewButton = new UI.AssignmentTargetExtended
            {
                Label = "<< Back",
                Background = new SolidColorBrush(Colors.LightGreen),
                AllowDrop = false,
            };

            backToOverviewButton.PreviewMouseLeftButtonDown += (o, args) =>
            {
                this.RefreshCategoryAssignmentView(null);
                this.ShowAllTimeTrackingElements();
            };

            this.CategoryAssignmentGrid.AddElement(backToOverviewButton);

            var selectedCategoryButton = new UI.AssignmentTargetExtended
            {
                Label = categoryName,
                Tag = category,
                AllowDrop = true,
            };

            selectedCategoryButton.Target.PrimaryActionButton.Background = !string.IsNullOrEmpty(category?.ExtendedConfiguration) ? category.ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>()?.SoluiNetBrushDefinition?.ToBrush() : new SolidColorBrush(Colors.WhiteSmoke);

            selectedCategoryButton.PreviewMouseRightButtonDown += this.RightClickApplication;

            this.CategoryAssignmentGrid.AddElement(selectedCategoryButton);

            var timeTargets = this.context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit && x.CategoryUsageTime.Any(y => y.Category.CategoryName == categoryName))
                .GroupBy(x => x.ApplicationIdentification);

            this.FillTimeTrackingOverview(timeTargets, this.SetBackgroundForCategoryElements, false, false);
        }

        private void FillCategoryTargets(
            List<Entities.Category> categories,
            DragEventHandler dropCategoryDelegate,
            MouseButtonEventHandler rightClickCategoryDelegate,
            MouseButtonEventHandler selectCategoryDelegate)
        {
            foreach (var category in categories)
            {
                var categoryTarget = new UI.AssignmentTargetExtended
                {
                    Label = category.CategoryName,
                };

                categoryTarget.Target.PrimaryActionButton.Background = !string.IsNullOrEmpty(category.ExtendedConfiguration) ? category.ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>()?.SoluiNetBrushDefinition?.ToBrush() : new SolidColorBrush(Colors.WhiteSmoke);

                categoryTarget.Tag = category;

                categoryTarget.AllowDrop = true;
                categoryTarget.Drop += dropCategoryDelegate;

                categoryTarget.PreviewMouseRightButtonDown += rightClickCategoryDelegate;
                categoryTarget.PreviewMouseLeftButtonDown += selectCategoryDelegate;

                this.CategoryAssignmentGrid.AddElement(categoryTarget);
            }
        }

        private void RefreshCategoryAssignmentView(TimeTrackingContext localContext)
        {
            if (localContext == null)
            {
                localContext = this.context;
            }

            this.CategoryAssignmentGrid.Reset();

            MouseButtonEventHandler rightClickCategoryDelegate = this.RightClickCategory;
            MouseButtonEventHandler selectCategoryDelegate = this.SelectCategory;

            var categories = localContext.Category.ToList();

            DragEventHandler dropCategoryDelegate = this.DropOnCategoryElement;

            if (this.CategoryAssignmentGrid.CreateNewElement == null)
            {
                this.CategoryAssignmentGrid.CreateNewElement = () =>
                {
                    var categoryName = Prompt.ShowDialog("Please provide an category name", "Category Assignment");

                    if (!localContext.Category.Any(x => x.CategoryName == categoryName))
                    {
                        var category = new Category() { CategoryName = categoryName };

                        localContext.Category.Add(category);
                        localContext.SaveChanges();

                        var newElement = new UI.AssignmentTargetExtended() { Label = categoryName };
                        newElement.Target.PrimaryActionButton.Background =
                            !string.IsNullOrEmpty(category.ExtendedConfiguration)
                                ? category.ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>()
                                    ?.SoluiNetBrushDefinition?.ToBrush()
                                : new SolidColorBrush(Colors.WhiteSmoke);

                        newElement.Tag = category;

                        newElement.AllowDrop = true;
                        newElement.Drop += dropCategoryDelegate;

                        newElement.PreviewMouseRightButtonDown += rightClickCategoryDelegate;

                        return newElement;
                    }
                    else
                    {
                        MessageBox.Show("Overgiven category name already exists");

                        return null;
                    }
                };
            }

            var distributeEvenlyElement = new UI.AssignmentTargetExtended()
            {
                AllowDrop = true,
                Label = "Distribute evenly",
            };

            distributeEvenlyElement.Drop += dropCategoryDelegate;

            this.CategoryAssignmentGrid.AddElement(distributeEvenlyElement);

            this.FillCategoryTargets(categories, dropCategoryDelegate, rightClickCategoryDelegate, selectCategoryDelegate);
        }

        private void SelectApplication(object sender, EventArgs eventArgs)
        {
            var applicationTarget = sender as UI.AssignmentTarget;

            var applicationName = applicationTarget?.Label;
            var application = applicationTarget?.Tag as Entities.Application;

            var lowerDayLimit = DateTime.UtcNow.Date;
            var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

            if (this.AssignmentDate.SelectedDate.HasValue)
            {
                lowerDayLimit = this.AssignmentDate.SelectedDate.Value.Date;
                upperDayLimit = this.AssignmentDate.SelectedDate.Value.AddDays(1).Date;
            }

            this.ApplicationAssignmentGrid.Reset();

            var backToOverviewButton = new UI.AssignmentTarget
            {
                Label = "<< Back",
                Background = new SolidColorBrush(Colors.LightGreen),
                AllowDrop = false,
            };

            backToOverviewButton.PreviewMouseLeftButtonDown += (o, args) =>
            {
                this.RefreshApplicationAssignmentView(null);
                this.ShowAllTimeTrackingElements();
            };

            this.ApplicationAssignmentGrid.AddElement(backToOverviewButton);

            var selectedApplicationButton = new UI.AssignmentTarget
            {
                Label = applicationName,
                Target =
                {
                    Background = !string.IsNullOrEmpty(application?.ExtendedConfiguration)
                        ? application.ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>()
                            ?.SoluiNetBrushDefinition?.ToBrush()
                        : new SolidColorBrush(Colors.WhiteSmoke),
                },
                Tag = application,
                AllowDrop = true,
            };

            selectedApplicationButton.PreviewMouseRightButtonDown += this.RightClickApplication;

            this.ApplicationAssignmentGrid.AddElement(selectedApplicationButton);

            var timeTargets = this.context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit && x.Application.ApplicationName == applicationName)
                .GroupBy(x => x.ApplicationIdentification);

            this.FillTimeTrackingOverview(timeTargets, this.SetBackgroundForApplicationElements, false, false);
        }

        private void FillApplicationTargets(
            List<Entities.Application> applications,
            DragEventHandler dropApplicationDelegate,
            MouseButtonEventHandler rightClickApplicationDelegate,
            MouseButtonEventHandler selectApplicationDelegate)
        {
            foreach (var application in applications)
            {
                var applicationTarget = new UI.AssignmentTarget
                {
                    Label = application.ApplicationName,
                    Target =
                    {
                        Background = !string.IsNullOrEmpty(application.ExtendedConfiguration)
                            ? application.ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>()
                                ?.SoluiNetBrushDefinition?.ToBrush()
                            : new SolidColorBrush(Colors.WhiteSmoke),
                    },
                    Tag = application,
                    AllowDrop = true,
                };

                applicationTarget.Drop += dropApplicationDelegate;

                applicationTarget.PreviewMouseRightButtonDown += rightClickApplicationDelegate;
                applicationTarget.PreviewMouseLeftButtonDown += selectApplicationDelegate;

                this.ApplicationAssignmentGrid.AddElement(applicationTarget);
            }
        }

        private void RefreshApplicationAssignmentView(TimeTrackingContext localContext)
        {
            if (localContext == null)
            {
                localContext = this.context;
            }

            this.ApplicationAssignmentGrid.Reset();

            var applications = localContext.Application.ToList();

            DragEventHandler dropApplicationDelegate = this.DropOnApplicationElement;
            MouseButtonEventHandler rightClickApplicationDelegate = this.RightClickApplication;
            MouseButtonEventHandler selectApplicationDelegate = this.SelectApplication;

            if (this.ApplicationAssignmentGrid.CreateNewElement == null)
            {
                this.ApplicationAssignmentGrid.CreateNewElement = () =>
                {
                    var applicationName =
                        Prompt.ShowDialog("Please provide an application name", "Application Assignment");

                    if (!localContext.Application.Any(x => x.ApplicationName == applicationName))
                    {
                        var application = new Entities.Application() { ApplicationName = applicationName };

                        localContext.Application.Add(application);
                        localContext.SaveChanges();

                        var newElement = new UI.AssignmentTarget() { Label = applicationName };
                        newElement.Target.Background = !string.IsNullOrEmpty(application.ExtendedConfiguration)
                            ? application.ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>()
                                ?.SoluiNetBrushDefinition?.ToBrush()
                            : new SolidColorBrush(Colors.WhiteSmoke);

                        newElement.Tag = application;

                        newElement.AllowDrop = true;
                        newElement.Drop += dropApplicationDelegate;

                        newElement.PreviewMouseRightButtonDown += rightClickApplicationDelegate;

                        return newElement;
                    }
                    else
                    {
                        MessageBox.Show("Overgiven application name already exists");

                        return null;
                    }
                };
            }

            this.FillApplicationTargets(applications, dropApplicationDelegate, rightClickApplicationDelegate, selectApplicationDelegate);
        }

        private void RefreshApplicationAreaAssignmentView(TimeTrackingContext localContext)
        {
            if (localContext == null)
            {
                localContext = this.context;
            }

            this.ApplicationAreaAssignmentGrid.Children.Clear();
            this.ApplicationAreaAssignmentGrid.PrepareControl();

            var applications = localContext.Application;

            DragEventHandler dropApplicationAreaDelegate = this.DropOnApplicationAreaElement;
            MouseButtonEventHandler rightClickApplicationAreaDelegate = this.RightClickApplicationArea;

            if (this.ApplicationAreaAssignmentGrid.CreateNewElement == null)
            {
                this.ApplicationAreaAssignmentGrid.CreateNewElement = () =>
                {
                    var applicationAreaName =
                        Prompt.ShowDialog("Please provide an application area name", "Application Area Assignment");

                    if (!localContext.Application.Any(x => x.ApplicationName == applicationAreaName))
                    {
                        var applicationArea = new Entities.ApplicationArea() { ApplicationName = applicationAreaName };

                        localContext.ApplicationArea.Add(applicationArea);
                        localContext.SaveChanges();

                        var newElement = new UI.AssignmentTarget() { Label = applicationAreaName };
                        newElement.Target.Background = !string.IsNullOrEmpty(applicationArea.ExtendedConfiguration)
                            ? applicationArea.ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>()
                                ?.SoluiNetBrushDefinition?.ToBrush()
                            : new SolidColorBrush(Colors.WhiteSmoke);

                        newElement.Tag = applicationArea;

                        newElement.AllowDrop = true;
                        newElement.Drop += dropApplicationAreaDelegate;

                        newElement.PreviewMouseRightButtonDown += rightClickApplicationAreaDelegate;

                        return newElement;
                    }
                    else
                    {
                        MessageBox.Show("Overgiven application name already exists");

                        return null;
                    }
                };
            }

            foreach (var application in applications)
            {
                var applicationAreaTarget = new UI.AssignmentTarget
                {
                    Label = application.ApplicationName,
                    Target =
                    {
                        Background = !string.IsNullOrEmpty(application.ExtendedConfiguration)
                            ? application.ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>()
                                ?.SoluiNetBrushDefinition?.ToBrush()
                            : new SolidColorBrush(Colors.WhiteSmoke),
                    },
                    Tag = application,
                    AllowDrop = true,
                };

                applicationAreaTarget.Drop += dropApplicationAreaDelegate;

                applicationAreaTarget.PreviewMouseRightButtonDown += rightClickApplicationAreaDelegate;

                this.ApplicationAreaAssignmentGrid.AddElement(applicationAreaTarget);
            }
        }

        private void PrepareAssignmentView(DateTime lowerDayLimit, DateTime upperDayLimit, TimeTrackingContext localContext, bool showOnlyUnassigned = false)
        {
            var timeTargets = localContext.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit)
                .GroupBy(x => x.ApplicationIdentification);

            this.FillTimeTrackingOverview(timeTargets);

            this.RefreshApplicationAssignmentView(localContext);
            this.RefreshCategoryAssignmentView(localContext);
            this.RefreshApplicationAreaAssignmentView(localContext);
        }

        private void PrepareSourceDataView(DateTime lowerDayLimit, DateTime upperDayLimit, TimeTrackingContext context)
        {
            context.UsageTime.Load();

            this.SourceData.AutoGenerateColumns = true;
            this.SourceData.ItemsSource = context.UsageTime.Local.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit);
        }

        private void RearrangeWidths()
        {
            var highestDuration = Convert.ToDouble(this.TimeTrackingAssignmentOverview.Tag, CultureInfo.InvariantCulture);

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

        private void TimeTrackingTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as TabControl).SelectedIndex != 1)
            {
                return;
            }

            this.TimeTrackingAssignmentOverview.Loaded += (overviewSender, eventArgs) =>
            {
                this.RearrangeWidths();

                this.overviewLoaded = true;
            };

            if (this.overviewLoaded)
            {
                this.RearrangeWidths();
            }
        }

        private Brush SetBackgroundForApplicationElements(object applicationName)
        {
            return this.context.Application.Local
            .FirstOrDefault(x => !string.IsNullOrEmpty(x.ExtendedConfiguration)
                && x.ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>().regEx.RegExMatch(applicationName.ToString().ReplaceRegEx(GeneralConstants.DurationSearchPattern)))?
            .ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>()?.SoluiNetBrushDefinition.ToBrush();
        }

        private Brush SetBackgroundForCategoryElements(object categoryName)
        {
            return this.context.Category.Local
            .FirstOrDefault(x => !string.IsNullOrEmpty(x.ExtendedConfiguration)
                && x.ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>().regEx.RegExMatch(categoryName.ToString().ReplaceRegEx(GeneralConstants.DurationSearchPattern)))?
            .ExtendedConfiguration.DeserializeString<SoluiNetExtendedConfigurationType>()?.SoluiNetBrushDefinition.ToBrush();
        }

        private void FillTimeTrackingOverview(
            IQueryable<IGrouping<string, UsageTime>> timeTargets,
            ExtendedButton.ResolveBackgroundColour backgroundColourResolveDelegate = null,
            bool subtractAssignedDuration = true,
            bool ignoreSmallDurations = true)
        {
            this.TimeTrackingAssignmentOverview.Children.Clear();
            this.TimeTrackingAssignmentOverview.RowDefinitions.Clear();

            var highestDuration = timeTargets.Any() ? timeTargets.Max(x => x.Any() ? x.Sum(y => y != null ? y.Duration : 0) : 0) : 0;
            this.TimeTrackingAssignmentOverview.Tag = highestDuration;

            var header = (this.TimeTrackingAssignmentTargetTabs.SelectedItem as TabItem)?.Header?.ToString();

            foreach (var timeTarget in timeTargets)
            {
                this.TimeTrackingAssignmentOverview.RowDefinitions.Add(new RowDefinition());

                var duration = Convert.ToDouble(timeTarget.Sum(x => x.Duration));

                if (header == "Category" && subtractAssignedDuration)
                {
                    duration -= timeTarget.Sum(x => x.CategoryUsageTime?.Sum(y => y.Duration) ?? 0);
                }

                duration = Math.Round(duration);

                if (Math.Abs(duration) < 0.00001 && ignoreSmallDurations)
                {
                    continue;
                }

                var label = $"{timeTarget.Key} ({duration.ToDurationString()})";

                var timeTargetButton = new ExtendedButton(this.UseLengthDependency.IsChecked.HasValue && this.UseLengthDependency.IsChecked.Value) { HorizontalAlignment = HorizontalAlignment.Left };

                if (backgroundColourResolveDelegate != null)
                {
                    timeTargetButton.OnBackgroundColourResolving = backgroundColourResolveDelegate;
                }
                else if (string.IsNullOrEmpty(header) || header == "Application")
                {
                    timeTargetButton.OnBackgroundColourResolving = this.SetBackgroundForApplicationElements;
                }
                else if (header == "Category")
                {
                    timeTargetButton.OnBackgroundColourResolving = this.SetBackgroundForCategoryElements;
                }

                timeTargetButton.Content = label;
                timeTargetButton.ToolTip = label;
                timeTargetButton.Tag = timeTarget;

                if (highestDuration > 0)
                {
                    timeTargetButton.DependencyReferenceValue = highestDuration;
                    timeTargetButton.DependencyValue = Convert.ToDouble(timeTarget.Sum(x => x.Duration));
                }

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

        private void ShowAllTimeTrackingElements()
        {
            var lowerDayLimit = DateTime.UtcNow.Date;
            var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

            if (this.AssignmentDate.SelectedDate.HasValue)
            {
                lowerDayLimit = this.AssignmentDate.SelectedDate.Value.Date;
                upperDayLimit = this.AssignmentDate.SelectedDate.Value.AddDays(1).Date;
            }

            var timeTargets = this.context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit).GroupBy(x => x.ApplicationIdentification);

            this.FillTimeTrackingOverview(timeTargets, null, false);
        }

        private void TimeTrackingAssignmentTargetTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as TabControl).SelectedItem == null)
            {
                return;
            }

            var header = ((sender as TabControl).SelectedItem as TabItem).Header.ToString();

            foreach (var assignmentButton in this.TimeTrackingAssignmentOverview.Children)
            {
                if (assignmentButton is ExtendedButton)
                {
                    if (string.IsNullOrEmpty(header) || header == "Application")
                    {
                        (assignmentButton as ExtendedButton).OnBackgroundColourResolving = this.SetBackgroundForApplicationElements;
                        (assignmentButton as ExtendedButton).Refresh();
                    }
                    else if (header == "Category")
                    {
                        (assignmentButton as ExtendedButton).OnBackgroundColourResolving = this.SetBackgroundForCategoryElements;
                        (assignmentButton as ExtendedButton).Refresh();
                    }
                }
            }

            this.ShowAll.RemoveEvent("Click");
            this.ShowAll.Click += (showAllButton, eventArgs) =>
            {
                this.ShowAllTimeTrackingElements();
            };

            if (header == "Application")
            {
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
            else if (header == "Category")
            {
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

                    var timeTargets = this.context.UsageTime.Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit && (!x.CategoryUsageTime.Any() || x.CategoryUsageTime.Sum(y => y.Duration) < x.Duration)).GroupBy(x => x.ApplicationIdentification);

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
                this.QueryData.Columns.Add(new DataGridTextColumn() { Header = "Duration", Binding = new Binding("Duration") { Converter = new SoluiNet.DevTools.Core.UI.WPF.Converter.DurationConverter() } });
                this.QueryData.Columns.Add(new DataGridTextColumn() { Header = "Application", Binding = new Binding("Application.ApplicationName") });
                this.QueryData.Columns.Add(new DataGridTextColumn() { Header = "ApplicationArea", Binding = new Binding("ApplicationArea.ApplicationName") });
            }

            foreach (var item in queryResults)
            {
                this.QueryData.Items.Add(item);
            }
        }

        private void StartQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var lowerDayLimit = DateTime.UtcNow.Date;
                var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

                if (this.QueryDateBegin.SelectedDate.HasValue && this.QueryDateEnd.SelectedDate.HasValue)
                {
                    lowerDayLimit = this.QueryDateBegin.SelectedDate.Value.Date;
                    upperDayLimit = this.QueryDateEnd.SelectedDate.Value.AddDays(1).Date;
                }

                var queryResults = this.context.UsageTime
                    .Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit).ToList();

                if (!string.IsNullOrEmpty(this.QueryFilter.Text))
                {
                    queryResults = queryResults.AsQueryable().Where(this.QueryFilter.Text).ToList();
                }

                this.FillQueryResults(queryResults);

                var userName = $"{Environment.UserDomainName}\\{Environment.UserName}";

                var existingFilters = this.context.FilterHistory.Where(x =>
                    x.FilterString == this.QueryFilter.Text && x.ExecutionUser == userName);

                if (!existingFilters.Any())
                {
                    this.context.FilterHistory.Add(new FilterHistory()
                    {
                        ExecutionUser = userName,
                        FilterString = this.QueryFilter.Text,
                        LastExecutionDateTime = DateTime.UtcNow,
                    });

                    this.QueryFilter.Items.Add(this.QueryFilter.Text);
                }
                else
                {
                    var existingFilter = existingFilters
                        .OrderByDescending(x => x.LastExecutionDateTime)
                        .FirstOrDefault();

                    if (existingFilter != null)
                    {
                        existingFilter.LastExecutionDateTime = DateTime.UtcNow;
                    }
                }

                this.context.SaveChanges();
            }
            catch (DataException exception)
            {
                Logger.Error(exception, "Error while executing query '{0}'", !string.IsNullOrEmpty(this.QueryFilter.Text) ? this.QueryFilter.Text : "no filter");

                MessageBox.Show(exception.Message, Resources.GetString("QueryError", CultureInfo.CurrentCulture));
            }
        }

        private void FillSummaryResults(Dictionary<string, double> summaryResults)
        {
            if (summaryResults == null)
            {
                return;
            }

            this.SummaryData.Items.Clear();

            if (this.SummaryData.Columns.Count == 0)
            {
                this.SummaryData.Columns.Add(new DataGridTextColumn() { Header = "Grouping Element", Binding = new Binding("Key") });
                this.SummaryData.Columns.Add(new DataGridTextColumn() { Header = "Duration Value", Binding = new Binding("Value") { Converter = new SoluiNet.DevTools.Core.UI.WPF.Converter.DurationConverter() } });
                this.SummaryData.Columns.Add(new DataGridTextColumn() { Header = "Value", Binding = new Binding("Value") });
            }

            foreach (var item in summaryResults)
            {
                this.SummaryData.Items.Add(item);
            }

            this.SummaryData.Items.Add(new KeyValuePair<string, double>("Sum", summaryResults.Sum(x => x.Value)));
        }

        private void FillSummaryResults(Dictionary<string, Dictionary<string, double>> summaryResults)
        {
            if (summaryResults == null)
            {
                return;
            }

            this.SummaryData.Items.Clear();

            if (this.SummaryData.Columns.Count == 0)
            {
                this.SummaryData.Columns.Add(new DataGridTextColumn() { Header = "Grouping Date", Binding = new Binding("Date") });
                this.SummaryData.Columns.Add(new DataGridTextColumn() { Header = "Grouping Element", Binding = new Binding("GroupingElement") });
                this.SummaryData.Columns.Add(new DataGridTextColumn() { Header = "Duration Value", Binding = new Binding("Duration") { Converter = new SoluiNet.DevTools.Core.UI.WPF.Converter.DurationConverter() } });
                this.SummaryData.Columns.Add(new DataGridTextColumn() { Header = "Value", Binding = new Binding("Duration") });
            }

            foreach (var dateGroupItem in summaryResults)
            {
                foreach (var item in dateGroupItem.Value)
                {
                    this.SummaryData.Items.Add(new { Date = dateGroupItem.Key, GroupingElement = item.Key, Duration = item.Value });
                }

                this.SummaryData.Items.Add(new { Date = dateGroupItem.Key, GroupingElement = "Sum", Duration = dateGroupItem.Value.Sum(x => x.Value) });
            }

            this.SummaryData.Items.Add(new { Date = string.Empty, GroupingElement = "Total Sum", Duration = summaryResults.Sum(x => x.Value.Sum(y => y.Value)) });
        }

        private void StartSummary_Click(object sender, RoutedEventArgs e)
        {
            var summaryType = (this.SummaryType.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(summaryType))
            {
                Confirm.ShowDialog(
                    Resources.GetString("SelectSummaryType", CultureInfo.CurrentCulture),
                    Resources.GetString("Warning", CultureInfo.CurrentCulture));

                return;
            }

            var lowerDayLimit = DateTime.UtcNow.Date;
            var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

            if (this.SummaryDateBegin.SelectedDate.HasValue && this.SummaryDateEnd.SelectedDate.HasValue)
            {
                lowerDayLimit = this.SummaryDateBegin.SelectedDate.Value.Date;
                upperDayLimit = this.SummaryDateEnd.SelectedDate.Value.AddDays(1).Date;
            }

            if ((upperDayLimit - lowerDayLimit).TotalMinutes <= 24 * 60)
            {
                Dictionary<string, double> summaryResults = null;

                if (summaryType == "Application")
                {
                    summaryResults = this.context.UsageTime
                        .Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit)
                        .GroupBy(x => x.Application != null ? x.Application.ApplicationName : "n/a")
                        .ToDictionary(x => !string.IsNullOrEmpty(x.Key) ? x.Key : "n/a", y => Convert.ToDouble(y.Sum(z => z.Duration)));
                }
                else if (summaryType == "Category")
                {
                    summaryResults = this.context.CategoryUsageTime
                        .Where(x => x.UsageTime.StartTime >= lowerDayLimit && x.UsageTime.StartTime < upperDayLimit)
                        .GroupBy(x => x.Category != null ? x.Category.CategoryName : "n/a")
                        .ToDictionary(x => !string.IsNullOrEmpty(x.Key) ? x.Key : "n/a", y => Math.Round(Convert.ToDouble(y.Sum(z => z.Duration))));

                    var notAssignedCategoryDurationList = this.context.UsageTime
                        .Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit &&
                                    !x.CategoryUsageTime.Any());

                    var notAssignedCategoryDuration = notAssignedCategoryDurationList.Any() ? notAssignedCategoryDurationList.Sum(x => x.Duration) : 0;

                    summaryResults.Add(
                        "n/a",
                        Math.Round(Convert.ToDouble(notAssignedCategoryDuration)));
                }

                this.FillSummaryResults(summaryResults);
            }
            else
            {
                var summaryResults = new Dictionary<string, Dictionary<string, double>>();

                if (summaryType == "Application")
                {
                    var usageTimeInPeriod = this.context.UsageTime
                        .Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit)
                        .ToList();

                    foreach (var item in usageTimeInPeriod.GroupBy(x => x.StartTime.Date))
                    {
                        summaryResults.Add(
                            item.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                            item
                                .GroupBy(x => x.Application != null ? (!string.IsNullOrEmpty(x.Application.ApplicationName) ? x.Application.ApplicationName : "n/a") : "n/a")
                                .ToDictionary(x => !string.IsNullOrEmpty(x.Key) ? x.Key : "n/a", y => Math.Round(Convert.ToDouble(y.Sum(z => z.Duration)))));
                    }
                }
                else if (summaryType == "Category")
                {
                    var usageTimeInPeriod = this.context.CategoryUsageTime
                        .Where(x => x.UsageTime.StartTime >= lowerDayLimit && x.UsageTime.StartTime < upperDayLimit)
                        .ToList();

                    foreach (var item in usageTimeInPeriod.GroupBy(x => x.UsageTime.StartTime.Date))
                    {
                        var dateDictionary = item
                            .GroupBy(x => x.Category != null ? x.Category.CategoryName : "n/a")
                            .ToDictionary(x => !string.IsNullOrEmpty(x.Key) ? x.Key : "n/a", y => Math.Round(Convert.ToDouble(y.Sum(z => z.Duration))));

                        var endTimepoint = item.Key.Date.AddDays(1);

                        var notAssignedCategoryDurationList = this.context.UsageTime
                            .Where(x => x.StartTime >= item.Key.Date && x.StartTime < endTimepoint &&
                                        !x.CategoryUsageTime.Any());

                        var notAssignedCategoryDuration = notAssignedCategoryDurationList.Any() ? notAssignedCategoryDurationList.Sum(x => x.Duration) : 0;

                        if (dateDictionary.ContainsKey("n/a"))
                        {
                            dateDictionary["n/a"] += Math.Round(Convert.ToDouble(notAssignedCategoryDuration));
                        }
                        else
                        {
                            dateDictionary.Add(
                                "n/a",
                                Math.Round(Convert.ToDouble(notAssignedCategoryDuration)));
                        }

                        if (summaryResults.ContainsKey(item.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)))
                        {
                            var entry = summaryResults[item.Key.ToString(CultureInfo.InvariantCulture)];

                            summaryResults[item.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)] = entry.Merge(dateDictionary);
                        }
                        else
                        {
                            summaryResults.Add(
                                item.Key.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                                dateDictionary);
                        }
                    }
                }

                this.FillSummaryResults(summaryResults);
            }
        }

        private void ApplicationSettings_Click(object sender, RoutedEventArgs e)
        {
            var applicationButton = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as UI.AssignmentTarget;

            var window = new SoluiNetWindow();

            // todo: get element which has been right clicked and deliver via constructor parameter
            window.ShowWithUserControl(new ExtendedConfigurationUserControl(applicationButton.Tag as Entities.Application));

            this.context.SaveChanges();
        }

        private void ApplicationAreaSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!(((sender as MenuItem)?.Parent as ContextMenu)?.PlacementTarget is UI.AssignmentTarget applicationAreaButton))
            {
                return;
            }

            var window = new SoluiNetWindow();

            // todo: get element which has been right clicked and deliver via constructor parameter
            window.ShowWithUserControl(new ExtendedConfigurationUserControl(applicationAreaButton.Tag as Entities.ApplicationArea));

            this.context.SaveChanges();
        }

        private void CategorySettings_Click(object sender, RoutedEventArgs e)
        {
            if (!(((sender as MenuItem)?.Parent as ContextMenu)?.PlacementTarget is UI.AssignmentTargetExtended categoryButton))
            {
                return;
            }

            var window = new SoluiNetWindow();

            window.ShowWithUserControl(new CategorySettings(categoryButton.Tag as Entities.Category));

            this.context.SaveChanges();
        }

        private void TimeTrackingAssignmentOverview_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                foreach (var assignment in this.TimeTrackingAssignmentOverview.Children)
                {
                    if (assignment is ExtendedButton button)
                    {
                        button.SwitchSelection();
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "Ignore for better readability")]
        private void AutomaticAssignAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                const int batchSize = 1000;

                var unassignedApplicationBatch = this.context.UsageTime
                    .Where(x => x.ApplicationId == null &&
                                (x.ApplicationAutomaticAssigned == null || !x.ApplicationAutomaticAssigned.Value))
                    .OrderBy(x => x.UsageTimeId)
                    .Take(batchSize);
                var iteration = 1;

                do
                {
                    foreach (var unassigned in unassignedApplicationBatch)
                    {
                        var content = unassigned.ApplicationIdentification;

                        foreach (var application in this.context.Application)
                        {
                            if (string.IsNullOrEmpty(application.ExtendedConfiguration))
                            {
                                continue;
                            }

                            if (content.MatchesRegEx(application.ExtendedConfiguration
                                .DeserializeString<SoluiNetExtendedConfigurationType>().regEx))
                            {
                                Logger.Info("automatic assign '{0}' to application '{1}'", content,
                                    application.ApplicationName);

                                unassigned.ApplicationId = application.ApplicationId;
                            }
                        }

                        unassigned.ApplicationAutomaticAssigned = true;
                    }

                    Logger.Info("[Automatic Assign Application] Persist changes");
                    this.context.SaveChanges();

                    Logger.Info(
                        "[Automatic Assign Application] Get next batch ({1}) of {2}, {0} already done",
                        unassignedApplicationBatch.Count() < 1000
                            ? ((iteration - 1) * batchSize) + unassignedApplicationBatch.Count()
                            : iteration * batchSize,
                        iteration,
                        batchSize);
                    unassignedApplicationBatch = this.context.UsageTime
                        .Where(x => x.ApplicationId == null &&
                                    (x.ApplicationAutomaticAssigned == null || !x.ApplicationAutomaticAssigned.Value))
                        .OrderBy(x => x.UsageTimeId)
                        .Skip(iteration * batchSize).Take(batchSize);
                    iteration++;
                } while (unassignedApplicationBatch.Any());

                var unassignedCategoryBatch = this.context.UsageTime
                    .Where(x => !x.CategoryUsageTime.Any() &&
                                (x.CategoryAutomaticAssigned == null || !x.CategoryAutomaticAssigned.Value))
                    .OrderBy(x => x.UsageTimeId)
                    .Take(batchSize);
                iteration = 1;

                do
                {
                    foreach (var unassigned in unassignedCategoryBatch)
                    {
                        var content = unassigned.ApplicationIdentification;

                        foreach (var category in this.context.Category)
                        {
                            if (string.IsNullOrEmpty(category.ExtendedConfiguration))
                            {
                                continue;
                            }

                            if (content.MatchesRegEx(category.ExtendedConfiguration
                                .DeserializeString<SoluiNetExtendedConfigurationType>().regEx))
                            {
                                Logger.Info("automatic assign '{0}' to category '{1}'", content, category.CategoryName);

                                this.context.CategoryUsageTime.Add(new CategoryUsageTime()
                                {
                                    UsageTimeId = unassigned.UsageTimeId,
                                    Duration = unassigned.Duration,
                                    CategoryId = category.CategoryId,
                                });
                            }
                        }

                        unassigned.CategoryAutomaticAssigned = true;
                    }

                    Logger.Info("[Automatic Assign Category] Persist changes");
                    this.context.SaveChanges();

                    Logger.Info(
                        "[Automatic Assign Category] Get next batch ({1}) of {2}, {0} already done",
                        unassignedCategoryBatch.Count() < 1000
                            ? ((iteration - 1) * batchSize) + unassignedCategoryBatch.Count()
                            : iteration * batchSize,
                        iteration,
                        batchSize);

                    unassignedCategoryBatch = this.context.UsageTime
                        .Where(x => !x.CategoryUsageTime.Any() &&
                                    (x.CategoryAutomaticAssigned == null || !x.CategoryAutomaticAssigned.Value))
                        .OrderBy(x => x.UsageTimeId)
                        .Skip(iteration * batchSize)
                        .Take(batchSize);
                    iteration++;
                }
                while (unassignedCategoryBatch.Any());

                this.context.SaveChanges();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while processing Automatic Assignment (All)");
            }
        }

        private void AutomaticAssignment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var header = (this.TimeTrackingAssignmentTargetTabs.SelectedItem as TabItem)?.Header?.ToString();

                foreach (var assignment in this.TimeTrackingAssignmentOverview.Children)
                {
                    var content = string.Empty;

                    if (assignment is ExtendedButton)
                    {
                        content = ((assignment as ExtendedButton).Tag as IGrouping<string, UsageTime>).Key;
                    }

                    if (string.IsNullOrEmpty(content))
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(header) || header == "Application")
                    {
                        foreach (var application in this.context.Application)
                        {
                            if (string.IsNullOrEmpty(application.ExtendedConfiguration))
                            {
                                continue;
                            }

                            if (content.MatchesRegEx(application.ExtendedConfiguration
                                .DeserializeString<SoluiNetExtendedConfigurationType>().regEx))
                            {
                                foreach (var usageTime in
                                    ((assignment as ExtendedButton).Tag as IGrouping<string, UsageTime>).Select(x => x))
                                {
                                    usageTime.ApplicationId = application.ApplicationId;

                                    usageTime.ApplicationAutomaticAssigned = true;
                                }
                            }
                        }
                    }
                    else if (header == "Category")
                    {
                        foreach (var category in this.context.Category)
                        {
                            if (string.IsNullOrEmpty(category.ExtendedConfiguration))
                            {
                                continue;
                            }

                            if (content.MatchesRegEx(category.ExtendedConfiguration
                                .DeserializeString<SoluiNetExtendedConfigurationType>().regEx))
                            {
                                foreach (var usageTime in
                                    ((assignment as ExtendedButton).Tag as IGrouping<string, UsageTime>).Select(x => x))
                                {
                                    this.context.CategoryUsageTime.Add(new CategoryUsageTime()
                                    {
                                        UsageTimeId = usageTime.UsageTimeId,
                                        Duration = usageTime.Duration,
                                        CategoryId = category.CategoryId,
                                    });

                                    usageTime.CategoryAutomaticAssigned = true;
                                }
                            }
                        }
                    }
                }

                this.context.SaveChanges();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while processing Automatic Assignment (Manual)");
            }
        }

        private void CopyMonthToClipboard_Click(object sender, RoutedEventArgs e)
        {
            var copyType = (this.CopyType.SelectedItem as ComboBoxItem)?.Content.ToString();
            var clipboardType = (this.ClipboardType.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(copyType))
            {
                copyType = "Category";
            }

            if (string.IsNullOrEmpty(clipboardType))
            {
                clipboardType = "Excel";
            }

            var startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);

            if (this.TasksDateBegin.SelectedDate.HasValue)
            {
                startDate = this.TasksDateBegin.SelectedDate.Value;
            }

            if (this.TasksDateEnd.SelectedDate.HasValue)
            {
                endDate = this.TasksDateEnd.SelectedDate.Value;
            }

            var categoryName = string.Empty;

            var formatString = string.Empty;

            if (clipboardType == "Excel")
            {
                formatString = "{0:0.00}\t";
            }
            else if (clipboardType == "CSV")
            {
                formatString = "{0:0.00},";
            }
            else if (clipboardType == "XML")
            {
                formatString = "<value timestamp=\"{1}\">{0:0.00}</value>";
            }

            var searchValues = this.SearchValue.Text.Split(';');

            if (copyType == "Application")
            {
                // do nothing
            }
            else if (copyType == "Category")
            {
                var usageTimePerDayAndCategory = this.context.CategoryUsageTime.Where(x =>
                    x.UsageTime.StartTime >= startDate && x.UsageTime.StartTime <= endDate &&
                    searchValues.Contains(x.Category.CategoryName)).
                    Select(x => new { Category = x.Category.CategoryName, x.Duration, x.UsageTime.StartTime }).
                    ToList().
                    GroupBy(x => new { x.Category, StartTime = x.StartTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) }).
                    ToList();

                var clipboardContent = string.Empty;

                for (var dateIterator = startDate; dateIterator < endDate; dateIterator = dateIterator.AddDays(1))
                {
                    clipboardContent += string.Format(
                        CultureInfo.InvariantCulture,
                        formatString,
                        usageTimePerDayAndCategory
                        .Where(x => x.Key.StartTime == dateIterator.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture))
                        .Sum(x => x.Sum(y => y.Duration)).SecondsToHours().RoundWithDelta(0.25),
                        dateIterator.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                }

                Clipboard.SetText(clipboardContent);
            }
        }

        private void CategoryDelete_Click(object sender, RoutedEventArgs e)
        {
            var categoryButton = ((sender as MenuItem)?.Parent as ContextMenu)?.PlacementTarget as UI.AssignmentTargetExtended;

            if (!(categoryButton?.Tag is Category category))
            {
                return;
            }

            if (Confirm.ShowDialog(
                $"Do you really want to delete the category '{category.CategoryName}'?",
                Resources.GetString("ConfirmDeletion", CultureInfo.CurrentCulture)))
            {
                this.context.Category.Remove(category);

                this.context.SaveChanges();

                this.RefreshCategoryAssignmentView(null);
            }
        }

        private void FillTimespanResults(IEnumerable<UsageTime> queryResults)
        {
            this.TimespanData.Items.Clear();

            if (this.TimespanData.Columns.Count == 0)
            {
                this.TimespanData.Columns.Add(new DataGridTextColumn() { Header = "Date", Binding = new Binding("Date") });
                this.TimespanData.Columns.Add(new DataGridTextColumn() { Header = "StartTime", Binding = new Binding("StartTime") });
                this.TimespanData.Columns.Add(new DataGridTextColumn() { Header = "EndTime", Binding = new Binding("EndTime") });
                this.TimespanData.Columns.Add(new DataGridTextColumn() { Header = "Duration", Binding = new Binding("Duration") });
            }

            foreach (var item in queryResults.ToList().GroupBy(x => x.StartTime.Date))
            {
                var firstItem = item.OrderBy(x => x.StartTime).FirstOrDefault();
                var lastItem = item.OrderByDescending(x => x.StartTime).FirstOrDefault();

                var endTime = lastItem?.StartTime.AddSeconds(lastItem.Duration);

                this.TimespanData.Items.Add(new
                {
                    Date = item.Key.Date.ToString(CultureInfo.InvariantCulture),
                    firstItem?.StartTime,
                    EndTime = endTime,
                    Duration = ((endTime - (firstItem?.StartTime ?? endTime)) ?? new TimeSpan(0)).TotalSeconds.RoundWithDelta(1).ToDurationString(),
                });
            }
        }

        private void TimespanQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var lowerDayLimit = DateTime.UtcNow.Date;
                var upperDayLimit = DateTime.UtcNow.AddDays(1).Date;

                if (this.TimespanBegin.SelectedDate.HasValue && this.TimespanEnd.SelectedDate.HasValue)
                {
                    lowerDayLimit = this.TimespanBegin.SelectedDate.Value.Date;
                    upperDayLimit = this.TimespanEnd.SelectedDate.Value.AddDays(1).Date;
                }

                var queryResults = this.context.UsageTime
                    .Where(x => x.StartTime >= lowerDayLimit && x.StartTime < upperDayLimit).ToList();

                this.FillTimespanResults(queryResults);
            }
            catch (DataException exception)
            {
                Logger.Error(exception, "Error while executing time span query");

                MessageBox.Show(exception.Message, Resources.GetString("QueryError", CultureInfo.CurrentCulture));
            }
        }

        private void Optimize_Click(object sender, RoutedEventArgs e)
        {
            TimeTrackingContext.Optimize();
        }

        private void CleanUp_Click(object sender, RoutedEventArgs e)
        {
            TimeTrackingContext.CleanUp();
        }
    }
}
