namespace Domain.Tests
{
    using System;
    using Helpers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HierarchyHelperTests
    {
        [TestMethod]
        public void CanFindLeafNode()
        {
            string path = "songs/rotations/a-list/";

            Widget actual = Widget
                .GetHierarchy()
                .FindNode(path, w => w.Id, w => w.ParentId, w => w.Name);

            Assert.IsNotNull(actual);
            Assert.AreEqual("a-list", actual.Name);
        }

        [TestMethod]
        public void CanFindRootNode()
        {
            string path = "songs";

            Widget actual = Widget
                .GetHierarchy()
                .FindNode(path, w => w.Id, w => w.ParentId, w => w.Name);

            Assert.IsNotNull(actual);
            Assert.AreEqual("songs", actual.Name);
        }

        [TestMethod]
        public void CannotFindNonExistantNode()
        {
            string path = "songs/rotations/c-list";

            Widget actual = Widget
                .GetHierarchy()
                .FindNode(path, w => w.Id, w => w.ParentId, w => w.Name);

            Assert.IsNull(actual);
        }
    }

    public class Widget
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }

        public static Widget[] GetHierarchy()
        {
            Guid root = Guid.NewGuid();
            Guid childA = Guid.NewGuid();
            Guid grandchildA = Guid.NewGuid();
            Guid grandchildB = Guid.NewGuid();

            return new[]
            {
                new Widget {Id = root, Name = "songs"},
                new Widget {Id = childA, Name = "rotations", ParentId = root},
                new Widget {Id = grandchildA, Name = "a-list", ParentId = childA},
                new Widget {Id = grandchildB, Name = "b-list", ParentId = childA}
            };
        }
    }
}
