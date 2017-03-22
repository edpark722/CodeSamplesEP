ALTER Proc [dbo].[UserCreditCards_UpdateDefaultByUserId]
	@UserId nvarchar(128)
	,@ExternalCardIdNonce nvarchar(128)
as


Begin

	UPDATE [dbo].[UserCreditCards]
	SET 
		[DefaultCard] = 0
		
	WHERE UserId = @UserId
	

	UPDATE [dbo].[UserCreditCards]
	SET
		[DefaultCard] = 1

	Where ExternalCardIdNonce = @ExternalCardIdNonce


End


/*

---TEST CODE---
DECLARE @UserId nvarchar(128) = '610e4188-b9a8-43ae-a59c-8570f8af973e'
DECLARE @ExternalCardIdNonce nvarchar(128) = 'a335e712-d0e5-0d87-13f7-2d3a2cd29e18'

Select *
From dbo.UserCreditCards
Where UserId = @UserId

Exec dbo.UserCreditCards_UpdateDefaultByUserId
	@UserId
	,@ExternalCardIdNonce


Select *
From dbo.UserCreditCards
Where UserId = @UserId

*/
