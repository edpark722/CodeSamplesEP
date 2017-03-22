ALTER Proc [dbo].[UserCreditCards_SoftDeleteByNonce]
	@ExternalCardIdNonce nvarchar(128)
	
as

Begin
	Update dbo.UserCreditCards
	Set
		DateDeleted = getutcdate(),
		DefaultCard = 0
	Where 
		ExternalCardIdNonce = @ExternalCardIdNonce

End

/*

---TEST CODE---
Declare @ExternalCardIdNonce nvarchar(128) = '49d68a8a-c70c-05df-1a25-a72f400c0771'

Select*
From dbo.UserCreditCards
Where ExternalCardIdNonce = @ExternalCardIdNonce

Exec dbo.UserCreditCards_UpdateByNonce
	@ExternalCardIdNonce

Select*
From dbo.UserCreditCards
Where ExternalCardIdNonce = @ExternalCardIdNonce

*/

