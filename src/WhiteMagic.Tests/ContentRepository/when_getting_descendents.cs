using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Core;
using NUnit.Framework;
using Shouldly;
using WhiteMagic.Tests.Pages;

namespace WhiteMagic.Tests.ContentRepository
{
    public class when_getting_descendents : TestBase
    {
        protected PageReference StartPageReference;
        protected PageReference ChildPage3Reference;
        protected PageReference ChildPage3ChildPage3Reference;
        protected IEnumerable<ContentReference> Descendants;

        public override void Given()
        {
            base.Given();

            StartPageReference = ContentRepository.Publish<StartPage>(ContentReference.RootPage);

            ContentRepository.Publish<StartPage>(StartPageReference, "ChildPage1");
            ContentRepository.Publish<StartPage>(StartPageReference, "ChildPage2");
            ChildPage3Reference = ContentRepository.Publish<StartPage>(StartPageReference, "ChildPage3");

            ContentRepository.Publish<StandardPage>(ChildPage3Reference, "ChildPage3-ChildPage1");
            ContentRepository.Publish<StandardPage>(ChildPage3Reference, "ChildPage3-ChildPage2");
            ChildPage3ChildPage3Reference = ContentRepository.Publish<StandardPage>(ChildPage3Reference, "ChildPage3-ChildPage3");

            ContentRepository.Publish<StandardPage>(ChildPage3ChildPage3Reference, "ChildPage3-ChildPage3-ChildPage1");
            ContentRepository.Publish<StandardPage>(ChildPage3ChildPage3Reference, "ChildPage3-ChildPage3-ChildPage2");
            ContentRepository.Publish<StandardPage>(ChildPage3ChildPage3Reference, "ChildPage3-ChildPage3-ChildPage3");
        }
    }

    public class when_getting_descendents_from_start_page : when_getting_descendents
    {
        public override void When()
        {
            base.When();
            Descendants = ContentRepository.GetDescendents(ContentReference.RootPage);
        }

        [Test]
        public void it_should_return_the_correct_number_of_descendants_including_the_waste_basket_page()
        {
            Descendants.Count().ShouldBe(11);
        }
    }

    public class when_getting_descendents_from_page_deeper_in_structure : when_getting_descendents
    {

        public override void When()
        {
            base.When();
            Descendants = ContentRepository.GetDescendents(ChildPage3Reference);
        }

        [Test]
        public void it_should_return_the_correct_number_of_descendants()
        {
            Descendants.Count().ShouldBe(6);
        }
    }
}
