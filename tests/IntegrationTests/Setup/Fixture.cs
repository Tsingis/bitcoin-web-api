
using Common;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using IntegrationTests.Setup;
using Xunit;

[assembly: AssemblyFixture(typeof(Fixture))]

namespace IntegrationTests.Setup;

public class Fixture : IAsyncLifetime
{
    private readonly IContainer? _wireMockContainer;

    public Fixture()
    {
        DotNetEnv.Env.TraversePath().Load();

        if (EnvVarAccessors.UseMocking)
        {
            var network = new NetworkBuilder()
                .WithName(Guid.NewGuid().ToString("N"))
                .Build();

            _wireMockContainer = new ContainerBuilder()
                .WithImage("wiremock/wiremock:3x-alpine")
                .WithName("wiremock")
                .WithCleanUp(true)
                .WithAutoRemove(true)
                .WithNetwork(network)
                .WithPortBinding(8080, true)
                .WithBindMount(Path.Join(AppContext.BaseDirectory, "wiremock"), "/home/wiremock/mappings")
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(request => request
                        .ForPort(8080)
                        .ForPath("/__admin/mappings")
                        .ForStatusCode(System.Net.HttpStatusCode.OK)))
                .Build();

            TestContext.Current.SendDiagnosticMessage("Creating Wiremock container");
        }
    }

    public async ValueTask InitializeAsync()
    {
        if (_wireMockContainer is not null)
        {
            var ct = TestContext.Current.CancellationToken;
            TestContext.Current.SendDiagnosticMessage("Starting Wiremock container");
            await _wireMockContainer.StartAsync(ct).ConfigureAwait(false);

            if (EnvVarAccessors.ShowMockingLogs)
            {
                var (Stdout, _) = await _wireMockContainer.GetLogsAsync(ct: ct).ConfigureAwait(false);
                TestContext.Current.SendDiagnosticMessage($"[WireMock] {Stdout}");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_wireMockContainer is not null)
        {
            var ct = TestContext.Current.CancellationToken;
            TestContext.Current.SendDiagnosticMessage("Stopping Wiremock container");
            await _wireMockContainer.StopAsync(ct).ConfigureAwait(false);
            await _wireMockContainer.DisposeAsync().ConfigureAwait(false);
        }
        GC.SuppressFinalize(this);
    }

    public ushort? GetPort()
    {
        return _wireMockContainer?.GetMappedPublicPort();
    }
}
