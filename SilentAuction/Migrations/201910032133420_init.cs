namespace SilentAuction.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuctionPrizes",
                c => new
                    {
                        AuctionPrizeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ActualValue = c.Double(nullable: false),
                        MinimumBid = c.Double(nullable: false),
                        BidIncrement = c.Double(nullable: false),
                        CurrentBid = c.Double(nullable: false),
                        Description = c.String(),
                        TopParticipant = c.Int(),
                        Category = c.String(),
                        ParticipantId = c.Int(),
                        AuctionId = c.Int(nullable: false),
                        Paid = c.Boolean(),
                    })
                .PrimaryKey(t => t.AuctionPrizeId)
                .ForeignKey("dbo.Auctions", t => t.AuctionId, cascadeDelete: true)
                .ForeignKey("dbo.Participants", t => t.ParticipantId)
                .Index(t => t.ParticipantId)
                .Index(t => t.AuctionId);
            
            CreateTable(
                "dbo.Auctions",
                c => new
                    {
                        AuctionId = c.Int(nullable: false, identity: true),
                        ManagerId = c.Int(nullable: false),
                        Message = c.String(),
                        Day = c.DateTime(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        TotalRaised = c.Double(nullable: false),
                        Donors = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.AuctionId)
                .ForeignKey("dbo.Managers", t => t.ManagerId, cascadeDelete: true)
                .Index(t => t.ManagerId);
            
            CreateTable(
                "dbo.Managers",
                c => new
                    {
                        ManagerId = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        EmailAddress = c.String(),
                    })
                .PrimaryKey(t => t.ManagerId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Participants",
                c => new
                    {
                        ParticipantId = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(maxLength: 128),
                        EmailAddress = c.String(),
                        TotalSpent = c.Double(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        RaffleTickets = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ParticipantId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.Data",
                c => new
                    {
                        DataId = c.Int(nullable: false, identity: true),
                        Time = c.Double(nullable: false),
                        Money = c.Double(nullable: false),
                        AuctionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DataId)
                .ForeignKey("dbo.Auctions", t => t.AuctionId, cascadeDelete: true)
                .Index(t => t.AuctionId);
            
            CreateTable(
                "dbo.ParticipantActions",
                c => new
                    {
                        ParticipantActionId = c.Int(nullable: false, identity: true),
                        Action = c.String(),
                        ParticipantId = c.Int(nullable: false),
                        Time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ParticipantActionId)
                .ForeignKey("dbo.Participants", t => t.ParticipantId, cascadeDelete: true)
                .Index(t => t.ParticipantId);
            
            CreateTable(
                "dbo.RafflePrizes",
                c => new
                    {
                        RafflePrizeId = c.Int(nullable: false, identity: true),
                        RaffleId = c.Int(nullable: false),
                        Value = c.Double(nullable: false),
                        Description = c.String(),
                        WinnerId = c.Int(),
                        CurrentTickets = c.Int(nullable: false),
                        Category = c.String(),
                    })
                .PrimaryKey(t => t.RafflePrizeId)
                .ForeignKey("dbo.Raffles", t => t.RaffleId, cascadeDelete: true)
                .Index(t => t.RaffleId);
            
            CreateTable(
                "dbo.Raffles",
                c => new
                    {
                        RaffleId = c.Int(nullable: false, identity: true),
                        Day = c.DateTime(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        TotalRaised = c.Double(nullable: false),
                        Donors = c.String(),
                        Name = c.String(),
                        ManagerId = c.Int(nullable: false),
                        Description = c.String(),
                        CostPerTicket = c.Int(nullable: false),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.RaffleId)
                .ForeignKey("dbo.Managers", t => t.ManagerId, cascadeDelete: true)
                .Index(t => t.ManagerId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.tblAuctionPrizes",
                c => new
                    {
                        AuctionPrizeId = c.Int(nullable: false, identity: true),
                        ActualValue = c.Double(nullable: false),
                        MinimumBid = c.Double(nullable: false),
                        BidIncrement = c.Double(nullable: false),
                        CurrentBid = c.Double(nullable: false),
                        Description = c.String(),
                        Picture = c.String(),
                        TopParticipant = c.Int(),
                        Category = c.String(),
                        WinnerId = c.Int(),
                        AuctionId = c.Int(nullable: false),
                        ImagePath = c.String(),
                    })
                .PrimaryKey(t => t.AuctionPrizeId)
                .ForeignKey("dbo.Auctions", t => t.AuctionId, cascadeDelete: true)
                .Index(t => t.AuctionId);
            
            CreateTable(
                "dbo.tblRafflePrizes",
                c => new
                    {
                        RafflePrizeId = c.Int(nullable: false, identity: true),
                        RaffleId = c.Int(nullable: false),
                        Value = c.Double(nullable: false),
                        Description = c.String(),
                        ImagePath = c.String(),
                        WinnerId = c.Int(),
                        CurrentTickets = c.Int(nullable: false),
                        Category = c.String(),
                    })
                .PrimaryKey(t => t.RafflePrizeId)
                .ForeignKey("dbo.Raffles", t => t.RaffleId, cascadeDelete: true)
                .Index(t => t.RaffleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tblRafflePrizes", "RaffleId", "dbo.Raffles");
            DropForeignKey("dbo.tblAuctionPrizes", "AuctionId", "dbo.Auctions");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.RafflePrizes", "RaffleId", "dbo.Raffles");
            DropForeignKey("dbo.Raffles", "ManagerId", "dbo.Managers");
            DropForeignKey("dbo.ParticipantActions", "ParticipantId", "dbo.Participants");
            DropForeignKey("dbo.Data", "AuctionId", "dbo.Auctions");
            DropForeignKey("dbo.AuctionPrizes", "ParticipantId", "dbo.Participants");
            DropForeignKey("dbo.Participants", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AuctionPrizes", "AuctionId", "dbo.Auctions");
            DropForeignKey("dbo.Auctions", "ManagerId", "dbo.Managers");
            DropForeignKey("dbo.Managers", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.tblRafflePrizes", new[] { "RaffleId" });
            DropIndex("dbo.tblAuctionPrizes", new[] { "AuctionId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Raffles", new[] { "ManagerId" });
            DropIndex("dbo.RafflePrizes", new[] { "RaffleId" });
            DropIndex("dbo.ParticipantActions", new[] { "ParticipantId" });
            DropIndex("dbo.Data", new[] { "AuctionId" });
            DropIndex("dbo.Participants", new[] { "ApplicationUserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Managers", new[] { "ApplicationUserId" });
            DropIndex("dbo.Auctions", new[] { "ManagerId" });
            DropIndex("dbo.AuctionPrizes", new[] { "AuctionId" });
            DropIndex("dbo.AuctionPrizes", new[] { "ParticipantId" });
            DropTable("dbo.tblRafflePrizes");
            DropTable("dbo.tblAuctionPrizes");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Raffles");
            DropTable("dbo.RafflePrizes");
            DropTable("dbo.ParticipantActions");
            DropTable("dbo.Data");
            DropTable("dbo.Participants");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Managers");
            DropTable("dbo.Auctions");
            DropTable("dbo.AuctionPrizes");
        }
    }
}
