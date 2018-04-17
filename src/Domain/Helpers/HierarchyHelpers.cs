namespace Domain.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DataModel.Entities;

    public static class HierarchyHelpers
    {
        public static Category FindNode(
            this IEnumerable<Category> categories,
            string path)
        {
            return categories.FindNode(
                path,
                x => x.Id,
                x => x.ParentId,
                x => x.Name);
        }

        public static T FindNode<T, TKey>(
            this IEnumerable<T> collection,
            string path,
            Func<T, TKey> idFunc,
            Func<T, TKey?> parentKeyFunc,
            Func<T, string> nameFunc,
            char separator = '/')
            where TKey : struct
        {
            Ensure.IsNotNull(path, nameof(path));
            Ensure.IsNotNull(collection, nameof(collection));

            List<T> list = collection.ToList();

            string[] split = path.Split(
                new[] {separator},
                StringSplitOptions.RemoveEmptyEntries);

            T current = default(T);

            foreach (string s in split)
            {
                current = list.FirstOrDefault(
                    x => string.Equals(
                             s,
                             nameFunc(x),
                             StringComparison.InvariantCultureIgnoreCase) &&
                         (current == null || idFunc(current).Equals(parentKeyFunc(x))));

                if (current == null)
                {
                    return default(T);
                }
            }

            return current;
        }
    }
}