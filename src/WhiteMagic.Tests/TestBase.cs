using System;
using EPiServer.Core;
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
            ContentRepository = new InMemoryContentRepository(StandardActivator, new InMemoryPermanentLinkMapper());
            ContentRepository.CreateSystemPages();

            Given();
            When();
        }

        private static IContentData StandardActivator(Type type)
        {
            return Activator.CreateInstance(type) as IContentData;
        }

        private IContentData StructureMapActivator(Type type)
        {
            return ObjectFactory.Container.GetInstance(type) as IContentData;
        }

        public virtual void Given()
        {
            
        }

        public virtual void When()
        {
            
        }
    }
}
