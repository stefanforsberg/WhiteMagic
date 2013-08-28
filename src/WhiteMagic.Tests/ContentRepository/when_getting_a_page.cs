using EPiServer.Core;
using NUnit.Framework;
using Shouldly;
using WhiteMagic.Tests.Pages;

namespace WhiteMagic.Tests.ContentRepository
{
    public class when_getting_a_page : TestBase
    {
        protected PageReference StartPageReference;

        public override void Given()
        {
            base.Given();
            StartPageReference = ContentRepository
                .Publish<StartPage>(ContentReference.RootPage, "StartPage");
        }
    }

    public class when_getting_a_page_with_base_type_constraint : when_getting_a_page
    {
        private PageData _page;

        public override void When()
        {
            _page = ContentRepository.Get<PageData>(StartPageReference);
        }

        [Test]
        public void it_should_return_a_page_typed_to_the_base_type()
        {
            _page.PageName.ShouldBe("StartPage");
        }
    }

    public class when_getting_a_page_with_wrong_type_constraint : when_getting_a_page
    {
        [Test]
        public void it_should_throw_a_type_mismatch_exception()
        {
            Assert.Throws<TypeMismatchException>(() =>ContentRepository.Get<StandardPage>(StartPageReference));
        }
    }

    public class when_getting_a_page_by_content_reference_that_does_not_exist : when_getting_a_page
    {
        [Test]
        public void it_should_throw_a_page_not_found_exception()
        {
            Assert.Throws<PageNotFoundException>(() => ContentRepository.Get<StandardPage>(new ContentReference(1000)));
        }
    }
}
