using System.Collections.Generic;
using TestReporter.SpecFlow.Tool.Models.StepDefinitions;

namespace TestReporter.SpecFlow.Tool.Models.Attributes
{
    public class AttributeInformationDetailed : AttributeInformation
    {
        public int NumberOfCalls { get; set; }

        public IEnumerable<StepDetails> GeneratedStepDefinitions { get; set; }
    }
}