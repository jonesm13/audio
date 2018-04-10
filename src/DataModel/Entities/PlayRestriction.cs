namespace DataModel.Entities
{
    using System;

    public class PlayRestriction : IEntity
    {
        public Guid Id { get; set; }
        public Guid AudioItemId { get; set; }
        public DayOfTheWeek Days { get; set; }
        public int Start { get; set; }
        public int End { get; set; }

        public virtual AudioItem AudioItem { get; set; }
    }

    [Flags]
    public enum DayOfTheWeek
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64
    }
}