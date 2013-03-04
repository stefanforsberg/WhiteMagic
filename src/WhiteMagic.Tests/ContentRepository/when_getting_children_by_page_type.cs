using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using NUnit.Framework;
using Shouldly;
using WhiteMagic.Tests.Pages;

namespace WhiteMagic.Tests.ContentRepository
{
    public class when_getting_children_by_page_type : TestBase
    {
        StartPage _startPage;
        IEnumerable<StartPage> _children;

        public override void Given()
        {
            base.Given();

            _startPage = ContentRepository.GetDefaultPageData<StartPage>(ContentReference.RootPage);
            ContentRepository.Save(_startPage, SaveAction.Publish, AccessLevel.NoAccess);

            var childPage1 = ContentRepository.GetDefaultPageData<StartPage>(_startPage.PageLink);
            childPage1.PageName = "ChildPage1";
            ContentRepository.Save(childPage1, SaveAction.Publish, AccessLevel.NoAccess);

            var childPage2 = ContentRepository.GetDefaultPageData<StartPage>(_startPage.PageLink);
            childPage2.PageName = "ChildPage2";
            ContentRepository.Save(childPage2, SaveAction.Publish, AccessLevel.NoAccess);

            var childPage3 = ContentRepository.GetDefaultPageData<StandardPage>(_startPage.PageLink);
            childPage3.PageName = "ChildPage3";
            ContentRepository.Save(childPage3, SaveAction.Publish, AccessLevel.NoAccess);
        }

        public override void When()
        {
            base.When();
            _children = ContentRepository.GetChildren<StartPage>(_startPage.PageLink);
        }

        [Test]
        public void it_should_fetch_the_correct_number_of_children()
        {
            _children.Count().ShouldBe(2);
        }

        [Test]
        public void it_should_not_include_page_with_none_matching_page_type()
        {
            _children.ShouldNotContain(s => s.PageName == "ChildPage3");
        }
    }
}