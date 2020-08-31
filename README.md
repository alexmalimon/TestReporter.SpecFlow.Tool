# TestReporter.SpecFlow.Tool

TestReporter.SpecFlow.Tool is a .NET Core Global Tool uesd to generate HTML test report file for [SpecFlow](https://specflow.org/).

## Installation

```text
dotnet tool install --global TestReporter.SpecFlow.Tool
```

## Usage

```text
Arguments:

  -p, --project    Path to the SpecFlow project folder

  -o, --output     Path to directory, where test report file will be saved

  --help           Display this help screen.

  --version        Display version information.
```

## Example

```text
specflow-report --project ".\TestProject"

specflow-report --project ".\TestProject" --output "D:\ReportOutput"
```