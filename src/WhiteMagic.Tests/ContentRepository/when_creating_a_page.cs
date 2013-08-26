using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using NUnit.Framework;
using Shouldly;
using WhiteMagic.Tests.Pages;

namespace WhiteMagic.Tests.ContentRepository
{
    public class when_creating_a_page : TestBase
    {
        private PageReference _startPageReference;
        private ReadOnlyStringList _languages;

        public override void Given()
        {
            base.Given();

            _startPageReference = ContentRepository
                .Publish<StartPage>(ContentReference.RootPage, "StartPage");
        }

        public override void When()
        {
            _languages = ContentRepository.Get<StartPage>(_startPageReference).PageLanguages;
        }

        [Test]
        public void the_page_should_have_one_language()
        {
            _languages.Count.ShouldBe(1);
        }

        [Test]
        public void the_page_should_have_master_language()
        {
            _languages[0].ShouldBe("EN");
        }
    }

    public class when_creating_a_page_and_adding_languagebrach : TestBase
    {
        private PageReference _startPageReference;
        private ReadOnlyStringList _languages;

        public override void Given()
        {
            base.Given();

            _startPageReference = ContentRepository
                .Publish<StartPage>(ContentReference.RootPage, "StartPage");

            var startPageInSwedish = ContentRepository
                .CreateLanguageBranch<StartPage>(_startPageReference, new LanguageSelector("sv"), AccessLevel.NoAccess);

            startPageInSwedish.PageName = "Startsida";

            ContentRepository.Save(startPageInSwedish, SaveAction.Publish, AccessLevel.NoAccess);
        }

        public override void When()
        {
            _languages = ContentRepository.Get<StartPage>(_startPageReference).PageLanguages;
        }

        [Test]
        public void the_page_should_have_two_languages()
        {
            _languages.Count.ShouldBe(2);
        }
    }
}
