﻿using CommandLine;

namespace TestReporter.SpecFlow.Tool.Models.Console
{
    public class ConsoleArguments
    {
        [Option('p', "project", Required = true, HelpText = "Path to the SpecFlow project folder")]
        public string ProjectFolder { get; set; }
    }
}