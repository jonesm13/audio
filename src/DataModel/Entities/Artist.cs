namespace DataModel.Entities
{
    using System;
    using System.Collections.Generic;

    public class Artist : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ArtistGroup> Groups { get; set; }
    }
}