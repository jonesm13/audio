namespace DataModel.Migrations
{
    using System.Data.Entity.Migrations;

    sealed class Configuration : DbMigrationsConfiguration<AudioDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AudioDbContext context)
        {
        }
    }
}
