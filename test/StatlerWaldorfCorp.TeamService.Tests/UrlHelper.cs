using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace StatlerWaldorfCorp.TeamService.Tests
{
    class UrlHelper : IUrlHelper
    {
        public string Action(UrlActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        public string Content(string contentPath)
        {
            throw new NotImplementedException();
        }

        public bool IsLocalUrl(string url)
        {
            throw new NotImplementedException();
        }

        public string RouteUrl(UrlRouteContext routeContext)
        {
            throw new NotImplementedException();
        }

        public string Link(string routeName, object values)
        {
            return String.Empty;
        }

        public ActionContext ActionContext { get; }
    }
}
