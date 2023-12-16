using FluentMigrator;

namespace bkmarker.Migrations;

[Migration(202312161654)]
public class AddUserTable : Migration
{
    public override void Up()
    {
        Create.Table("users")
          .WithColumn("id").AsInt64().PrimaryKey().Identity()
          .WithColumn("email").AsString()
          .WithColumn("password").AsString();
    }

    public override void Down()
    {
        Delete.Table("users");
    }
}

