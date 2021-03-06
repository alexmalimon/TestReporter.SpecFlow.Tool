﻿using System;
using Serilog;
using System.IO;
using System.Linq;
using CommandLine;
using System.Diagnostics;
using TestReporter.SpecFlow.Tool.Constants;
using TestReporter.SpecFlow.Tool.Models.Report;
using TestReporter.SpecFlow.Tool.Helpers.Calls;
using TestReporter.SpecFlow.Tool.Models.Console;
using TestReporter.SpecFlow.Tool.Helpers.Reports;
using TestReporter.SpecFlow.Tool.Helpers.Features;
using TestReporter.SpecFlow.Tool.Helpers.StepDefinitions;

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
                    Environment.Exit(1);
                }

                var stopwatch = Stopwatch.StartNew();

                var projectDirectories = Directory.GetDirectories(parsed?.ProjectFolder)
                    .Where(d =>
                        !ApplicationConstants.ExcludeDirectories.Contains(new DirectoryInfo(d).Name,
                            StringComparer.InvariantCultureIgnoreCase))
                    .ToList();

                var stepPaths = projectDirectories.SelectMany(d =>
                    Directory.GetFiles(d, ApplicationConstants.StepDefinitionFileExtension, SearchOption.AllDirectories)
                        .Where(p => !p.EndsWith(".feature.cs", StringComparison.InvariantCultureIgnoreCase))
                        .Select(Path.GetFullPath)).ToList();

                Log.Information("Found {Count} code files.", stepPaths.Count);

                var featureCsPaths = projectDirectories.SelectMany(d =>
                    Directory.GetFiles(d, ApplicationConstants.FeatureCSharpFileExtension, SearchOption.AllDirectories)
                        .Where(p => p.EndsWith(".feature.cs", StringComparison.InvariantCultureIgnoreCase))
                        .Select(Path.GetFullPath)).ToList();

                Log.Information("Found {Count} generated feature code files.", featureCsPaths.Count);

                var stepDefinitionsInfo =
                    StepDefinitionHelper.ExtractInformationFromFiles(stepPaths).ToList();

                Log.Information("Finished extracting information about step definitions");

                var stepDefinitionsGeneratedInfo =
                    CSharpFeatureHelper.ExtractInformationFromFiles(featureCsPaths).ToList();

                Log.Information("Finished extracting information about generated feature's code");

                var stepDefinitionCallInformation =
                    StepDefinitionCallCountHelper
                        .CalculateNumberOfCalls(stepDefinitionsInfo, stepDefinitionsGeneratedInfo).ToList();

                if (!stepDefinitionCallInformation.Any())
                {
                    Log.Error("No step definitions have been found.");
                    Environment.Exit(0);
                }

                Log.Information("Found {Count} step definitions.", stepDefinitionCallInformation.Count);

                Log.Information("Staring generating HTML test report file.");

                var reportSettings = new ReportSettings
                {
                    MaterialIcons = ApplicationConstants.MaterialIcons,
                    GeneratedDateTime = DateTime.UtcNow.ToString("g"),
                    ProjectName = Path.GetFileNameWithoutExtension(projectFile),
                    SpecFlowIconPath = ApplicationConstants.SpecFlowIconPathGithubUrl,
                    MaterialJsLibraryPath = ApplicationConstants.MaterialJsLibraryPath,
                    MaterialCssLibraryPath = ApplicationConstants.MaterialCssLibraryPath
                };

                var resultHtml = TestReportGenerator.GetHtmlReport(stepDefinitionCallInformation, reportSettings);

                Log.Information("Finished generating HTML test report file.");

                var testReportHtmlFileName = string.Format(ApplicationConstants.GeneratedReportFilePathWithName,
                    reportSettings.ProjectName);

                var testReportOutputDirectory = parsed?.TestReportDirectory ?? Directory.GetCurrentDirectory();

                if (!Directory.Exists(testReportOutputDirectory))
                {
                    Directory.CreateDirectory(testReportOutputDirectory);
                }

                var testReportFullPath = Path.GetFullPath(testReportOutputDirectory);
                
                var testReportHtmlFilePath = Path.Combine(testReportFullPath, testReportHtmlFileName);

                Log.Information("Saving generated test report.");

                File.WriteAllText(testReportHtmlFilePath, resultHtml);

                Log.Information("Generated test report file path: {FilePath}", testReportHtmlFilePath);
                
                stopwatch.Stop();

                var elapsed = stopwatch.Elapsed.ToString("hh\\:mm\\:ss\\.ff");

                Log.Information("Elapsed time: {ElapsedTime}", elapsed);
            });
        }
    }
}