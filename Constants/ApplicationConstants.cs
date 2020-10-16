using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace TestReporter.SpecFlow.Tool.Constants
{
    public static class ApplicationConstants
    {
        private static string PackageDirectoryPath { get; } =
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string ProjectFileExtension { get; } = "*.csproj";

        public static string TestReportTemplateKey { get; } = "TestReport";

        public static string StepDefinitionFileExtension { get; } = "*.cs";

        public static string ExcludeExamplePattern { get; } = "string.Format";

        public static string FeatureCSharpFileExtension { get; } = "*.feature.cs";

        public static string GeneratedReportFilePathWithName { get; } = "Test Report - {0}.html";

        public static IEnumerable<string> ExcludeDirectories { get; } =
            new[] { "bin", "obj" };

        public static IEnumerable<string> StepDefinitionAttributeMethods { get; } =
            new[] { "Given", "When", "Then" };

        public static IEnumerable<string> GeneratedStepDefinitionMethods { get; } =
            new[] { "When", "Given", "Then", "And", "But" };

        public static string ReportTemplatePath { get; } =
            Path.Combine(PackageDirectoryPath, "Report", "Template.cshtml");

        public static string MaterialIcons { get; } =
            "https://fonts.googleapis.com/icon?family=Material+Icons";

        public static string MaterialCssLibraryPath { get; } =
            "https://cdnjs.cloudflare.com/ajax/libs/materialize/1.0.0/css/materialize.min.css";

        public static string MaterialJsLibraryPath { get; } =
            "https://cdnjs.cloudflare.com/ajax/libs/materialize/1.0.0/js/materialize.min.js";

        public static string SpecFlowIconPathGithubUrl { get; } =
            "https://raw.githubusercontent.com/SpecFlowOSS/SpecFlow/master/specflow.ico";
    }
}