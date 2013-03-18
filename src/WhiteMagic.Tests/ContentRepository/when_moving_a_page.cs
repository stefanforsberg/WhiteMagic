using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using NUnit.Framework;
using Shouldly;
using WhiteMagic.Tests.Pages;

namespace WhiteMagic.Tests.ContentRepository
{
    public class when_moving_a_page : TestBase
    {
        StartPage _startPage;
        StartPage _oldParent;
        StartPage _newParent;
        StartPage _pageToMove;

        public override void Given()
        {
            base.Given();

            _startPage = ContentRepository
                .Publish<StartPage>(ContentReference.RootPage);

            _oldParent = ContentRepository
                .GetDefault<StartPage>(_startPage.PageLink)
                .With(p =>
                    {
                        p.PageName = "OldParent";
                        p.LinkURL = "oldparent";
                    })
                .SaveAndPublish(ContentRepository);

            _newParent = ContentRepository.GetDefault<StartPage>(_startPage.PageLink);
            _newParent.PageName = "NewParent";
            _newParent.LinkURL = "newparent";
            ContentRepository.Save(_newParent, SaveAction.Publish, AccessLevel.NoAccess);

            _pageToMove = ContentRepository.GetDefault<StartPage>(_oldParent.PageLink);
            _pageToMove.PageName = "PageToMove";
            ContentRepository.Save(_pageToMove, SaveAction.Publish, AccessLevel.NoAccess);
        }

        public override void When()
        {
            base.When();
            ContentRepository.Move(_pageToMove.PageLink, _newParent.PageLink);
        }

        [Test]
        public void it_should_have_the_destination_page_as_parent()
        {
            var pageAfterMove = ContentRepository.Get<StartPage>(_pageToMove.PageLink);
            pageAfterMove.ParentLink.ShouldBe(_newParent.PageLink);
        }

        [Test]
        public void it_should_be_returned_when_getting_children_for_new_parent()
        {
            ContentRepository
                .GetChildren<StartPage>(_newParent.PageLink)
                .ShouldContain(p => p.PageName == "PageToMove");
        }

        [Test]
        public void it_should_not_be_returned_when_getting_children_for_old_parent()
        {
            ContentRepository
                .GetChildren<StartPage>(_oldParent.PageLink)
                .ShouldNotContain(p => p.PageName == "PageToMove");
                
        }
    }
}