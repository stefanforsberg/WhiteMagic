using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using NUnit.Framework;
using Shouldly;
using WhiteMagic.Tests.Pages;

namespace WhiteMagic.Tests.ContentRepository
{
    public class when_getting_children_by_page_type : TestBase
    {
        
        IEnumerable<StartPage> _children;
        private PageReference _startPageReference;

        public override void Given()
        {
            base.Given();

            _startPageReference = ContentRepository.Publish<StartPage>(ContentReference.RootPage);

            ContentRepository.Publish<StartPage>(_startPageReference, "ChildPage1");
            ContentRepository.Publish<StartPage>(_startPageReference, "ChildPage2");
            ContentRepository.Publish<StandardPage>(_startPageReference, "ChildPage3");
        }

        public override void When()
        {
            base.When();
            _children = ContentRepository.GetChildren<StartPage>(_startPageReference);
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

    public class when_getting_children_with_a_none_matching_page_type : TestBase
    {

        IEnumerable<StandardPage> _children;
        private PageReference _startPageReference;

        public override void Given()
        {
            base.Given();

            _startPageReference = ContentRepository.Publish<StartPage>(ContentReference.RootPage);

            ContentRepository.Publish<StartPage>(_startPageReference, "ChildPage1");
            ContentRepository.Publish<StartPage>(_startPageReference, "ChildPage2");
            ContentRepository.Publish<StartPage>(_startPageReference, "ChildPage3");
        }

        public override void When()
        {
            base.When();
            _children = ContentRepository.GetChildren<StandardPage>(_startPageReference);
        }

        [Test]
        public void it_should_not_fetch_any_children()
        {
            _children.Count().ShouldBe(0);
        }
    }

    public class when_getting_children_for_a_none_existing_page : TestBase
    {
        [Test]
        public void it_should_throw_a_page_not_found_exception()
        {
            Assert.Throws<PageNotFoundException>(() => ContentRepository.GetChildren<PageData>(new ContentReference(1000)));
        }
    }
}