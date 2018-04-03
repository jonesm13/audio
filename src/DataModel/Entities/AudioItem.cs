namespace DataModel.Entities
{
    using System;
    using System.Collections.Generic;

    public class AudioItem : IEntity
    {
        public AudioItem()
        {
// ReSharper disable once VirtualMemberCallInConstructor
            Categories = new HashSet<Category>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public AudioItemFlags Flags { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Marker> Markers { get; set; }
    }

    [Flags]
    public enum AudioItemFlags
    {
        None = 0,
        Ready = 1
    }
}