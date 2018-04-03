namespace DataModel.Entities
{
    using System;
    using System.Collections.Generic;

    public class Category : IEntity
    {
        public Category()
        {
// ReSharper disable once VirtualMemberCallInConstructor
            AudioItems = new HashSet<AudioItem>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }

        public virtual ICollection<AudioItem> AudioItems { get; set; }
    }
}