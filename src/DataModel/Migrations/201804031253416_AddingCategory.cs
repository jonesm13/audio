namespace DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingCategory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        ParentId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AudioCategories",
                c => new
                    {
                        AudioItemId = c.Guid(nullable: false),
                        CategoryId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.AudioItemId, t.CategoryId })
                .ForeignKey("dbo.AudioItem", t => t.AudioItemId, cascadeDelete: true)
                .ForeignKey("dbo.Category", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.AudioItemId)
                .Index(t => t.CategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AudioCategories", "CategoryId", "dbo.Category");
            DropForeignKey("dbo.AudioCategories", "AudioItemId", "dbo.AudioItem");
            DropIndex("dbo.AudioCategories", new[] { "CategoryId" });
            DropIndex("dbo.AudioCategories", new[] { "AudioItemId" });
            DropTable("dbo.AudioCategories");
            DropTable("dbo.Category");
        }
    }
}
