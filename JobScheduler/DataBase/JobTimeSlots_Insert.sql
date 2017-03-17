ALTER PROC [dbo].[JobTimeSlots_Insert]
	@Id int OUTPUT
	,@Date datetime2(7) = null
	,@TimeStart int
	,@TimeEnd int
	,@Capacity int
	,@DefaultId int = null
	,@DayOfWeek nvarchar(20)
	,@TeamId int = 1
	,@ScheduleType bit = 1

as


BEGIN

INSERT INTO [dbo].[JobTimeSlots]
        ([Date]
        ,[TimeStart]
        ,[TimeEnd]
        ,[Capacity]
	,[DefaultId]
	,[DayOfWeek]
	,[TeamId]
	,[ScheduleType])
VALUES
        (@Date
	,@TimeStart 
	,@TimeEnd
	,@Capacity
	,@DefaultId
	,@DayOfWeek
	,@TeamId
	,@ScheduleType)

	SET @Id = SCOPE_IDENTITY()

END




/*
---TEST CODE---
DECLARE @Id int
DECLARE @Date datetime2(7) = null
			,@TimeStart int = 0900
			,@TimeEnd int = 1100
			,@Capacity int = 4
			,@DefaultId int = null
			,@DayOfWeek nvarchar(20) = 'Wednesday'
			,@TeamId int = 1
			,@ScheduleType bit = 1
EXEC dbo.JobTimeSlots_Insert
			@Id OUTPUT
			,@Date
			,@TimeStart
			,@TimeEnd
			,@Capacity
			,@DefaultId
			,@DayOfWeek
			,@TeamId
			,@ScheduleType
SELECT @Id
SELECT *
FROM dbo.JobTimeSlots
WHERE Id = @Id

*/