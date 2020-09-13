using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace TestReporter.SpecFlow.Tool.Constants
{
    public static class ApplicationConstants
    {
        private static string PackageDirectoryPath { get; } =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string TestReportTemplateKey { get; } = "TestReport";

        public static string StepDefinitionFileExtension { get; } = "*.cs";

        public static string ProjectFileExtension { get; } = "*.csproj";

        public static string FeatureCSharpFileExtension { get; } = "*.feature.cs";

        public static string ExcludeExamplePattern { get; } = "string.Format";

        public static string GeneratedReportFilePathWithName { get; } = "Test Report - {0}.html";

        public static IEnumerable<string> StepDefinitionAttributeMethods { get; } = new[] { "Given", "When", "Then" };

        public static IEnumerable<string> GeneratedStepDefinitionMethods { get; } =
            new[] { "When", "Given", "Then", "And", "But" };

        public static IEnumerable<string> ExcludeDirectories { get; } = new[] { "bin", "obj" };

        public static string BootstrapLibraryPathLocal { get; } =
            Path.Combine(PackageDirectoryPath, "Report\\lib\\bootstrap.min.css");

        public static string SpecFlowIconPathLocal { get; } =
            Path.Combine(PackageDirectoryPath, "Report\\icon\\specflow-icon.ico");

        public static string BootstrapLibraryCdnUrl { get; } =
            "https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css";

        public static string SpecFlowIconPathGithubUrl { get; } =
            "https://raw.githubusercontent.com/SpecFlowOSS/SpecFlow/master/specflow.ico";

        public static string ReportTemplatePath { get; } =
            Path.Combine(PackageDirectoryPath, "Report\\Template.cshtml");
    }
}