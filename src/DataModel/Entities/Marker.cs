namespace DataModel.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Marker : IEntity
    {
        public Guid Id { get; set; }
        [Index("IX_MarkerKey", 1, IsUnique = true)]
        public Guid AudioItemId { get; set; }
        [Index("IX_MarkerKey", 2, IsUnique = true)]
        public long Offset { get; set; }
        [Index("IX_MarkerKey", 3, IsUnique = true)]
        [StringLength(50)]
        public string Type { get; set; }
        public string State { get; set; }

        public virtual AudioItem AudioItem { get; set; }
    }
}