using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.SpecializedProperties;
using EPiServer.Web;
using StructureMap;

namespace WhiteMagic.Tests
{
    public class InMemoryContentRepository : IContentRepository
    {
        private readonly Func<Type, IContentData> _activator;
        private readonly IInmemoryPermantentLinkMapper _permanentLinkMapper;

        private readonly Dictionary<PageReference, List<PageData>> _pages = new Dictionary<PageReference, List<PageData>>();
        private readonly Dictionary<PageReference, List<PageReference>> _structure = new Dictionary<PageReference, List<PageReference>>();

        private int _pageId = 3;

        public InMemoryContentRepository(Func<Type, IContentData> activator, IInmemoryPermantentLinkMapper permanentLinkMapper)
        {
            _activator = activator;
            _permanentLinkMapper = permanentLinkMapper;
        }

        #region IContentRepository

        public IEnumerable<T> GetLanguageBranches<T>(ContentReference contentLink) where T : IContentData
        {
            return GetLanguageBranches(contentLink.ToPageReference()).OfType<T>();
        }

        public T GetDefault<T>(ContentReference parentLink) where T : IContentData
        {
            return GetDefaultPageData<T>(parentLink.ToPageReference());
        }

        public T GetDefault<T>(ContentReference parentLink, ILanguageSelector languageSelector) where T : IContentData
        {
            throw new NotImplementedException();
        }

        public T GetDefault<T>(ContentReference parentLink, int contentTypeID, ILanguageSelector languageSelector) where T : IContentData
        {
            return EnsureTOrNull<T>(GetDefaultPageData(parentLink.ToPageReference(), contentTypeID, languageSelector));
        }

        public T CreateLanguageBranch<T>(ContentReference contentLink, ILanguageSelector languageSelector, AccessLevel access) where T : IContentData
        {
            return EnsureTOrNull<T>(CreateLanguageBranch(contentLink.ToPageReference(), languageSelector, access));
        }

        public ContentReference Copy(ContentReference source, ContentReference destination, AccessLevel requiredSourceAccess, AccessLevel requiredDestinationAccess, bool publishOnDestination)
        {
            return Copy(source.ToPageReference(), destination.ToPageReference(), requiredSourceAccess, requiredDestinationAccess, publishOnDestination);
        }

        public ContentReference Save(IContent content, SaveAction action, AccessLevel access)
        {
            return Save(content as PageData, action, access);
        }

        public IEnumerable<ReferenceInformation> GetReferencesToContent(ContentReference contentLink, bool includeDecendents)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReferenceInformation> GetReferencesToContent(ContentReference contentLink)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IContent> IContentRepository.ListDelayedPublish()
        {
            throw new NotImplementedException();
        }

        public T Get<T>(Guid contentGuid) where T : IContentData
        {
            throw new NotImplementedException();
        }

        public T Get<T>(Guid contentGuid, ILanguageSelector selector) where T : IContentData
        {
            throw new NotImplementedException();
        }

        public T Get<T>(ContentReference contentLink) where T : IContentData
        {
            return EnsureTOrNull<T>(GetPage(contentLink.ToPageReference()));
        }

        public T Get<T>(ContentReference contentLink, ILanguageSelector selector) where T : IContentData
        {
            return EnsureTOrNull<T>(GetPage(contentLink.ToPageReference(), selector));
        }

        public IEnumerable<T> GetChildren<T>(ContentReference contentLink) where T : IContentData
        {
            return GetChildren(contentLink.ToPageReference(), -1, -1).OfType<T>();
        }

        public IEnumerable<T> GetChildren<T>(ContentReference contentLink, ILanguageSelector selector) where T : IContentData
        {
            return GetChildren(contentLink.ToPageReference(), -1, -1).OfType<T>();
        }

        public IEnumerable<T> GetChildren<T>(ContentReference contentLink, ILanguageSelector selector, int startIndex, int maxRows) where T : IContentData
        {
            return GetChildren(contentLink.ToPageReference(), -1, -1).OfType<T>();
        }

        public IEnumerable<ContentReference> GetDescendents(ContentReference contentLink)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IContent> GetAncestors(ContentReference contentLink)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IContent> GetItems(IEnumerable<ContentReference> contentLinks, ILanguageSelector selector)
        {
            return contentLinks.Select(Get<IContent>);
        }

        public void DeleteChildren(ContentReference contentLink, bool forceDelete, AccessLevel access)
        {
            throw new NotImplementedException();
        }

