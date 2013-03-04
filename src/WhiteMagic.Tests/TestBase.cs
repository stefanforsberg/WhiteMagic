using NUnit.Framework;
using StructureMap;

namespace WhiteMagic.Tests
{
    public class TestBase
    {
        protected InMemoryContentRepository ContentRepository;

        [SetUp]
        public void Setup()
        {
            ContentRepository = new InMemoryContentRepository(ObjectFactory.Container, new InMemoryPermanentLinkMapper());
            ContentRepository.CreateSystemPages();

            Given();
            When();
        }

        public virtual void Given()
        {
            
        }

        public virtual void When()
        {
            
        }
    }
}
