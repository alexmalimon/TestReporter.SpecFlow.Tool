﻿using System.Collections.Generic;

namespace TestReporter.SpecFlow.Tool.Constants
{
    public static class ApplicationConstants
    {
        public static string ProjectName { get; set; }

        public static string TestReportTemplateKey { get; } = "TestReport";

        public static string GeneratedReportFilePathWithName { get; } = "TestReport.html";

        public static IEnumerable<string> StepDefinitionAttributeMethods { get; } = new[] { "Given", "When", "Then" };

        public static IEnumerable<string> GeneratedStepDefinitionMethods { get; } = new[] { "When", "Given", "Then", "And" };

        public static string ReportTemplate { get; } = @"﻿<!DOCTYPE html><html lang=""en""><head><meta charset=""UTF-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css""><title>Step Definition Reporter</title><style>.report-text-small{font-size:14px}body{padding:10px}</style></head><body><h3> Step Definition Report @(!string.IsNullOrEmpty(Model.ProjectName) ? ""- "" + @Model.ProjectName : null)</h3><div class=""mb-3""> Generated by SpecFlow at @Model.GeneratedDateTime (see <a href=""http://www.specflow.org/"">http://www.specflow.org/</a>).</div> @foreach (var groupedSteps in @Model.Results) {<h4> @(groupedSteps.Type)</h4><table class=""table table-bordered table-hover""><thead><th class=""report-text-small"">Step Definition</th><th class=""report-text-small"">Instances</th></thead><tbody> @foreach (var stepInfo in groupedSteps.Attributes) {<tr class=""@(stepInfo.NumberOfCalls == 0 ? ""table-danger"" : null)""><td class=""report-text-small"">@(stepInfo.Value)</td><td class=""report-text-small"">@(stepInfo.NumberOfCalls)</td></tr> }</tbody></table> }</body></html>";
    }
}