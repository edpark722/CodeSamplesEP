ALTER PROC [dbo].[JobTimeSlots_Update]
	@Id int
	,@Date datetime2(7) = null
	,@TimeStart int
	,@TimeEnd int
	,@Capacity int
	,@DayOfWeek nvarchar(20)
	,@TeamId int
	,@ScheduleType bit = 1
as



BEGIN

	UPDATE [dbo].[JobTimeSlots]
	SET [Date] = @Date
      		,[TimeStart] = @TimeStart
      		,[TimeEnd] = @TimeEnd
      		,[Capacity] = @Capacity
	  	,[ModifiedDate] = GETUTCDATE()
	  	,[DayOfWeek] = @DayOfWeek
	  	,[TeamId] = @TeamId
	  	,[ScheduleType] = @ScheduleType
	WHERE Id = @Id

END

/*

---TEST CODE---
DECLARE @Id INT = 1
DECLARE @Date datetime2(7) = null
DECLARE @TimeStart Int = 900
DECLARE @TimeEnd Int = 1100
DECLARE @Capacity Int = 4
DECLARE @DayOfWeek nvarchar(20) = 'Monday'
DECLARE @TeamId int = 1
DECLARE @ScheduleType bit = 1

SELECT * 
FROM [dbo].[JobTimeSlots]
WHERE Id = @Id

EXEC dbo.JobTimeSlots_Update
	@Id
	,@Date
	,@TimeStart
	,@TimeEnd
	,@Capacity
	,@DayOfWeek
	,@TeamId
	,@ScheduleType

SELECT * 
FROM [dbo].[JobTimeSlots]
WHERE Id = @Id


*/
