namespace DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DurationIntToLong : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AudioItem", "Duration", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AudioItem", "Duration", c => c.Int(nullable: false));
        }
    }
}
