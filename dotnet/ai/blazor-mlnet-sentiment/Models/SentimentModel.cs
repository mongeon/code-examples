using Microsoft.ML.Data;

namespace BlazorMLNET.Models
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string SentimentText { get; set; } = string.Empty;

        [LoadColumn(1), ColumnName("Label")]
        public bool Sentiment { get; set; }
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