        public void Delete(ContentReference pageLink, bool forceDelete, AccessLevel access)
        {
            PageEventArgs args = new PageEventArgs(pageLink);
            RaisePageEvent(DeletingPage, args);
            RaisePageEvent(DeletedPage, args);

            throw new NotImplementedException();
        }

        public void DeleteLanguageBranch(ContentReference pageLink, string languageBranch, AccessLevel access)
        {
            PageEventArgs args = new PageEventArgs(pageLink);
            RaisePageEvent(DeletingPageLanguage, args);
            RaisePageEvent(DeletedPageLanguage, args);

            throw new NotImplementedException();
        }

        public void Move(ContentReference pageLink, ContentReference destinationLink, AccessLevel requiredSourceAccess, AccessLevel requiredDestinationAccess)
        {
            PageEventArgs pageArgs = new PageEventArgs(pageLink, destinationLink);
            RaisePageEvent(MovingPage, pageArgs);

            var page = GetPage(pageLink.ToPageReference()).CreateWritableClone();

            _structure[page.ParentLink].Remove(pageLink.ToPageReference());

            page.ParentLink = destinationLink.ToPageReference();
            _pages[pageLink.ToPageReference()].Add(page);

            if (!_structure.ContainsKey(destinationLink.ToPageReference()))
            {
                _structure.Add(destinationLink.ToPageReference(), new List<PageReference> { pageLink.ToPageReference() });
            }
            else
            {
                _structure[destinationLink.ToPageReference()].Add(page.PageLink);
            }

            RaisePageEvent(MovedPage, pageArgs); ;
        }

        public void MoveToWastebasket(ContentReference contentLink, string deletedBy)
        {
            throw new NotImplementedException();
        }

        #endregion

        public PageData CreateLanguageBranch(PageReference pageLink, ILanguageSelector selector, AccessLevel access)
        {
            PageData masterPage = GetPage(pageLink);

            LanguageSelectorContext ls = new LanguageSelectorContext(masterPage);
            selector.LoadLanguage(ls);
            PageData pageData = ConstructContentData<PageData>(masterPage.PageName, masterPage.PageLink, masterPage.ParentLink, ls.SelectedLanguage);
            List<String> languages = new List<string>(masterPage.PageLanguages);
            languages.Add(ls.SelectedLanguage);
            pageData.InitializeData(languages);
            pageData.PageGuid = masterPage.PageGuid;
            pageData.PageLink.ID = masterPage.PageLink.ID;
            return pageData;
        }

        private PageDataCollection GetChildren(PageReference pageLink, int startIndex, int maxRows)
        {
            List<IContent> children = new List<IContent>();

            ChildrenEventArgs args = new ChildrenEventArgs(pageLink, children);
            try
            {

                RaiseChildrenEvent(LoadingChildren, args);
                List<PageReference> childrenRefs;
                if (_structure.TryGetValue(pageLink, out childrenRefs))
                {
                    foreach (PageReference link in childrenRefs)
                    {
                        children.Add(GetPage(link));
                    }
                }
            }
            catch
            {
                RaiseChildrenEvent(FailedLoadingChildren, args);
                throw;
            }

            RaiseChildrenEvent(LoadedChildren, args);
            RaiseChildrenEvent(FinishedLoadingChildren, args);
            return new PageDataCollection(children);
        }

        private PageData GetDefaultPageData(PageReference parentPageLink, int pageTypeID, ILanguageSelector selector)
        {
            return GetDefaultPageData<PageData>(parentPageLink);
        }

        public TPageData GetDefaultPageData<TPageData>(PageReference parentPageLink)
            where TPageData : IContentData
        {
            PageEventArgs args = new PageEventArgs(parentPageLink);
            RaisePageEvent(LoadingDefaultPageData, args);

            var emptyPage = ConstructContentData<TPageData>(String.Empty, parentPageLink);

            RaisePageEvent(LoadedDefaultPageData, new PageEventArgs(emptyPage as PageData));

            return emptyPage;
        }

        private PageDataCollection GetLanguageBranches(PageReference pageLink)
        {
            PageDataCollection languages = new PageDataCollection();
            languages.Add(GetPage(pageLink));
            return languages;
        }

        private PageData GetPage(PageReference pageLink, ILanguageSelector selector)
        {
            RaisePageEvent(LoadingPage, new PageEventArgs(pageLink));

            PageData page = null;
            try
            {
                List<PageData> pages;
                if (!_pages.TryGetValue(pageLink, out pages))
                {
                    throw new PageNotFoundException(pageLink);
                }
                page = pages.Last();
                LanguageSelector ls = selector as LanguageSelector;
                if (ls != null && !String.IsNullOrEmpty(ls.LanguageBranch))
                {
                    page = pages.Find(p => p.LanguageBranch == ls.LanguageBranch);
                }
            }
            catch
            {
                RaisePageEvent(FailedLoadingPage, new PageEventArgs(pageLink));
                throw;
            }

            PageEventArgs pageArgs = new PageEventArgs(page);
            RaisePageEvent(LoadedPage, pageArgs);
            RaisePageEvent(FinishedLoadingPage, pageArgs);

            return page;
        }

