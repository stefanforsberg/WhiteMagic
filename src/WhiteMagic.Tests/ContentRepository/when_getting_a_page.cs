using EPiServer.Core;
using NUnit.Framework;
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
                .Publish<StartPage>(ContentReference.RootPage, p => p.MainBody = "Hejhej!");
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
