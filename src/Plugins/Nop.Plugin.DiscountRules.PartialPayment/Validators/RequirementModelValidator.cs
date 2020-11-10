using FluentValidation;
using Nop.Plugin.DiscountRules.PartialPayment.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.DiscountRules.CustomerRoles.Validators
{
    /// <summary>
    /// Represents an <see cref="RequirementModel"/> validator.
    /// </summary>
    public class RequirementModelValidator : BaseNopValidator<RequirementModel>
    {
        public RequirementModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.DiscountId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.DiscountRules.PartialPayment.Fields.DiscountId.Required"));
            RuleFor(model => model.CustomerRoleId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.DiscountRules.PartialPayment.Fields.CustomerRoleId.Required"));
        }
    }
}
