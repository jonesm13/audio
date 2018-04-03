namespace DataModel.Entities
{
    using System;

    public class Marker : IEntity
    {
        public Guid Id { get; set; }
        public Guid AudioItemId { get; set; }
        public long Offset { get; set; }
        public MarkerType Type { get; set; }
        public string CustomName { get; set; }
        public string Command { get; set; }

        public virtual AudioItem AudioItem { get; set; }
    }

    public enum MarkerType
    {
        IntroStart,
        IntroEnd,
        HookStart,
        HookEnd,
        Segue,
        Command,
        Custom
    }
}