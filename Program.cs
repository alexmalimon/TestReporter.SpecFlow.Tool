using System;
using Serilog;
using System.IO;
using System.Linq;
using CommandLine;
using TestReporter.SpecFlow.Tool.Helpers;
using TestReporter.SpecFlow.Tool.Constants;
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

                if (!Directory.Exists(parsed?.FeaturesFolderPath))
                {
                    Log.Error("Features directory not found: {Directory}.", parsed?.FeaturesFolderPath);
                    Environment.Exit(1);
                }
                
                if (!Directory.Exists(parsed?.StepsFolderPath))
                {
                    Log.Error("Step definitions directory not found: {Directory}.", parsed?.StepsFolderPath);
                    Environment.Exit(1);
                }

                if (!File.Exists(ApplicationConstants.ReportTemplatePath))
                {
                    Log.Error("File not found: {File}.", ApplicationConstants.ReportTemplatePath);
                    Environment.Exit(1);
                }

                ApplicationConstants.ProjectName = parsed?.ProjectName;

                var stepDefinitionDetails = 
                    Directory.GetFiles(parsed?.StepsFolderPath, 
                            ApplicationConstants.StepDefinitionFileExtension, SearchOption.AllDirectories)
                    .Select(Path.GetFullPath)
                    .ToList();
                
                Log.Information("Found {Count} step definition files.", stepDefinitionDetails.Count);

                var featuresCsDetails = 
                    Directory.GetFiles(parsed?.FeaturesFolderPath, 
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
                
                File.WriteAllText(ApplicationConstants.GeneratedReportFilePathWithName, resultHtml);

                var generatedReportFileFullPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
                    ApplicationConstants.GeneratedReportFilePathWithName));

                Log.Information("Generated test report file path: {FilePath}", generatedReportFileFullPath);
            });
        }
    }
}