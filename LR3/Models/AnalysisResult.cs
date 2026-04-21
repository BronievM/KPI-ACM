using OxyPlot;
namespace LR3.Models
{
    internal class AnalysisResult
    {
        public PlotModel GraphModel { get; set; }
        public List<string> LogLines { get; set; } = new List<string>();
    }
}
