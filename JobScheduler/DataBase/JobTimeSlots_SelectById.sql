ALTER PROC [dbo].[JobTimeSlots_SelectById]
	@Id int

as


Begin

	SELECT [Id]
      		,[Date]
      		,[CreatedDate]
	  	,[ModifiedDate]
      		,[TimeStart]
      		,[TimeEnd]
      		,[Capacity]
	  	,[DefaultId]
	  	,[DayOfWeek]
	  	,[TeamId]
	  	,[ScheduleType]
	FROM [dbo].[JobTimeSlots]
	Where Id = @Id

END

/*

---TEST CODE---
Declare @Id int = 1
EXEC dbo.JobTimeSlots_SelectById
@Id

*/