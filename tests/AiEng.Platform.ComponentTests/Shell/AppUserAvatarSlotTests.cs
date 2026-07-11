using AiEng.Platform.App.Components.Shell;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Shell;

public class AppUserAvatarSlotTests : BunitContext
{
    [Fact]
    public void Renders_The_App_User_Avatar_Slot_Container()
    {
        var cut = Render<AppUserAvatarSlot>();

        Assert.NotNull(cut.Find(".app-user-avatar-slot"));
    }

    [Fact]
    public void Default_Initials_Are_Question_Mark()
    {
        var cut = Render<AppUserAvatarSlot>();

        Assert.Equal("?", cut.Find(".app-avatar").TextContent);
    }

    [Fact]
    public void Explicit_Initials_Are_Rendered()
    {
        var cut = Render<AppUserAvatarSlot>(parameters => parameters
            .Add(p => p.Initials, "ab"));

        Assert.Equal("AB", cut.Find(".app-avatar").TextContent);
    }

    [Fact]
    public void Default_Aria_Label_Is_Current_User()
    {
        var cut = Render<AppUserAvatarSlot>();

        Assert.Equal("Current user", cut.Find(".app-user-avatar-slot").GetAttribute("aria-label"));
    }
}
