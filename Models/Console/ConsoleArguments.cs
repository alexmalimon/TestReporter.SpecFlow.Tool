﻿using CommandLine;

namespace TestReporter.SpecFlow.Tool.Models.Console
{
    public class ConsoleArguments
    {
        [Option('p', "project", Required = true, HelpText = "Path to the SpecFlow project folder")]
        public string ProjectFolder { get; set; }

        [Option('o', "output", Required = false, HelpText = "Path to directory, where test report file will be saved")]
        public string TestReportDirectory { get; set; }

        [Option('g', "global", Required = false, Default = false, HelpText = "Specifies whether to use global or local paths to libraries")]
        public bool Global { get; set; }
    }
}