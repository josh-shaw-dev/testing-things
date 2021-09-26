using AutoFixture.Xunit2;
using Xunit.Sdk;

namespace TestingThingsTests.Common
{
    public class InlineAutoMoqDataAttribute : CompositeDataAttribute
    {
        public InlineAutoMoqDataAttribute(params object[] arguments) : base(
            new DataAttribute[]
            {
                new InlineAutoDataAttribute(arguments),
                new AutoMoqDataAttribute()
            })
        {
        }
    }
}
