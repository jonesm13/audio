namespace DataModel.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

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
        [Index(IsUnique = true)]
        [StringLength(150)]
        public string Name { get; set; }

        public virtual ICollection<ArtistGroup> Groups { get; set; }
        public virtual ICollection<AudioItem> AudioItems { get; set; }
    }
}