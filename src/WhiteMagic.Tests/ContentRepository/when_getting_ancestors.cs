using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using NUnit.Framework;
using Shouldly;
using WhiteMagic.Tests.Pages;

namespace WhiteMagic.Tests.ContentRepository
{
    public class when_getting_ancestors : TestBase
    {
        private PageReference _pageToUseRef;
        private IEnumerable<IContent> _ancestors;

        public override void Given()
        {
            base.Given();

            var startPageReference = ContentRepository.Publish<StartPage>(ContentReference.RootPage, "StartPage");
            var childPage1Reference = ContentRepository.Publish<StartPage>(startPageReference, "ChildPage1");
            var childPage1ChildPage1Reference = ContentRepository.Publish<StandardPage>(childPage1Reference, "ChildPage1-ChildPage1");

            _pageToUseRef = ContentRepository.Publish<StandardPage>(childPage1ChildPage1Reference, "ChildPage1-ChildPage3-ChildPage1");
        }

        public override void When()
        {
            base.When();
            _ancestors = ContentRepository.GetAncestors(_pageToUseRef);
        }

        [Test]
        public void it_should_return_the_correct_number_of_pages()
        {
            _ancestors.Count().ShouldBe(4);
        }

        [Test]
        public void it_should_return_the_pages_in_order_of_distance_to_origin()
        {
            _ancestors.ElementAt(0).Name.ShouldBe("ChildPage1-ChildPage1");
            _ancestors.ElementAt(1).Name.ShouldBe("ChildPage1");
            _ancestors.ElementAt(2).Name.ShouldBe("StartPage");
            _ancestors.ElementAt(3).Name.ShouldBe("RootPage");
        }
    }

    public class when_getting_ancestors_for_the_root_page : TestBase
    {
        private IEnumerable<IContent> _ancestors;

        //public override void Given()
        //{
        //    base.Given();

        //    var startPageReference = ContentRepository.Publish<StartPage>(ContentReference.RootPage, "StartPage");
        //    var childPage1Reference = ContentRepository.Publish<StartPage>(startPageReference, "ChildPage1");
        //    var childPage1ChildPage1Reference = ContentRepository.Publish<StandardPage>(childPage1Reference, "ChildPage1-ChildPage1");

        //    _pageToUseRef = ContentRepository.Publish<StandardPage>(childPage1ChildPage1Reference, "ChildPage1-ChildPage3-ChildPage1");
        //}

        public override void When()
        {
            base.When();
            _ancestors = ContentRepository.GetAncestors(ContentReference.RootPage);
        }

        [Test]
        public void it_should_not_have_any_ancestors()
        {
            _ancestors.Count().ShouldBe(0);
        }
    }
}