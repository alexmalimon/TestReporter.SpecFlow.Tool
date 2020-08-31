using System;
using Serilog;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TestReporter.SpecFlow.Tool.Helpers.Features;
using TestReporter.SpecFlow.Tool.Models.Attributes;
using TestReporter.SpecFlow.Tool.Models.StepDefinitions;
using TestReporter.SpecFlow.Tool.Helpers.StepDefinitions;

namespace TestReporter.SpecFlow.Tool.Helpers.Calls
{
    public static class StepDefinitionCallCountHelper
    {
        public static IEnumerable<AttributeInformationDetailed> CalculateNumberOfCalls(IEnumerable<string> stepPaths,
            IEnumerable<string> featureCsPaths)
        {
            var stepDefinitionsInfo =
                StepDefinitionHelper.ExtractInformationFromFiles(stepPaths).ToList();

            Log.Information("Finished extracting information about step definitions");

            var stepDefinitionsGeneratedInfo =
                CSharpFeatureHelper.ExtractInformationFromFiles(featureCsPaths).ToList();

            Log.Information("Finished extracting information about generated feature's code");

            return stepDefinitionsInfo
                .GroupBy(x => x.Value, baseStep => new AttributeInformationDetailed
                {
                    Type = baseStep.Type,
                    Value = baseStep.Value,
                    NumberOfCalls = stepDefinitionsGeneratedInfo.Count(x => Regex.IsMatch(x.Value, baseStep.Value)),
                    StepId = Guid.NewGuid().ToString("N"),
                    GeneratedStepDefinitions = stepDefinitionsGeneratedInfo
                        .Where(x => Regex.IsMatch(x.Value, baseStep.Value))
                        .Select(x => new StepDetails
                        {
                            FeatureFileName = x.FeatureFileName,
                            FeatureFilePath = x.FeatureFilePath,
                            StepName = x.Value
                        })
                }).SelectMany(x => x.ToList());
        }
    }
}