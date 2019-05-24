// <copyright file="MachineLearningUserControl.xaml.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Utils.MachineLearning
{
    using System;
    using System.Collections.Generic;
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
    using Microsoft.ML;
    using Newtonsoft.Json;
    using SoluiNet.DevTools.Core.Tools.Json;
    using SoluiNet.DevTools.Utils.MachineLearning.Models;

    /// <summary>
    /// Interaktionslogik für MachineLearningUserControl.xaml.
    /// </summary>
    public partial class MachineLearningUserControl : UserControl
    {
        public MachineLearningUserControl()
        {
            this.InitializeComponent();
        }

        private void PredictResults_Click(object sender, RoutedEventArgs e)
        {
            var learningContext = new MLContext();

            SimpleLearningModel<float, float>[] trainingData =
            {
                new SimpleLearningModel<float, float> { ReferenceValue = 1, DependentValue = 1 },
                new SimpleLearningModel<float, float> { ReferenceValue = 2, DependentValue = 2 },
                new SimpleLearningModel<float, float> { ReferenceValue = 3, DependentValue = 4 },
                new SimpleLearningModel<float, float> { ReferenceValue = 4, DependentValue = 8 },
                new SimpleLearningModel<float, float> { ReferenceValue = 5, DependentValue = 64 },
                new SimpleLearningModel<float, float> { ReferenceValue = 6, DependentValue = 128 },
                new SimpleLearningModel<float, float> { ReferenceValue = 7, DependentValue = Convert.ToSingle(Math.Pow(128, 2)) },
                new SimpleLearningModel<float, float> { ReferenceValue = 8, DependentValue = Convert.ToSingle(Math.Pow(128, 2)) * 2 },
            };

            var trainingDataView = learningContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = learningContext.Transforms
                .Concatenate("Features", new[] { "ReferenceValue" })
                .Append(learningContext.Regression.Trainers.LbfgsPoissonRegression(
                    labelColumnName: "DependentValue",
                    historySize: 100));

            var model = pipeline.Fit(trainingDataView);

            var predictionModel = new SimpleLearningModel<float, float> { ReferenceValue = 9 };
            var calculatedPrediction = learningContext.Model.CreatePredictionEngine<SimpleLearningModel<float, float>, SimplePredictionModel<float>>(model).Predict(predictionModel);

            this.Predictions.Text = JsonTools.Serialize(calculatedPrediction);
        }
    }
}
