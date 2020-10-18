using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TestReporter.SpecFlow.Tool.Models.Attributes;
using TestReporter.SpecFlow.Tool.Models.StepDefinitions;

namespace TestReporter.SpecFlow.Tool.Helpers.Calls
{
    public static class StepDefinitionCallCountHelper
    {
        public static IEnumerable<AttributeInformationDetailed> CalculateNumberOfCalls(
            List<AttributeInformation> stepDefinitionsInfo, List<AttributeInformation> stepDefinitionsGeneratedInfo) =>
            stepDefinitionsInfo.GroupBy(x => x.Value, baseStep =>
            {
                var matchedSteps = stepDefinitionsGeneratedInfo
                    .Where(x => Regex.IsMatch(x.Value, baseStep.Value))
                    .ToList();

                return new AttributeInformationDetailed
                {
                    Type = baseStep.Type,
                    Value = baseStep.Value,
                    NumberOfCalls = matchedSteps.Count,
                    GeneratedStepDefinitions = matchedSteps.GroupBy(ms => new { ms.FeatureFileName, ms.Value })
                        .Select(x => new StepDetails
                        {
                            StepName = x.Key.Value,
                            NumberCallsInFeatureFile = x.Count(),
                            FeatureFileName = x.Key.FeatureFileName
                        })
                };
            }).SelectMany(x => x.ToList());
    }
}