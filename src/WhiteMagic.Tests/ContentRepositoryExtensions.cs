using System;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;

namespace WhiteMagic.Tests
{
    public static class ContentRepositoryExtensions
    {
        public static readonly Guid RootPageGuid = new Guid("945D02B0-B744-4349-A995-87CE3FCD51F4");
        public static readonly Guid WasteBasketGuid = new Guid("27D4C717-4ABC-4DC4-B3A1-5008F311E301");

        public static void CreateSystemPages(this IContentRepository contentRepository)
        {
            var rootPage = new PageData();
            rootPage.Property.Add(new PropertyString() { Name = "PageName" });
            rootPage.Property.Add(new PropertyDate() { Name = "PageChanged", Value = DateTime.Now });
            rootPage.Property.Add(new PropertyString() { Name = "PageChangedBy" });
            rootPage.Property.Add(new PropertyString() { Name = "PageLinkURL" });
            rootPage.Property.Add(new PropertyString() { Name = "PageGUID" });
            rootPage.Property.Add(new PropertyPageReference() { Name = "PageLink" });
            rootPage.Property.Add(new PropertyPageReference() { Name = "PageParentLink" });
            rootPage.Property.Add(new PropertyDate() { Name = "PageSaved" });
            rootPage.Property.Add(new PropertyBoolean() { Name = "PagePendingPublish" });
            rootPage.Property.Add(new PropertyString() { Name = "PageLanguageBranch" });
            rootPage.Property.Add(new PropertyString() { Name = "PageWorkStatus" });
            rootPage.Property.Add(new PropertyString() { Name = "SimpleAddress", Value = "RootSimpleAddress" });
            rootPage.PageName = "RootPage";
            rootPage.PageLink = new PageReference(1);
            rootPage.ParentLink = PageReference.EmptyReference;
            rootPage.LinkURL = "/templates/sysroot.aspx?id=1";
            rootPage.PageGuid = RootPageGuid;
            rootPage.Property["PageLanguageBranch"].Value = "en";

            contentRepository.Save(rootPage, SaveAction.Publish, AccessLevel.NoAccess);
            ContentReference.RootPage = rootPage.PageLink;

            var wasteBaketPage = new PageData();
            wasteBaketPage.Property.Add(new PropertyString() { Name = "PageName" });
            wasteBaketPage.Property.Add(new PropertyDate() { Name = "PageChanged", Value = DateTime.Now });
            wasteBaketPage.Property.Add(new PropertyString() { Name = "PageChangedBy" });
            wasteBaketPage.Property.Add(new PropertyString() { Name = "PageLinkURL" });
            wasteBaketPage.Property.Add(new PropertyString() { Name = "PageGUID" });
            wasteBaketPage.Property.Add(new PropertyPageReference() { Name = "PageLink" });
            wasteBaketPage.Property.Add(new PropertyPageReference() { Name = "PageParentLink" });
            wasteBaketPage.Property.Add(new PropertyDate() { Name = "PageSaved" });
            wasteBaketPage.Property.Add(new PropertyBoolean() { Name = "PagePendingPublish" });
            wasteBaketPage.Property.Add(new PropertyString() { Name = "PageLanguageBranch" });
            wasteBaketPage.Property.Add(new PropertyString() { Name = "PageWorkStatus" });
            wasteBaketPage.Property.Add(new PropertyString() { Name = "SimpleAddress", Value = "WasteBasketSimpleAddress" });
            wasteBaketPage.PageName = "Wastebasket";
            wasteBaketPage.PageLink = new PageReference(2);
            wasteBaketPage.ParentLink = new PageReference(1);
            wasteBaketPage.LinkURL = "/templates/syswastebasket.aspx?id=2";
            wasteBaketPage.PageGuid = WasteBasketGuid;
            wasteBaketPage.Property["PageLanguageBranch"].Value = "en";

            contentRepository.Save(wasteBaketPage, SaveAction.Publish, AccessLevel.NoAccess);
            ContentReference.WasteBasket = wasteBaketPage.PageLink;
        }
    }
}