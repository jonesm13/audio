namespace DataModel.Entities
{
    using System;
    using System.Collections.Generic;

    public class ArtistGroup : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Artist> Members { get; set; }
    }
}