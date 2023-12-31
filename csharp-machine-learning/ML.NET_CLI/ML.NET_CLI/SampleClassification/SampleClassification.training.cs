// This file was auto-generated by ML.NET Model Builder.
using Microsoft.ML;
using Microsoft.ML.Trainers.LightGbm;
using Microsoft.ML.Transforms;

namespace SampleClassification.ConsoleApp
{
    public partial class SampleClassification
    {
        public const string RetrainFilePath =  @"..\DataSets\credit_customers.csv";
        public const char RetrainSeparatorChar = ',';
        public const bool RetrainHasHeader =  true;

         /// <summary>
        /// Train a new model with the provided dataset.
        /// </summary>
        /// <param name="outputModelPath">File path for saving the model. Should be similar to "C:\YourPath\ModelName.mlnet"</param>
        /// <param name="inputDataFilePath">Path to the data file for training.</param>
        /// <param name="separatorChar">Separator character for delimited training file.</param>
        /// <param name="hasHeader">Boolean if training file has a header.</param>
        public static void Train(string outputModelPath, string inputDataFilePath = RetrainFilePath, char separatorChar = RetrainSeparatorChar, bool hasHeader = RetrainHasHeader)
        {
            var mlContext = new MLContext();

            var data = LoadIDataViewFromFile(mlContext, inputDataFilePath, separatorChar, hasHeader);
            var model = RetrainModel(mlContext, data);
            SaveModel(mlContext, model, data, outputModelPath);
        }

        /// <summary>
        /// Load an IDataView from a file path.
        /// </summary>
        /// <param name="mlContext">The common context for all ML.NET operations.</param>
        /// <param name="inputDataFilePath">Path to the data file for training.</param>
        /// <param name="separatorChar">Separator character for delimited training file.</param>
        /// <param name="hasHeader">Boolean if training file has a header.</param>
        /// <returns>IDataView with loaded training data.</returns>
        public static IDataView LoadIDataViewFromFile(MLContext mlContext, string inputDataFilePath, char separatorChar, bool hasHeader)
        {
            return mlContext.Data.LoadFromTextFile<ModelInput>(inputDataFilePath, separatorChar, hasHeader);
        }



        /// <summary>
        /// Save a model at the specified path.
        /// </summary>
        /// <param name="mlContext">The common context for all ML.NET operations.</param>
        /// <param name="model">Model to save.</param>
        /// <param name="data">IDataView used to train the model.</param>
        /// <param name="modelSavePath">File path for saving the model. Should be similar to "C:\YourPath\ModelName.mlnet.</param>
        public static void SaveModel(MLContext mlContext, ITransformer model, IDataView data, string modelSavePath)
        {
            // Pull the data schema from the IDataView used for training the model
            DataViewSchema dataViewSchema = data.Schema;

            using (var fs = File.Create(modelSavePath))
            {
                mlContext.Model.Save(model, dataViewSchema, fs);
            }
        }


        /// <summary>
        /// Retrains model using the pipeline generated as part of the training process.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="trainData"></param>
        /// <returns></returns>
        public static ITransformer RetrainModel(MLContext mlContext, IDataView trainData)
        {
            var pipeline = BuildPipeline(mlContext);
            var model = pipeline.Fit(trainData);

            return model;
        }


        /// <summary>
        /// build the pipeline that is used from model builder. Use this function to retrain model.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations
            var pipeline = mlContext.Transforms.Categorical.OneHotEncoding(new []{new InputOutputColumnPair(@"checking_status", @"checking_status"),new InputOutputColumnPair(@"credit_history", @"credit_history"),new InputOutputColumnPair(@"savings_status", @"savings_status"),new InputOutputColumnPair(@"employment", @"employment"),new InputOutputColumnPair(@"personal_status", @"personal_status"),new InputOutputColumnPair(@"other_parties", @"other_parties"),new InputOutputColumnPair(@"property_magnitude", @"property_magnitude"),new InputOutputColumnPair(@"other_payment_plans", @"other_payment_plans"),new InputOutputColumnPair(@"housing", @"housing"),new InputOutputColumnPair(@"job", @"job"),new InputOutputColumnPair(@"own_telephone", @"own_telephone"),new InputOutputColumnPair(@"foreign_worker", @"foreign_worker")}, outputKind: OneHotEncodingEstimator.OutputKind.Indicator)      
                                    .Append(mlContext.Transforms.ReplaceMissingValues(new []{new InputOutputColumnPair(@"duration", @"duration"),new InputOutputColumnPair(@"credit_amount", @"credit_amount"),new InputOutputColumnPair(@"installment_commitment", @"installment_commitment"),new InputOutputColumnPair(@"residence_since", @"residence_since"),new InputOutputColumnPair(@"age", @"age"),new InputOutputColumnPair(@"existing_credits", @"existing_credits"),new InputOutputColumnPair(@"num_dependents", @"num_dependents")}))      
                                    .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName:@"purpose",outputColumnName:@"purpose"))      
                                    .Append(mlContext.Transforms.Concatenate(@"Features", new []{@"checking_status",@"credit_history",@"savings_status",@"employment",@"personal_status",@"other_parties",@"property_magnitude",@"other_payment_plans",@"housing",@"job",@"own_telephone",@"foreign_worker",@"duration",@"credit_amount",@"installment_commitment",@"residence_since",@"age",@"existing_credits",@"num_dependents",@"purpose"}))      
                                    .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:@"class",inputColumnName:@"class",addKeyValueAnnotationsAsText:false))      
                                    .Append(mlContext.MulticlassClassification.Trainers.LightGbm(new LightGbmMulticlassTrainer.Options(){NumberOfLeaves=4,NumberOfIterations=1873,MinimumExampleCountPerLeaf=20,LearningRate=0.9999997766729865,LabelColumnName=@"class",FeatureColumnName=@"Features",ExampleWeightColumnName=null,Booster=new GradientBooster.Options(){SubsampleFraction=0.9999997766729865,FeatureFraction=0.99999999,L1Regularization=2E-10,L2Regularization=0.47105816489190666},MaximumBinCountPerFeature=266}))      
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName:@"PredictedLabel",inputColumnName:@"PredictedLabel"));

            return pipeline;
        }
    }
 }