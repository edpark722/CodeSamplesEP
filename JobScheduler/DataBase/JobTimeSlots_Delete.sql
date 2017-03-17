ALTER PROC [dbo].[JobTimeSlots_Delete]
		@Id int

as



BEGIN
	
	DELETE FROM [dbo].[JobTimeSlots]
      	WHERE Id = @Id

END

/*

---Test Code---
EXEC dbo.JobTimeSlots_Delete
     @Id int = 1

*/