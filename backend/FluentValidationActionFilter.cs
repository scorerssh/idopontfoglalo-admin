using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApartManBackend
{
    public class FluentValidationActionFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public FluentValidationActionFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var arg in context.ActionArguments.Values)
            {
                if (arg is null)
                {
                    continue;
                }

                var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;
                if (validator is null)
                {
                    continue;
                }

                ValidationResult result = await validator.ValidateAsync(
                    new ValidationContext<object>(arg),
                    context.HttpContext.RequestAborted);

                if (!result.IsValid)
                {
                    context.Result = new BadRequestObjectResult(result.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    }));

                    return;
                }
            }

            await next();
        }
    }
}
