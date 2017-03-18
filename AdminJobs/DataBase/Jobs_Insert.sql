ALTER Proc [dbo].[Jobs_Insert]
	@Id int Output
	,@UserId nvarchar(128) = null
	,@Status int = null
	,@JobType int = null
	,@Price decimal(11,2) = null
	,@Phone varchar(30) = null
	,@JobRequest int = null
	,@SpecialInstructions nvarchar(MAX) = null
	,@WebsiteId int = null
	,@ExternalCustomerId int = null

as


Begin

INSERT INTO [dbo].[Jobs]
	([UserId]
	,[Status]
        ,[JobType]
        ,[Price]
        ,[Phone]
	,[JobRequest]
	,[SpecialInstructions]
	,[WebsiteId]
	,[ExternalCustomerId])

Values
	(@UserId
	,@Status
	,@JobType
	,@Price
	,@Phone
	,@JobRequest
	,@SpecialInstructions
	,@WebsiteId
	,@ExternalCustomerId)

	Set @Id = SCOPE_IDENTITY()

End

/*

---TEST CODE---
Declare @Id int = 0;

Declare  @UserId nvarchar(128) = 5
	,@Status int = 9
	,@JobType int = 1
	,@Price decimal(11,2) = 99.00
	,@Phone varchar(30) = 9008888888
	,@JobRequest int = 2
	,@SpecialInstructions nvarchar(MAX) = null
	,@WebsiteId int = 55

INSERT INTO [dbo].[Jobs]
	([UserId]
	,[Status]
        ,[JobType]
        ,[Price]
        ,[Phone]
	,[JobRequest]
	,[SpecialInstructions]
	,[WebsiteId])

VALUES
	(@UserId
	,@Status
	,@JobType
	,@Price
	,@Phone
	,@JobRequest
	,@SpecialInstructions
	,@WebsiteId)

	Set @Id = SCOPE_IDENTITY()

	Select*
	From dbo.Jobs

*/