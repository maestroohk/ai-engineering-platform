namespace AiEng.Platform.ArchitectureTests.A11y;

public class AxeCoreAuditTests
{
    [Fact(Skip = "Axe-core harness is not in the toolchain yet. The test is registered but disabled until M4-D per ADR-016. The M2.5 slice records the deferral; the activation milestone is M4-D.")]
    public void Axe_Core_Reports_Zero_Critical_Or_Serious_Violations_On_Every_Route()
    {
        var registeredRoutes = new[]
        {
            "/",
            "/counter",
            "/dashboard",
            "/design-system",
            "/error",
            "/not-found",
            "/weather"
        };

        Assert.NotEmpty(registeredRoutes);
    }

    [Fact(Skip = "Axe-core harness is not in the toolchain yet. The test is registered but disabled until M4-D per ADR-016. The M2.5 slice records the deferral; the activation milestone is M4-D.")]
    public void Axe_Core_Reports_Zero_Critical_Or_Serious_Violations_On_The_Layout()
    {
        Assert.True(true, "Disabled until M4-D; see ADR-016.");
    }

    [Fact(Skip = "Axe-core harness is not in the toolchain yet. The test is registered but disabled until M4-D per ADR-016. The M2.5 slice records the deferral; the activation milestone is M4-D.")]
    public void Axe_Core_Reports_Zero_Critical_Or_Serious_Violations_On_The_Design_System()
    {
        Assert.True(true, "Disabled until M4-D; see ADR-016.");
    }
}
