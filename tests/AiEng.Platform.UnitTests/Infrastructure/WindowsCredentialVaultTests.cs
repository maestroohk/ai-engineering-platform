using AiEng.Platform.Application.Infrastructure;
using AiEng.Platform.Infrastructure.Credentials;
using Xunit;

namespace AiEng.Platform.UnitTests.Infrastructure;

public class WindowsCredentialVaultTests
{
    [Fact]
    public void Constructor_does_not_throw_on_Windows()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }
        var vault = new WindowsCredentialVault();
        Assert.NotNull(vault);
    }

    [Fact]
    public async Task GetAsync_returns_null_when_credential_is_absent()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }
        var vault = new WindowsCredentialVault();
        var name = "aieng-missing-" + Guid.NewGuid().ToString("N");

        var result = await vault.GetAsync(name);

        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_then_GetAsync_round_trip_secret()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }
        var vault = new WindowsCredentialVault();
        var name = "aieng-roundtrip-" + Guid.NewGuid().ToString("N");
        try
        {
            await vault.SetAsync(name, "super-secret-value");

            var stored = await vault.GetAsync(name);

            Assert.Equal("super-secret-value", stored);
        }
        finally
        {
            await vault.DeleteAsync(name);
        }
    }

    [Fact]
    public async Task DeleteAsync_removes_an_existing_credential()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }
        var vault = new WindowsCredentialVault();
        var name = "aieng-delete-" + Guid.NewGuid().ToString("N");
        await vault.SetAsync(name, "to-be-deleted");
        Assert.Equal("to-be-deleted", await vault.GetAsync(name));

        await vault.DeleteAsync(name);

        Assert.Null(await vault.GetAsync(name));
    }

    [Fact]
    public async Task DeleteAsync_is_a_no_op_when_credential_is_absent()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }
        var vault = new WindowsCredentialVault();
        var name = "aieng-missing-delete-" + Guid.NewGuid().ToString("N");

        await vault.DeleteAsync(name);
    }

    [Fact]
    public async Task SetAsync_throws_when_name_is_null()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }
        var vault = new WindowsCredentialVault();
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await vault.SetAsync(null!, "x"));
    }

    [Fact]
    public async Task SetAsync_throws_when_secret_is_null()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }
        var vault = new WindowsCredentialVault();
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await vault.SetAsync("name", null!));
    }

    [Fact]
    public async Task SetAsync_throws_on_non_Windows()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }
        var vault = new WindowsCredentialVault();

        await Assert.ThrowsAsync<PlatformNotSupportedException>(
            async () => await vault.SetAsync("name", "secret"));
    }

    [Fact]
    public async Task GetAsync_throws_on_non_Windows()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }
        var vault = new WindowsCredentialVault();

        await Assert.ThrowsAsync<PlatformNotSupportedException>(
            async () => await vault.GetAsync("name"));
    }

    [Fact]
    public async Task DeleteAsync_throws_on_non_Windows()
    {
        if (OperatingSystem.IsWindows())
        {
            return;
        }
        var vault = new WindowsCredentialVault();

        await Assert.ThrowsAsync<PlatformNotSupportedException>(
            async () => await vault.DeleteAsync("name"));
    }
}
