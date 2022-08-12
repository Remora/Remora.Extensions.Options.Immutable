Remora.Extensions.Options.Immutable
===================================
This package provides an extension to `Microsoft.Extensions.Options`, allowing 
the use of immutable types (such as `record`s) in the options ecosystem.

The extension takes a simple and direct approach to integration with the 
existing Microsoft-provided extension, adding only the minimal API necessary to
achieve feature parity.

# Usage
Usage is simple - immutable option types may be configured in a practically 
identical manner to mutable types, provided one of the following conditions are 
true:

  * The type defines a parameterless constructor
  * The type defines a constructor where all arguments are optional
  * The type is explicitly initialized with a root state

That is, given the following types,

```c#
public record ExplicitOptions(string Value, bool Flag);
public record ParameterlessOptions()
{
    public string? Value { get; init; }
}
public record AllOptionalOptions(string Value = "initial", bool Flag = true");
```

they may be utilized in the following manner:

```c#
var services = new ServiceCollection()
    .Configure(() => new ExplicitOptions("initial", true))
    .Configure<ExplicitOptions>(opt => opt with { Flag = false });

var services = new ServiceCollection()
    .Configure<ParameterlessOptions>(opt => opt with { Value = "configured" });

var services = new ServiceCollection()
    .Configure<AllOptionalOptions>(opt => opt with { Flag = false });
```

All the various normal configuration calls, such as `Configure`, 
`PostConfigure`, `ConfigureAll`, and `PostConfigureAll` (along with their named)
variants are supported.

# Installation
Get it on [NuGet][1]!


[1]: https://www.nuget.org/packages/Remora.Extensions.Options.Immutable