        public PageReference Save(PageData page, SaveAction action, AccessLevel access)
        {
            //Ensure valid properties are set
            if (PageReference.IsNullOrEmpty(page.PageLink))
            {
                page.PageLink = new PageReference(_pageId++);
            }
            else
            {
                if (page.PageLink.ID > _pageId)
                {
                    _pageId = page.PageLink.ID;
                }
            }

            if (page.PageGuid == Guid.Empty)
            {
                page.PageGuid = Guid.NewGuid();
            }

            PageEventArgs pageArgs = new PageEventArgs(page);
            RaisePageEvent(SavingPage, pageArgs);

            if (!_pages.ContainsKey(page.PageLink))
            {
                RaisePageEvent(CreatingPage, pageArgs);
            }

            if ((action & SaveAction.CheckIn) == SaveAction.CheckIn)
            {
                RaisePageEvent(CheckingInPage, pageArgs);

                page["PagePendingPublish"] = true;

                RaisePageEvent(CheckedInPage, pageArgs);
            }

            if ((action & SaveAction.Publish) == SaveAction.Publish)
            {
                RaisePageEvent(PublishingPage, pageArgs);

                page["PagePendingPublish"] = false;
                page["PageWorkStatus"] = VersionStatus.Published;
                page.Changed = DateTime.Now;

                RaisePageEvent(PublishedPage, pageArgs);
            }

            if ((action & SaveAction.ForceCurrentVersion) == SaveAction.ForceCurrentVersion)
            {
                RaisePageEvent(PublishingPage, pageArgs);

                page["PagePendingPublish"] = false;
                page["PageWorkStatus"] = VersionStatus.Published;
                page.Changed = DateTime.Now;

                RaisePageEvent(PublishedPage, pageArgs);
            }

            if ((action & SaveAction.ForceNewVersion) == SaveAction.ForceNewVersion)
            {
                RaisePageEvent(PublishingPage, pageArgs);

                page["PagePendingPublish"] = false;
                page["PageWorkStatus"] = VersionStatus.Published;
                page.Changed = DateTime.Now;

                RaisePageEvent(PublishedPage, pageArgs);
            }

            try
            {
                if (String.IsNullOrEmpty(page.LinkURL))
                {
                    page.LinkURL = "/templates/Page.aspx?id=" + page.PageLink.ToString();
                }
            }
            catch
            { }

            page["PageSaved"] = DateTime.Now;

            if (!_pages.ContainsKey(page.PageLink))
            {
                _pages[page.PageLink] = new List<PageData>();
                RaisePageEvent(CreatedPage, pageArgs);
            }

            _pages[page.PageLink].Add(page);

            RaisePageEvent(SavedPage, pageArgs);

            if (!PageReference.IsNullOrEmpty(page.ParentLink))
            {
                List<PageReference> parentChildren;
                if (!_structure.TryGetValue(page.ParentLink, out parentChildren))
                {
                    parentChildren = new List<PageReference>();
                    _structure[page.ParentLink] = parentChildren;
                }

                if (!parentChildren.Contains(page.PageLink))
                {
                    parentChildren.Add(page.PageLink);
                }
            }

            _permanentLinkMapper.RegisterLinkMap(new PermanentContentLinkMap(page.PageGuid, "aspx", new Uri("http://localhost" + page.LinkURL), page.PageLink, null));

            return page.PageLink;
        }

        private PageData GetPage(PageReference pageLink)
        {
            List<PageData> pages;
            if (!_pages.TryGetValue(pageLink, out pages))
            {
                throw new PageNotFoundException(pageLink);
            }
            return pages.Last().Copy();
        }

        private void RaisePageEvent(PageEventHandler pageEvent, PageEventArgs e)
        {
            if (pageEvent != null)
            {
                pageEvent(this, e);
            }
        }

        private void RaiseChildrenEvent(ChildrenEventHandler childrenEvent, ChildrenEventArgs e)
        {
            if (childrenEvent != null)
            {
                childrenEvent(this, e);
            }
        }

        private TPageData ConstructContentData<TPageData>(string pageName, PageReference parentLink)
            where TPageData : IContentData
        {
            return ConstructContentData<TPageData>(pageName, PageReference.EmptyReference, parentLink, "en");
        }

