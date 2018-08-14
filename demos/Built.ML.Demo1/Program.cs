using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using Microsoft.ML.Data;
using System;

namespace Built.ML.Demo1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // STEP 2: Create a pipeline and load your data
            var pipeline = new LearningPipeline();

            // If working in Visual Studio, make sure the 'Copy to Output Directory'
            // property of iris-data.txt is set to 'Copy always'
            string dataPath = "iris.data.txt";
            pipeline.Add(new TextLoader(dataPath).CreateFrom<HistoryData>(separator: ','));

            // STEP 3: Transform your data
            // Assign numeric values to text in the "Label" column, because only
            // numbers can be processed during model training
            pipeline.Add(new Dictionarizer("Label"));

            // Puts all features into a vector
            pipeline.Add(new ColumnConcatenator("Features", "SepalLength", "SepalWidth", "PetalLength", "PetalWidth"));

            // STEP 4: Add learner
            // Add a learning algorithm to the pipeline.
            // This is a classification scenario (What type of iris is this?)
            pipeline.Add(new StochasticDualCoordinateAscentClassifier());

            // Convert the Label back into original text (after converting to number in step 3)
            pipeline.Add(new PredictedLabelColumnOriginalValueConverter() { PredictedLabelColumn = "PredictedLabel" });

            // STEP 5: Train your model based on the data set
            var model = pipeline.Train<HistoryData, HistoryDataPrediction>();

            // STEP 6: Use your model to make a prediction
            // You can change these numbers to test different predictions
            //var prediction = model.Predict(new HistoryData()
            //{
            //    Num1
            //});

            //Console.WriteLine($"Predicted flower type is: {prediction.PredictedLabels}");

            Console.ReadKey();
        }
    }
}