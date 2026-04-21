using OxyPlot;

namespace LR4.Models
{
    public class AnalysisResult
    {
        public PlotModel GraphModel { get; set; }
        public List<string> LogLines { get; set; } = new List<string>();
    }
}
