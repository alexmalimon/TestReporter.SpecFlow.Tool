using System;
using System.IO;
using System.Linq;
using RazorEngine;
using RazorEngine.Templating;
using System.Collections.Generic;
using TestReporter.SpecFlow.Tool.Constants;
using TestReporter.SpecFlow.Tool.Models.Report;
using TestReporter.SpecFlow.Tool.Models.Attributes;

namespace TestReporter.SpecFlow.Tool.Helpers.Reports
{
    public static class TestReportGenerator
    {
        public static string GetHtmlReport(List<AttributeInformationDetailed> stepsCalls,
            ReportSettings reportSettings) =>
            Engine.Razor.RunCompile(
                string.Join(Environment.NewLine, File.ReadAllLines(ApplicationConstants.ReportTemplatePath).Skip(1)),
                ApplicationConstants.TestReportTemplateKey, typeof(ReportDetails),
                new ReportDetails
                {
                    TotalNumberOfSteps = stepsCalls.Count,
                    ProjectName = reportSettings.ProjectName,
                    MaterialIcons = reportSettings.MaterialIcons,
                    SpecFlowIconPath = reportSettings.SpecFlowIconPath,
                    GeneratedDateTime = reportSettings.GeneratedDateTime,
                    MaterialJsLibraryPath = reportSettings.MaterialJsLibraryPath,
                    MaterialCssLibraryPath = reportSettings.MaterialCssLibraryPath,
                    TotalNumberOfUnusedSteps = stepsCalls.Count(x => x.NumberOfCalls == 0),
                    Results = stepsCalls.GroupBy(x => x.Type)
                        .Select(x => new ReportResult
                        {
                            Type = x.Key,
                            Attributes = x.ToList()
                        })
                });
    }
}