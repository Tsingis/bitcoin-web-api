using Shouldly;
using Xunit;

namespace ArchitectureTests;

public class AssemblyReferenceTests
{

    [Fact]
    public void Common_NoUnexpectedAssemblyReferences()
    {
        var commonAssembly = typeof(Common.EnvVarKeys).Assembly;

        var referencedAssemblies = commonAssembly
            .GetReferencedAssemblies()
            .Select(a => a.Name)
            .ToList();

        var allowed = new[]
        {
                "System",
                "mscorlib",
                "netstandard",
                "System.Core",
                "Microsoft.CSharp",
                commonAssembly.GetName().Name!
            };

        var forbidden = referencedAssemblies
            .Where(x => !allowed.Any(y => x?.StartsWith(y, StringComparison.InvariantCulture) ?? false))
            .ToList();

        forbidden.ShouldBeEmpty();
    }
}
