using System;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;

namespace WhiteMagic.Tests
{
    public static class ContentExtensions
    {
        public static PageReference SaveAndPublish<T>(this T page, IContentRepository repository) where T : IContent
        {
            repository.Save(page, SaveAction.Publish, AccessLevel.NoAccess);
            return page.ContentLink.ToPageReference();
        }

        public static T With<T>(this T page, Action<T> with) where T : IContent
        {
            with(page);
            return page;
        }
    }
}