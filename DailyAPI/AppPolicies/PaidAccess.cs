using Microsoft.AspNetCore.Authorization;

namespace DailyAPI.AppPolicies;

public class PaidAccess : IAuthorizationRequirement
{
}

public class PaidAccessHandler : AuthorizationHandler<PaidAccess>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PaidAccess requirement)
    {
        if (!context.User.HasClaim(x => x.Type == "PaidUser"))
        {
            context.Fail();
            return Task.CompletedTask;

        }
        var dateToCheck = DateTime.Parse(context.User.FindFirst(x => x.Type == "PaidUser").Value);
        if (dateToCheck < DateTime.Now)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}