ALTER proc [dbo].[Jobs_DeleteById]
	@Id int

as


Begin

	Delete dbo.Jobs
	Where Id = @Id

End

/*

---TEST CODE---
Exec dbo.Jobs_Delete
@Id = 1

Select *
From dbo.Jobs

*/


