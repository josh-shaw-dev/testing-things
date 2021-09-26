using AutoFixture.Xunit2;

namespace TestingThingsTests.Common
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {

        public AutoMoqDataAttribute() : base(FixtureFactory.Create)
        {
        }
    }
}
