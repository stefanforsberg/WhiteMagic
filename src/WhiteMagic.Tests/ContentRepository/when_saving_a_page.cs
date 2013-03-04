using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using NUnit.Framework;
using Shouldly;
using WhiteMagic.Tests.Pages;

namespace WhiteMagic.Tests.ContentRepository
{
    public class when_saving_a_page : TestBase
    {
        [Test]
        public void it_should_be_returned_when_loaded()
        {
            var startPage = ContentRepository.GetDefaultPageData<StartPage>(ContentReference.RootPage);
            startPage.MainBody = "Hejhej!";

            ContentRepository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);

            var page2 = ContentRepository.Get<StartPage>(startPage.PageLink);
            page2.MainBody.ShouldBe("Hejhej!");
        }

    }
}