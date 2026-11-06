# Text to Form Fields

An experiment in turning a plain-text description of a form into field definitions (label, name, type and options),
written in C# on .NET 6.

**Status: unfinished. This project doesn't compile yet.**

## What's in it

- **`TextToFields/Program.cs`:** a console prompt that asks for a description. It calls `generate_form_fields`, but
  that function and the other attempts in this file are commented out.
- **`TextToFields/FormField.cs`:** an attempt using [OpenNLP](https://www.nuget.org/packages/OpenNLP) to tag words in
  the description and pick out field types such as number, dropdown, radio, select, date, phone, email and password.

The commented-out code in `Program.cs` includes a regex version, which looks for a field type on each line and takes
options from `[square brackets]`.

## What's left to do

- Bring back one version of `generate_form_fields` in `Program.cs`.
- Fix `FormField.cs`, which mixes the APIs of two OpenNLP packages. The OpenNLP model files it loads aren't in this
  repo either, so they'd need adding as embedded resources.

To see the current errors:

```
dotnet build TextToFields.sln
```
