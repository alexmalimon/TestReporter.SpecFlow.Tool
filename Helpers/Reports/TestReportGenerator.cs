﻿using System;
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
        public static string GetHtmlReport(List<AttributeInformationDetailed> stepsCalls) =>
            Engine.Razor.RunCompile(File.ReadAllText(ApplicationConstants.ReportTemplatePath),
                ApplicationConstants.TestReportTemplateKey, typeof(ReportDetails),
                new ReportDetails
                {
                    TotalNumberOfSteps = stepsCalls.Count,
                    ProjectName = ApplicationConstants.ProjectName,
                    GeneratedDateTime = DateTime.UtcNow.ToString("g"),
                    SpecFlowIconPath = ApplicationConstants.SpecFlowIconPath,
                    BootstrapLibraryPath = ApplicationConstants.BootstrapLibraryPath,
                    TotalNumberOfUnusedSteps = stepsCalls.Count(x => x.NumberOfCalls == 0),
                    Results = stepsCalls.GroupBy(x => x.Type)
                        .Select(x => new ReportResult
                        {
                            Type = x.Key,
                            Attributes = x.ToList(),
                            TotalNumberSteps = x.Count(),
                            NumberOfUnusedSteps = x.Count(s => s.NumberOfCalls == 0)
                        })
                });
    }
}