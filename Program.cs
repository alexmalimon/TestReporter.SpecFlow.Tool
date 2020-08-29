using System;
using Serilog;
using System.IO;
using System.Linq;
using CommandLine;
using TestReporter.SpecFlow.Tool.Constants;
using TestReporter.SpecFlow.Tool.Helpers.Calls;
using TestReporter.SpecFlow.Tool.Models.Console;
using TestReporter.SpecFlow.Tool.Helpers.Reports;

namespace TestReporter.SpecFlow.Tool
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ConsoleArguments>(args).WithParsed(parsed =>
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console(outputTemplate:
                        "[{Timestamp:G}] [{Level}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();

                if (!Directory.Exists(parsed?.ProjectFolder))
                {
                    Log.Error("Features directory not found: {Directory}.", parsed?.ProjectFolder);
                    Environment.Exit(1);
                }

                if (!File.Exists(ApplicationConstants.ReportTemplatePath))
                {
                    Log.Error("File not found: {File}.", ApplicationConstants.ReportTemplatePath);
                    Environment.Exit(1);
                }

                var projectFile = Directory.GetFiles(parsed?.ProjectFolder,
                        ApplicationConstants.ProjectFileExtension, SearchOption.AllDirectories)
                    .Select(Path.GetFullPath)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(projectFile))
                {
                    Log.Error("*.csproj file has not been found.");
                    throw new FileNotFoundException("*.csproj file has not been found.");
                }

                ApplicationConstants.ProjectName = Path.GetFileNameWithoutExtension(projectFile);

                var stepDefinitionDetails =
                    Directory.GetFiles(parsed?.ProjectFolder,
                            ApplicationConstants.StepDefinitionFileExtension, SearchOption.AllDirectories)
                        .Select(Path.GetFullPath)
                        .ToList();

                Log.Information("Found {Count} step definition files.", stepDefinitionDetails.Count);

                var featuresCsDetails =
                    Directory.GetFiles(parsed?.ProjectFolder,
                            ApplicationConstants.FeatureCSharpFileExtension, SearchOption.AllDirectories)
                        .Select(Path.GetFullPath)
                        .ToList();

                Log.Information("Found {Count} generated feature code files.", featuresCsDetails.Count);

                var stepDefinitionCallInformation =
                    StepDefinitionCallCountHelper.CalculateNumberOfCalls(stepDefinitionDetails, featuresCsDetails)
                        .ToList();

                Log.Information("Found {Count} step definitions.", stepDefinitionCallInformation.Count);

                Log.Information("Staring generating HTML test report file.");

                var resultHtml = TestReportGenerator.GetHtmlReport(stepDefinitionCallInformation);

                Log.Information("Finished generating HTML test report file.");

                var testReportHtmlFile = string.Format(ApplicationConstants.GeneratedReportFilePathWithName,
                    ApplicationConstants.ProjectName);

                File.WriteAllText(testReportHtmlFile, resultHtml);

                var generatedReportFileFullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
                    testReportHtmlFile));

                Log.Information("Generated test report file path: {FilePath}", generatedReportFileFullPath);
            });
        }
    }
}