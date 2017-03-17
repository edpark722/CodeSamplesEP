ALTER PROC [dbo].[JobTimeSlots_SelectAvailableByDate]
	@TeamId int
	,@QueryDate datetime2(7) = null
	,@QueryDay nvarchar(20) = null
			

as


BEGIN

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
	 
	FROM [dbo].[JobTimeSlots] d WITH (NOLOCK)
	Where (TeamId = @TeamId) AND (@QueryDay IS NULL OR [DayOfWeek] = @QueryDay) AND ([ScheduleType] = 1) AND ([DATE] IS NULL OR @QueryDate IS NULL OR [Date] = @QueryDate)
	ORDER BY [TimeStart] ASC

END

/*

---TEST CODE---
Declare @TeamId int = 1
Declare @QueryDate datetime2(7) = '2017-02-27 00:00:00.0000000'
DECLARE @QueryDay nvarchar(20) = 'Monday'
EXECUTE dbo.JobTimeSlots_SelectAvailableByDate
	@TeamId
	,@QueryDate
	,@QueryDay

*/