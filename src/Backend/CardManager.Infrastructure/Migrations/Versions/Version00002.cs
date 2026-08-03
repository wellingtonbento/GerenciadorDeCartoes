using System.Data;
using FluentMigrator;

namespace CardManager.Infrastructure.Migrations.Versions
{
    [Migration(DatabaseVersions.TABLE_CARD, "Create table to save the card's information")]
    public class Version00002 : VersionBase
    {
        public override void Up()
        {
            CreateTable("Cards")
                .WithColumn("UserId").AsInt64().NotNullable().ForeignKey("Users", "Id").OnDelete(Rule.Cascade)
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("Type").AsInt32().NotNullable()
                .WithColumn("CreditLimit").AsDecimal(18, 2).NotNullable()
                .WithColumn("AmountSpent").AsDecimal(18, 2).NotNullable()
                .WithColumn("DebitBalance").AsDecimal(18, 2).NotNullable();
        }
    }
}
