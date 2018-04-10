namespace DataModel.Entities
{
    using System;
    using System.Collections.Generic;

    public class Artist : IEntity
    {
        public Artist()
        {
// ReSharper disable VirtualMemberCallInConstructor
            Groups = new HashSet<ArtistGroup>();
            AudioItems = new HashSet<AudioItem>();
// ReSharper enable VirtualMemberCallInConstructor
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ArtistGroup> Groups { get; set; }
        public virtual ICollection<AudioItem> AudioItems { get; set; }
    }
}