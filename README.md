# TestReporter.SpecFlow.Tool

TestReporter.SpecFlow.Tool is a .NET Core Global Tool uesd to generate HTML test report file for [SpecFlow](https://specflow.org/).

## Installation

```bash
dotnet tool install --global TestReporter.SpecFlow.Tool
```

## Usage

```text
Arguments: 

  -p, --project    Required. Path to the SpecFlow project folder

  --help           Display this help screen.

  --version        Display version information.
```

## Example

```text
specflow-report --features "Features" --steps "Steps" 

specflow-report --project "Test Project" --features "Features" --steps "Steps"
```