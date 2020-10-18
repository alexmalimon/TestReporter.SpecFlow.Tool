using System.Collections.Generic;
using TestReporter.SpecFlow.Tool.Models.Attributes;

namespace TestReporter.SpecFlow.Tool.Models.Report
{
    public class ReportResult
    {
        public string Type { get; set; }

        public List<AttributeInformationDetailed> Attributes { get; set; }
    }
}