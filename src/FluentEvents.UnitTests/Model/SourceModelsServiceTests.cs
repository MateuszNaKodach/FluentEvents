using FluentEvents.Model;
using NUnit.Framework;

namespace FluentEvents.UnitTests.Model
{
    [TestFixture]
    public class SourceModelsServiceTests
    {
        private SourceModelsService _sourceModelsService;

        [SetUp]
        public void SetUp()
        {
            _sourceModelsService = new SourceModelsService();
        }

        [Test]
        public void GetOrCreateSourceModel_WhenSourceModelExists_ShouldReturnTheSameInstance()
        {
            var firstInvocationSourceModel = _sourceModelsService.GetOrCreateSourceModel(typeof(object));
            var secondInvocationSourceModel = _sourceModelsService.GetOrCreateSourceModel(typeof(object));

            Assert.That(secondInvocationSourceModel, Is.Not.Null);
            Assert.That(secondInvocationSourceModel, Is.EqualTo(firstInvocationSourceModel));
        }
    }
}
