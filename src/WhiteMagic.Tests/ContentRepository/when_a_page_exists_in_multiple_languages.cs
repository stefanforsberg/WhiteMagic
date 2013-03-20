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
    public class when_a_page_exists_in_swedish_and_english : TestBase
    {
        private PageReference _startPageReference;

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

        [Test]
        public void it_should_be_able_to_load_english_page()
        {
            ContentRepository.Get<StartPage>(_startPageReference).PageName.ShouldBe("StartPage");
        }

        [Test]
        public void it_should_be_able_to_load_swedish_page()
        {
            ContentRepository.Get<StartPage>(_startPageReference, new LanguageSelector("sv")).PageName.ShouldBe("Startsida");
        }
    }
}
