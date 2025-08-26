using AutoFixture;
using AutoFixture.Xunit2;
using Testing.Shared;

namespace Kevin.Testing.Shared.Attributes
{
    public class AutoFakeItEasyAttribute: AutoDataAttribute
    {
        //AutoFakeItEasyCustomization can create implementation of interfaces and abstract classes using FakeItEasy
        public AutoFakeItEasyAttribute()
            : base(CreateFixture)
        {
        }

        private static IFixture CreateFixture()
        {
            Fixture fixture = new Fixture();
            fixture.Customize(new AutoFixture.AutoFakeItEasy.AutoFakeItEasyCustomization());
            fixture.Customizations.Add(new CancellationTokenGenerator());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }
    }
}
