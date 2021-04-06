using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.IdentityModel.Tokens.Jwt;

namespace DotsApi.Authorization
{
    public class UserUDSelfAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, string>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement, 
            string resource)
        {
            if (context.User == null || resource == null)
                return Task.CompletedTask;

            if (requirement.Name != Constants.UpdateOperationName &&
                requirement.Name != Constants.DeleteOperationName)
                return Task.CompletedTask;

            if (context.User.HasClaim(JwtRegisteredClaimNames.Sub, resource))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
