using Serilog;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TestReporter.SpecFlow.Tool.Helpers.Features;
using TestReporter.SpecFlow.Tool.Models.Attributes;
using TestReporter.SpecFlow.Tool.Helpers.StepDefinitions;

namespace TestReporter.SpecFlow.Tool.Helpers
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
                .GroupBy(x => x.Value, baseStep =>
                {
                    var attributeUsageDetails =  new AttributeInformationDetailed
                    {
                        Type = baseStep.Type,
                        Value = baseStep.Value,
                        NumberOfCalls = stepDefinitionsGeneratedInfo.Count(x => Regex.IsMatch(x.Value, baseStep.Value))
                    };

                    Log.Information("Extracted attribute usage information: {@AttributeUsage}", attributeUsageDetails);    
                    
                    return attributeUsageDetails;
                }).SelectMany(x => x.ToList());
        }
    }
}