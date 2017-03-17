ALTER PROC [dbo].[JobTimeSlots_SelectAllByTeamId]
	@TeamId int
	,@QueryDay nvarchar(20) = null

as


BEGIN
--Select for all DEFAULT time slots--
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
	Where TeamId = @TeamId AND [DATE] IS NULL AND (@QueryDay IS NULL OR [DayOfWeek] = @QueryDay)

	ORDER BY CASE [DayOfWeek]
		WHEN 'Monday' THEN 1
        	WHEN 'Tuesday' THEN 2
           	WHEN 'Wednesday' THEN 3
         	WHEN 'Thursday' THEN 4
		WHEN 'Friday' THEN 5
		WHEN 'Saturday' THEN 6
		WHEN 'Sunday' THEN 7
	END, [TimeStart] ASC

--Select for all Override time slots--	
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
	Where TeamId = @TeamId AND [DATE] IS NOT NULL AND [ScheduleType] = 1

--Select for ASAP time slot--
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
	Where TeamId = @TeamId AND [DATE] IS NOT NULL AND ([ScheduleType] = 0) AND ([DayOfWeek] = @QueryDay)

END

/*

---TEST CODE---
Declare @TeamId int = 1
DECLARE @QueryDay nvarchar(20) = 'Friday'
EXECUTE dbo.JobTimeSlots_SelectAllByTeamId
	@TeamId
	,@QueryDay

*/