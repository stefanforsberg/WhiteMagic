using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using NUnit.Framework;
using Shouldly;
using WhiteMagic.Tests.Pages;

namespace WhiteMagic.Tests.ContentRepository
{
    public class when_getting_children_using_startindex_and_maxrows : TestBase
    {
        protected PageReference StartPageReference;
        protected IEnumerable<PageData> Children;

        public override void Given()
        {
            base.Given();

            StartPageReference = ContentRepository.Publish<StartPage>(ContentReference.RootPage);

            ContentRepository.Publish<StandardPage>(StartPageReference, "ChildPage1");
            ContentRepository.Publish<StandardPage>(StartPageReference, "ChildPage2");
            ContentRepository.Publish<StandardPage>(StartPageReference, "ChildPage3");
            ContentRepository.Publish<StandardPage>(StartPageReference, "ChildPage4");
            ContentRepository.Publish<StandardPage>(StartPageReference, "ChildPage5");
        }
    }

    public class when_skipping_first_page_and_only_getting_one_page : when_getting_children_using_startindex_and_maxrows
    {
        public override void When()
        {
            base.When();

            Children = ContentRepository.GetChildren<PageData>(StartPageReference, new LanguageSelector("en"), 1, 1);
        }

        [Test]
        public void it_should_return_the_expected_page()
        {
            Children.First().PageName.ShouldBe("ChildPage2");
        }

        [Test]
        public void it_should_only_return_one_page()
        {
            Children.Count().ShouldBe(1);
        }
    }

    public class when_skipping_two_pagse_and_getting_two_pages : when_getting_children_using_startindex_and_maxrows
    {
        public override void When()
        {
            base.When();

            Children = ContentRepository.GetChildren<PageData>(StartPageReference, new LanguageSelector("en"), 2, 2);
        }

        [Test]
        public void it_should_return_the_expected_page()
        {
            Children.First().PageName.ShouldBe("ChildPage3");
            Children.Last().PageName.ShouldBe("ChildPage4");
        }

        [Test]
        public void it_should_return_two_pages()
        {
            Children.Count().ShouldBe(2);
        }
    }
}