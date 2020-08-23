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
                StepDefinitionHelper.ExtractInformationFromFiles(stepPaths);

            var stepDefinitionsGeneratedInfo =
                CSharpFeatureHelper.ExtractInformationFromFiles(featureCsPaths);

            return stepDefinitionsInfo
                .GroupBy(x => x.Value, baseStep =>

                    new AttributeInformationDetailed
                    {
                        Type = baseStep.Type,
                        Value = baseStep.Value,
                        NumberOfCalls = stepDefinitionsGeneratedInfo.Count(x => Regex.IsMatch(x.Value, baseStep.Value))
                    }).SelectMany(x => x.ToList());
        }
    }
}