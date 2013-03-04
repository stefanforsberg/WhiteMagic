using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web;

namespace WhiteMagic.Tests
{
    public interface IInmemoryPermantentLinkMapper : IPermanentLinkMapper
    {
        void RegisterLinkMap(PermanentLinkMap linkMap);
    }

    public class InMemoryPermanentLinkMapper : IInmemoryPermantentLinkMapper
    {
        private readonly List<PermanentLinkMap> _linkMaps = new List<PermanentLinkMap>();

        public void RegisterLinkMap(PermanentLinkMap linkMap)
        {
            _linkMaps.Add(linkMap);
        }

        public bool TryToPermanent(string url, out string permanentUrl)
        {
            permanentUrl = null;
            var urlBuilder = new EPiServer.UrlBuilder(url);
            var plm = Find(urlBuilder);
            if (plm != null)
            {
                permanentUrl = MergeQueryParams(plm.PermanentLinkUrl, urlBuilder, false);
                return true;
            }
            return false;
        }

        public bool TryToMapped(string url, out string mappedUrl)
        {
            mappedUrl = null;
            var urlBuilder = new UrlBuilder(url);
            var plm = Find(urlBuilder);
            if (plm != null)
            {
                mappedUrl = MergeQueryParams(plm.MappedUrl, urlBuilder, true);
                return true;
            }
            return false;
        }

        private static string MergeQueryParams(Uri mappedUrl, UrlBuilder incomingUrl, bool keepId)
        {
            var queryCollection = incomingUrl.QueryCollection;

            var fragment = incomingUrl.Fragment;
            incomingUrl.Uri = mappedUrl;
            incomingUrl.Fragment = fragment;

            foreach (string q in incomingUrl.QueryCollection.Keys)
            {
                queryCollection.Remove(q);
            }
            incomingUrl.QueryCollection.Add(queryCollection);

            if (!keepId)
            {
                incomingUrl.QueryCollection.Remove("id");
            }

            return incomingUrl.ToString();
        }

        public PermanentLinkMap Find(Guid guid)
        {
            return _linkMaps.SingleOrDefault(map => map.Guid == guid);
        }

        public PermanentContentLinkMap Find(EPiServer.Core.ContentReference contentReference)
        {
            var pageMaps = _linkMaps.Where(map => map is PermanentContentLinkMap).Cast<PermanentContentLinkMap>();
            return pageMaps.SingleOrDefault(map => contentReference.CompareToIgnoreWorkID(map.ContentReference));
        }

        public PermanentLinkMap Find(EPiServer.UrlBuilder url)
        {
            var noLang = new UrlBuilder(url);
            noLang.QueryCollection.Remove("epslanguage");
            return _linkMaps.SingleOrDefault(map => map.MappedUrl == noLang.Uri || map.PermanentLinkUrl == noLang.Uri);
        }

        public void Clear()
        {
        }
        
        public void Initialize(IContentEvents contentEvents, IContentLoader contentQueryable, IEnterpriseSettings enterpriseSetting)
        {
            throw new NotImplementedException();
        }
    }
}