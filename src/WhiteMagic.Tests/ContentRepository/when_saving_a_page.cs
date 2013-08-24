using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using NUnit.Framework;
using Shouldly;
using WhiteMagic.Tests.Pages;

namespace WhiteMagic.Tests.ContentRepository
{
    public class when_creating_a_page_and_save_it : TestBase
    {
        private StartPage _loadedPage;
        private PageReference _startPageReference;

        public override void Given()
        {
            base.Given();
            _startPageReference = ContentRepository
                .Publish<StartPage>(ContentReference.RootPage, p => p.MainBody = "Hejhej!");
        }

        public override void When()
        {
            base.When();

            _loadedPage = ContentRepository.Get<StartPage>(_startPageReference); 
        }

        [Test]
        public void it_should_be_returned_when_loaded()
        {
            _loadedPage.MainBody.ShouldBe("Hejhej!");
        }

    }

    public class when_editing_an_existing_page_without_saving_it : TestBase
    {
        private PageReference _startPageReference;

        public override void Given()
        {
            base.Given();
            _startPageReference = ContentRepository
                .Publish<StartPage>(ContentReference.RootPage, p => p.MainBody = "Hejhej!");
        }

        public override void When()
        {
            base.When();

            var startPage = ContentRepository.Get<StartPage>(_startPageReference);
            startPage.MainBody = "An edited mainbody";
        }

        [Test]
        public void it_should_not_return_a_value_set_when_page_is_not_saved_afterwards()
        {
            ContentRepository
                .Get<StartPage>(_startPageReference)
                .MainBody
                .ShouldBe("Hejhej!");
        }
    }

    public class when_editing_an_existing_page_and_then_saveing_it : TestBase
    {
        private PageReference _startPageReference;

        public override void Given()
        {
            base.Given();
            _startPageReference = ContentRepository
                .Publish<StartPage>(ContentReference.RootPage, p => p.MainBody = "Hejhej!");
        }

        public override void When()
        {
            base.When();

            var startPage = ContentRepository.Get<StartPage>(_startPageReference);
            startPage.MainBody = "An edited mainbody";
            ContentRepository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);
        }

        [Test]
        public void it_should_return_the_edited_page()
        {
            ContentRepository
                .Get<StartPage>(_startPageReference)
                .MainBody
                .ShouldBe("An edited mainbody");
        }
    }
}