using System.Collections.Generic;

namespace TestReporter.SpecFlow.Tool.Models.Report
{
    public class ReportDetails
    {
        public string ProjectName { get; set; }

        public string GeneratedDateTime { get; set; }
        
        public IEnumerable<ReportResult> Results { get; set; }
    }
}