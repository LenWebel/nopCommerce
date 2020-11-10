
namespace Nop.Plugin.DiscountRules.PartialPayment
{
    /// <summary>
    /// Represents constants for the discount requirement rule
    /// </summary>
    public static class PartialPaymentRequirementDefaults
    {
        /// <summary>
        /// The system name of the discount requirement rule
        /// </summary>
        public const string SystemName = "DiscountRequirement.PartialPayment";

        /// <summary>
        /// The key of the settings to save restricted customer roles
        /// </summary>
        public const string SettingsKey = "DiscountRequirement.PartialPayment-{0}";

        /// <summary>
        /// The HTML field prefix for discount requirements
        /// </summary>
        public const string HtmlFieldPrefix = "DiscountRulesCustomerRoles{0}";
    }
}
