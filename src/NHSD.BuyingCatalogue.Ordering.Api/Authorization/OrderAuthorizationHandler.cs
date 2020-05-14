using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NHSD.BuyingCatalogue.Ordering.Common.Constants;

namespace NHSD.BuyingCatalogue.Ordering.Api.Authorization
{
    internal sealed class OrderAuthorizationHandler : IAuthorizationHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderAuthorizationHandler(IHttpContextAccessor httpContextAccessor) =>
            _httpContextAccessor = httpContextAccessor;

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (!ActionRequiresHandling(context.Resource as RouteEndpoint))
                return Task.CompletedTask;

            var user = context.User;

            if (!UserHasOrderingClaim(user))
                return Task.CompletedTask;

            if (!UserBelongsToRequestOrganisation(user))
                context.Fail();

            return Task.CompletedTask;
        }

        private static bool ActionRequiresHandling(Endpoint endpoint) =>
            endpoint?.Metadata?.GetMetadata<AuthenticateOrganisationAttribute>() != null;

        private static bool UserHasOrderingClaim(ClaimsPrincipal user) => user.HasClaim(c =>
            string.Equals(c.Type, ApplicationClaimTypes.Ordering, StringComparison.OrdinalIgnoreCase));

        private bool UserBelongsToRequestOrganisation(ClaimsPrincipal user)
        {
            // Currently works on the basis that the organization ID is the only parameter so will be the
            // last route value in the collection. It is also possible to look up the route value by name.
            var organisationIdRouteValue = _httpContextAccessor.HttpContext.Request.RouteValues.LastOrDefault();
            var requestedOrganisationId = organisationIdRouteValue.Value?.ToString();

            // Literal obviously isn't ideal but might be one for tech debt
            // Claims seem like a possible candidate for a NuGet package...
            var userOrganizationId = user.FindFirstValue("primaryOrganisationId");

            return string.Equals(userOrganizationId, requestedOrganisationId, StringComparison.OrdinalIgnoreCase);
        }
    }
}
