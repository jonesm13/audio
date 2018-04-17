namespace Domain.Helpers
{
    using System.Net;
    using FluentValidation;

    public static class ValidationHelpers
    {
        public static IRuleBuilderOptions<T, TProperty> WithHttpStatusCode<T, TProperty>(
            this IRuleBuilderOptions<T, TProperty> rule,
            HttpStatusCode code)
        {
            return rule.WithErrorCode(((int)code).ToString());
        }
    }
}