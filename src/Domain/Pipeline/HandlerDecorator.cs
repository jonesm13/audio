namespace Domain.Pipeline
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using FluentValidation.Results;
    using MediatR;

    public class HandlerDecorator<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse> where TRequest : class, IRequest<TResponse>
    {
        readonly IRequestHandler<TRequest, TResponse> inner;
        readonly IEnumerable<IValidator<TRequest>> validators;

        public HandlerDecorator(
            IRequestHandler<TRequest, TResponse> inner,
            IEnumerable<IValidator<TRequest>> validators)
        {
            this.inner = inner;
            this.validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken)
        {
            ValidationContext context = new ValidationContext(request);

            List<ValidationFailure> failures = validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .ToList();

            if (failures.Any())
            {
                throw new ValidationException(failures);
            }

            return await inner.Handle(request, cancellationToken);
        }
    }
}