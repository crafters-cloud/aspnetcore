using FluentAssertions;
using NUnit.Framework;

namespace CraftersCloud.AspNetCore.CopyAzureKeyVaultSecrets.HostingStartup.Tests
{
    [Category("unit")]
    public class SmokeTest
    {
        [Test]
        public void Test()
        {
            1.Should().Be(1);
        }
    }
}