        private TPageData ConstructContentData<TPageData>(string pageName, PageReference pageLink, PageReference parentLink, String language)
            where TPageData : IContentData
        {
            var page = _activator.Invoke(typeof(TPageData));
            
            page.Property.Add(new PropertyString() { Name = "PageName" });
            page.Property.Add(new PropertyDate() { Name = "PageStartPublish" });
            page.Property.Add(new PropertyDate() { Name = "PageStopPublish" });
            page.Property.Add(new PropertyDate() { Name = "PageChanged", Value = DateTime.Now });
            page.Property.Add(new PropertyString() { Name = "PageChangedBy" });
            page.Property.Add(new PropertyNumber() { Name = "PagePeerOrder" });
            page.Property.Add(new PropertySortOrder() { Name = "PageChildOrderRule" });
            page.Property.Add(new PropertyString() { Name = "PageLinkURL" });
            page.Property.Add(new PropertyString() { Name = "PageURLSegment" });
            page.Property.Add(new PropertyString() { Name = "PageGUID" });
            page.Property.Add(new PropertyString() { Name = "PageTypeName", Value = pageName });
            page.Property.Add(new PropertyPageReference() { Name = "PageLink" });
            page.Property.Add(new PropertyPageReference() { Name = "PageParentLink" });
            page.Property.Add(new PropertyDate() { Name = "PageSaved" });
            page.Property.Add(new PropertyBoolean() { Name = "PagePendingPublish" });
            page.Property.Add(new PropertyString() { Name = "PageLanguageBranch" });
            page.Property.Add(new PropertyString() { Name = "PageMasterLanguageBranch" });
            page.Property.Add(new PropertyNumber { Name = "PageShortcutType" });
            page.Property.Add(new PropertyNumber { Name = "PageWorkStatus" });
            page.Property.Add(new PropertyString() { Name = "SimpleAddress", Value = pageName + "SimpleAddress" });
            page.Property["PageLanguageBranch"].Value = language;

            if (page is PageData)
            {
                AddPageDataProperties(page as PageData, pageName, pageLink, parentLink);
                InitBlocks(page as PageData);
            }

            return (TPageData)page;
        }

        private void AddPageDataProperties(PageData page, string pageName, PageReference pageLink, PageReference parentLink)
        {
            page.ACL.Add(new AccessControlEntry("Everyone", AccessLevel.Read, SecurityEntityType.Role));

            page.PageName = pageName;
            page.PageLink = pageLink;
            page.ParentLink = parentLink;
            page.PageGuid = Guid.NewGuid();
            page.URLSegment = pageName;
            page.LinkURL = "/templates/Page.aspx?id=" + page.PageLink.ID;
        }

        private void InitBlocks(object page)
        {
            var blockProperties = page
                .GetType().
                GetProperties().
                Where(p => p.PropertyType.IsSubclassOf(typeof (BlockData)));

            foreach (var property in blockProperties)
            {
                var block = _activator.Invoke(property.PropertyType);

                if (property.CanWrite)
                {
                    property.SetValue(page, block, null);
                }

                InitBlocks(block);
            }
        }

        #region Events
        public event PageEventHandler LoadingPage;

        public event PageEventHandler LoadedPage;

        public event PageEventHandler FinishedLoadingPage;

        public event PageEventHandler FailedLoadingPage;

        public event PageEventHandler LoadingDefaultPageData;

        public event PageEventHandler LoadedDefaultPageData;

        public event PageEventHandler PublishingPage;

        public event PageEventHandler PublishedPage;

        public event PageEventHandler CheckingInPage;

        public event PageEventHandler CheckedInPage;

        public event PageEventHandler DeletingPage;

        public event PageEventHandler DeletedPage;

        public event PageEventHandler DeletingPageLanguage;

        public event PageEventHandler DeletedPageLanguage;

        public event PageEventHandler MovingPage;

        public event PageEventHandler MovedPage;

        public event PageEventHandler CreatingPage;

        public event PageEventHandler CreatedPage;

        public event PageEventHandler SavingPage;

        public event PageEventHandler SavedPage;

        public event ChildrenEventHandler LoadingChildren;

        public event ChildrenEventHandler LoadedChildren;

        public event ChildrenEventHandler FinishedLoadingChildren;

        public event ChildrenEventHandler FailedLoadingChildren;

        #endregion

        //Helper method to cast from PageData to T (if possible else null)
        private T EnsureTOrNull<T>(PageData page)
        {
            return new PageData[] { page }.OfType<T>().FirstOrDefault();
        }
    }

}