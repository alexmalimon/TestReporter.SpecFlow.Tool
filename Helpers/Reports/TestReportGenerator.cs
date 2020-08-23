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
        public static string GetHtmlReport(IEnumerable<AttributeInformationDetailed> stepsCalls) =>
            Engine.Razor.RunCompile(File.ReadAllText(ApplicationConstants.ReportTemplatePath),
                ApplicationConstants.TestReportTemplateKey, typeof(ReportDetails),
                new ReportDetails
                {
                    Results = stepsCalls.GroupBy(x => x.Type)
                        .Select(x => new ReportResult
                        {
                            Type = x.Key,
                            Attributes = x.ToList()
                        }),
                    ProjectName = ApplicationConstants.ProjectName,
                    GeneratedDateTime = DateTime.UtcNow.ToString("g")
                });
    }
}