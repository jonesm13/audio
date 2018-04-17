namespace Domain.Pipeline
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using FluentValidation.Results;
    using log4net;
    using MediatR;
    using MediatR.Pipeline;

    public class HandlerDecorator<TRequest, TResponse> :
        IRequestHandler<TRequest, TResponse>
            where TRequest : class, IRequest<TResponse>
    {
        static readonly ILog Log = LogManager.GetLogger("Pipeline");

        readonly IRequestHandler<TRequest, TResponse> inner;
        readonly IEnumerable<IValidator<TRequest>> validators;
        readonly IEnumerable<IRequestPreProcessor<TRequest>> preProcessors;
        readonly IMediator mediator;

        public HandlerDecorator(
            IRequestHandler<TRequest, TResponse> inner,
            IEnumerable<IValidator<TRequest>> validators,
            IEnumerable<IRequestPreProcessor<TRequest>> preProcessors,
            IMediator mediator)
        {
            this.inner = inner;
            this.validators = validators;
            this.preProcessors = preProcessors;
            this.mediator = mediator;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken)
        {
            Log.Debug($"Start {request.GetType().FullName}");

            foreach (IRequestPreProcessor<TRequest> preProcessor in preProcessors)
            {
                await preProcessor.Process(request, cancellationToken);
            }

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

            TResponse result = await inner.Handle(request, cancellationToken);

            CommandResult commandResult = result as CommandResult;

            if (commandResult != null)
            {
                await Task.WhenAll(
                    commandResult
                        .GetNotifications()
                        .Select(n => mediator.Publish(n, cancellationToken)));
            }

            Log.Debug($"Done {request.GetType().FullName}");

            return result;
        }
    }
}