ALTER PROC [dbo].[JobTimeSlots_SelectCapacityByDate_v2]
	@QueryDate datetime2(7) = null
	,@QueryScheduleId int = null
			

as


BEGIN

	SELECT (Capacity) As [CurrentAvailable]
	FROM dbo.JobTimeSlots 
	WHERE ([Id] = @QueryScheduleId) 

	SELECT count(a.jobId) As [CurrentUsed]
	FROM dbo.[JobSchedule] a WITH (NOLOCK) Left Outer Join
	dbo.Jobs b WITH (NOLOCK) on a.JobId = b.[Id]
	WHERE [Date] = @QueryDate AND [ScheduleId] = @QueryScheduleId AND b.[Status] IN (0,1,2,3,4,6,11)

END

/*

---TEST CODE---
Declare @QueryDate datetime2(7) = '2017-03-06 00:00:00.0000000'
DECLARE @QueryScheduleId int = 41
EXECUTE dbo.[JobTimeSlots_SelectCapacityByDate_v2]
	@QueryDate
	,@QueryScheduleId

*/