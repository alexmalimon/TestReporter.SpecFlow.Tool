using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace TestReporter.SpecFlow.Tool.Constants
{
    public static class ApplicationConstants
    {
        public static string ProjectName { get; set; }

        public static string TestReportTemplateKey { get; } = "TestReport";

        public static string StepDefinitionFileExtension { get; } = "*.cs";
        
        public static string FeatureCSharpFileExtension { get; } = "*.feature.cs";
        
        public static string ReportTemplatePath { get; } = 
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Report\\Template.cshtml");
        
        public static string GeneratedReportFilePathWithName { get; } = "TestReport.html";

        public static IEnumerable<string> StepDefinitionAttributeMethods { get; } = new[] { "Given", "When", "Then" };

        public static IEnumerable<string> GeneratedStepDefinitionMethods { get; } = new[] { "When", "Given", "Then", "And" };
    }
}