namespace DataModel.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddingAudioEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AudioItem",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        Duration = c.Int(nullable: false),
                        Flags = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AudioItem");
        }
    }
}
