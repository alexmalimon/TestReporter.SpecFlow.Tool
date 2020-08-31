using System.Collections.Generic;

namespace TestReporter.SpecFlow.Tool.Models.Report
{
    public class ReportDetails
    {
        public string ProjectName { get; set; }

        public string GeneratedDateTime { get; set; }

        public string BootstrapLibraryPath { get; set; }

        public string SpecFlowIconPath { get; set; }

        public int TotalNumberOfSteps { get; set; }

        public int TotalNumberOfUnusedSteps { get; set; }

        public IEnumerable<ReportResult> Results { get; set; }
    }
}