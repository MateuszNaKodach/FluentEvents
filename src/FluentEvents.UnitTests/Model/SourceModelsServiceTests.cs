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
        public void GetOrCreateSourceModel_WhenSourceModelDoesNotExists_ShouldCreate()
        {
            var sourceModel = _sourceModelsService.GetOrCreateSourceModel(typeof(object));

            Assert.That(_sourceModelsService.GetSourceModels(), Has.Exactly(1).Items);
            Assert.That(_sourceModelsService.GetSourceModels(), Has.Exactly(1).Items.EqualTo(sourceModel));
        }

        [Test]
        public void GetOrCreateSourceModel_WhenSourceModelExists_ShouldNotCreate()
        {
            _sourceModelsService.GetOrCreateSourceModel(typeof(object));
            var sourceModel = _sourceModelsService.GetOrCreateSourceModel(typeof(object));

            Assert.That(_sourceModelsService.GetSourceModels(), Has.Exactly(1).Items);
            Assert.That(_sourceModelsService.GetSourceModels(), Has.Exactly(1).Items.EqualTo(sourceModel));
        }

        [Test]
        public void GetOrCreateSourceModel_WhenSourceModelExists_ShouldReturn()
        {
            var createdSourceModel = _sourceModelsService.GetOrCreateSourceModel(typeof(object));
            var returnedSourceModel = _sourceModelsService.GetSourceModel(typeof(object));

            Assert.That(returnedSourceModel, Is.Not.Null);
            Assert.That(returnedSourceModel, Is.EqualTo(createdSourceModel));
        }

        [Test]
        public void GetOrCreateSourceModel_WhenSourceModelDoesNotExists_ShouldReturnNull()
        {
            var returnedSourceModel = _sourceModelsService.GetSourceModel(typeof(object));

            Assert.That(returnedSourceModel, Is.Null);
        }
    }
}
