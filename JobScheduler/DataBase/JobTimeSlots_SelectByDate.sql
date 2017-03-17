ALTER PROC [dbo].[JobTimeSlots_SelectByDate]
	@TeamId int
	,@ScheduleType bit = 1
	,@QueryDate datetime2(7) = null
	,@QueryDay nvarchar(20) = null
		
		
as


BEGIN

IF @ScheduleType = 0
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
	Where (TeamId = @TeamId) AND (@QueryDay IS NULL OR [DayOfWeek] = @QueryDay) AND ([Date] = @QueryDate) AND ([ScheduleType] = 0)
	

ELSE IF @QueryDate IS NULL

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
	Where (TeamId = @TeamId) AND (@QueryDay IS NULL OR [DayOfWeek] = @QueryDay) AND ([Date] IS NULL) AND ([ScheduleType] = 1)
	
	ORDER BY TimeStart ASC

	
ELSE IF @QueryDate IS NOT NULL

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
	Where (TeamId = @TeamId) AND (@QueryDay IS NULL OR [DayOfWeek] = @QueryDay) AND ([Date] = @QueryDate) AND ([ScheduleType] = 1)
	
	ORDER BY TimeStart ASC

END

/*

---TEST CODE---
DECLARE @TeamId int = 55
DECLARE @ScheduleType bit = 0
DECLARE @QueryDate datetime2(7) = '2017-03-02 00:00:00.0000000'
DECLARE @QueryDay nvarchar(20) = 'Thursday'

EXEC dbo.JobTimeSlots_SelectByDate
		@TeamId
		,@ScheduleType
		,@QueryDate
		,@QueryDay

*/