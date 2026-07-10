using AiEng.Platform.App.Components.Common;
using AiEng.Platform.App.Components.Display;
using Bunit;
using Xunit;

namespace AiEng.Platform.ComponentTests.Components.Display;

public class AppAvatarTests : BunitContext
{
    [Fact]
    public void Renders_Initials_Uppercased()
    {
        var cut = Render<AppAvatar>(parameters => parameters
            .Add(p => p.Initials, "ab"));

        Assert.Equal("AB", cut.Find(".app-avatar-initials").TextContent.Trim());
    }

    [Fact]
    public void Truncates_Initials_To_Two_Characters()
    {
        var cut = Render<AppAvatar>(parameters => parameters
            .Add(p => p.Initials, "abcdef"));

        Assert.Equal("AB", cut.Find(".app-avatar-initials").TextContent.Trim());
    }

    [Fact]
    public void Each_Size_Renders_Matching_Class()
    {
        var sizes = new (AppAvatarSize size, string expectedClass)[]
        {
            (AppAvatarSize.Small, "app-avatar-size-small"),
            (AppAvatarSize.Medium, "app-avatar-size-medium"),
            (AppAvatarSize.Large, "app-avatar-size-large")
        };

        foreach (var (size, expectedClass) in sizes)
        {
            var cut = Render<AppAvatar>(parameters => parameters
                .Add(p => p.Initials, "ab")
                .Add(p => p.Size, size));

            Assert.Contains(expectedClass, cut.Find(".app-avatar").ClassList.ToString());
        }
    }

    [Fact]
    public void Renders_Img_Role_With_Aria_Label()
    {
        var cut = Render<AppAvatar>(parameters => parameters
            .Add(p => p.Initials, "ab"));

        var avatar = cut.Find(".app-avatar");
        Assert.Equal("img", avatar.GetAttribute("role"));
        Assert.Equal("Avatar: AB", avatar.GetAttribute("aria-label"));
    }
}
