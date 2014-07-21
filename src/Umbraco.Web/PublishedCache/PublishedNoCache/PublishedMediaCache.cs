﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Core.Xml;
using Umbraco.Core.Xml.XPath;
using Umbraco.Core.Models;

namespace Umbraco.Web.PublishedCache.PublishedNoCache
{
    class PublishedMediaCache : IPublishedMediaCache
    {
        private readonly IMediaService _mediaService;

        public PublishedMediaCache(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public IPublishedContent GetById(bool preview, int contentId)
        {
            var content = _mediaService.GetById(contentId);
            return content == null ? null : (new PublishedMedia(content, this, preview)).CreateModel();
        }

        public IEnumerable<IPublishedContent> GetAtRoot(bool preview)
        {
            var content = _mediaService.GetRootMedia();
            return content.Select(c => (new PublishedMedia(c, this, preview)).CreateModel());
        }

        public IPublishedContent GetSingleByXPath(bool preview, string xpath, params XPathVariable[] vars)
        {
            var navigator = GetXPathNavigator(preview);
            var iterator = navigator.Select(xpath, vars);
            return GetSingleByXPath(iterator);
        }

        public IPublishedContent GetSingleByXPath(bool preview, XPathExpression xpath, params XPathVariable[] vars)
        {
            var navigator = GetXPathNavigator(preview);
            var iterator = navigator.Select(xpath, vars);
            return GetSingleByXPath(iterator);
        }

        private static IPublishedContent GetSingleByXPath(XPathNodeIterator iterator)
        {
            if (iterator.MoveNext() == false) return null;

            var xnav = iterator.Current as NavigableNavigator;
            if (xnav == null) return null;

            var xcontent = xnav.UnderlyingObject as Navigable.NavigableContent;
            return xcontent == null ? null : xcontent.InnerContent;
        }

        public IEnumerable<IPublishedContent> GetByXPath(bool preview, string xpath, params XPathVariable[] vars)
        {
            var navigator = GetXPathNavigator(preview);
            var iterator = navigator.Select(xpath, vars);
            return GetByXPath(iterator);
        }

        public IEnumerable<IPublishedContent> GetByXPath(bool preview, XPathExpression xpath, params XPathVariable[] vars)
        {
            var navigator = GetXPathNavigator(preview);
            var iterator = navigator.Select(xpath, vars);
            return GetByXPath(iterator);
        }

        private static IEnumerable<IPublishedContent> GetByXPath(XPathNodeIterator iterator)
        {
            while (iterator.MoveNext())
            {
                var xnav = iterator.Current as NavigableNavigator;
                if (xnav == null) continue;

                var xcontent = xnav.UnderlyingObject as Navigable.NavigableContent;
                if (xcontent == null) continue;

                yield return (xcontent.InnerContent);
            }
        }

        public XPathNavigator GetXPathNavigator(bool preview)
        {
            var source = new Navigable.Source(this, preview);
            var navigator = new NavigableNavigator(source);
            return navigator;
        }

        public bool XPathNavigatorIsNavigable { get { return true; } }

        public bool HasContent(bool preview)
        {
            return GetAtRoot(preview).Any();
        }
    }
}