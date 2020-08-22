﻿using System.IO;
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
                if (!Directory.Exists(parsed?.FeaturesFolderPath))
                {
                    throw new DirectoryNotFoundException(parsed?.FeaturesFolderPath);
                }
                
                if (!Directory.Exists(parsed?.StepsFolderPath))
                {
                    throw new DirectoryNotFoundException(parsed?.StepsFolderPath);
                }

                ApplicationConstants.ProjectName = parsed?.ProjectName;

                var stepDefinitionDetails = Directory
                    .GetFiles(parsed?.StepsFolderPath, "*.cs")
                    .Select(Path.GetFullPath);
            
                var featuresCsDetails = Directory
                    .GetFiles(parsed?.FeaturesFolderPath, "*.feature.cs")
                    .Select(Path.GetFullPath);
           
                var stepDefinitionCallInformation = StepDefinitionCallCountHelper
                    .CalculateNumberOfCalls(stepDefinitionDetails, featuresCsDetails)
                    .ToList();

                var resultHtml = TestReportGenerator.GetHtmlReport(stepDefinitionCallInformation);
                File.WriteAllText(ApplicationConstants.GeneratedReportFilePathWithName, resultHtml);
            });
        }
    }
}