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

            _startPage = ContentRepository.Publish<StartPage>(ContentReference.RootPage);

            ContentRepository.Publish<StartPage>(_startPage.PageLink, "ChildPage1");
            ContentRepository.Publish<StartPage>(_startPage.PageLink, "ChildPage2");
            ContentRepository.Publish<StandardPage>(_startPage.PageLink, "ChildPage3");
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