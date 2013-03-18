using EPiServer.Core;
using NUnit.Framework;
using Shouldly;
using WhiteMagic.Tests.Pages;

namespace WhiteMagic.Tests.ContentRepository
{
    public class when_saving_a_page : TestBase
    {
        private StartPage _startPage;
        private StartPage _loadedPage;

        public override void Given()
        {
            base.Given();
            _startPage = ContentRepository
                .Publish<StartPage>(ContentReference.RootPage, p => p.MainBody = "Hejhej!");
        }

        public override void When()
        {
            base.When();
            _loadedPage = ContentRepository.Get<StartPage>(_startPage.PageLink); 
        }

        [Test]
        public void it_should_be_returned_when_loaded()
        {
            _loadedPage.MainBody.ShouldBe("Hejhej!");
        }

    }
}