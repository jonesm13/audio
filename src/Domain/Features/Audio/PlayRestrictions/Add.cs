namespace Domain.Features.Audio.PlayRestrictions
{
    using System;
    using System.Linq;
    using DataModel.Entities;
    using FluentValidation;
    using MediatR;
    using Pipeline;

    public class Add
    {
        public class Command : IRequest<CommandResult>
        {
            public Guid Id { get; set; }
            public string[] Days { get; set; }
            public int Start { get; set; }
            public int End { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Start)
                    .Must(BeNonNegativeInteger)
                    .Must(NotBeMoreThanNumberOfMinutesInADay);

                RuleFor(x => x.End)
                    .Must(BeGreaterThanStart)
                    .Must(NotBeMoreThanNumberOfMinutesInADay);

                RuleFor(x => x.Days)
                    .Must(AllBeDaysOfWeek)
                    .Must(AllBeDistinct);
            }

            bool AllBeDistinct(string[] arg)
            {
                return arg.Distinct().Count() == arg.Length;
            }

            bool AllBeDaysOfWeek(string[] arg)
            {
                string[] daysOfWeek = Enum.GetNames(typeof(DayOfTheWeek));

                return arg.All(x => daysOfWeek.Any(y => y.Equals(x, StringComparison.InvariantCultureIgnoreCase)));
            }

            bool BeGreaterThanStart(Command arg1, int arg2)
            {
                return arg2 > arg1.Start;
            }

            bool NotBeMoreThanNumberOfMinutesInADay(int arg)
            {
                return arg <= 7 * 24;
            }

            bool BeNonNegativeInteger(int arg)
            {
                return arg >= 0;
            }
        }
    }
}