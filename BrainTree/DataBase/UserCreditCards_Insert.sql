ALTER proc [dbo].[UserCreditCards_Insert]
	@Id int Output
	,@UserId nvarchar(128)
	,@ExternalCardIdNonce nvarchar(128)
	,@Last4DigitsCC nvarchar(4)
	,@CardType nvarchar(50)

as


Begin
	Insert into dbo.UserCreditCards
		([UserId]
        	,[ExternalCardIdNonce]
        	,[Last4DigitsCC]
		,[CardType])
	Values
		(@UserId
		,@ExternalCardIdNonce
		,@Last4DigitsCC
		,@CardType)

	Set @Id = scope_identity()


End

/*

---TEST CODE---
DECLARE @Id int 
DECLARE @UserId nvarchar(128) = 'e957b7f9-62d5-4129-b90b-1dd9d94911b4'
DECLARE @ExternalCardIdNonce nvarchar(128) = 'asdf'
DECLARE @Last4DigitsCC nvarchar(4) = '88'
DECLARE @CardType nvarchar(50) = 'Visa'

Exec dbo.UserCreditCards_Insert
	@Id Output
	,@UserId
	,@ExternalCardIdNonce
	,@Last4DigitsCC
	,@CardType

Select @Id

Select *
From dbo.UserCreditCards
Where Id = @Id

*/


