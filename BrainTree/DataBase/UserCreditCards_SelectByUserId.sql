ALTER proc [dbo].[UserCreditCards_SelectByUserId]
	@UserId nvarchar(128)

as

Begin

	SELECT [Id]
      		,[UserId]
      		,[ExternalCardIdNonce]
      		,[Last4DigitsCC]
	  	,[CardType]
	  	,[DateCreated]
	  	,[DateDeleted]
		,[DefaultCard]

	FROM [dbo].[UserCreditCards]
	Where UserId = @UserId
	AND DateDeleted is null

End


/*

---TEST CODE---
Declare @UserId nvarchar(128) = '5aac3baf-8822-4f04-9565-44d4fe460305' 
Exec dbo.UserCreditCards_SelectByUserId
@UserId

*/
