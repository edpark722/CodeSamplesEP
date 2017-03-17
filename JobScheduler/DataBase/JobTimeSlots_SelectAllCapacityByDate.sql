ALTER PROC [dbo].[JobTimeSlots_SelectAllCapacityByDate_v2]
	@QueryDate datetime2(7) = null

			

as


BEGIN

SELECT count(a.JobId) as CurrentUsed
	, a.[ScheduleId]
FROM dbo.[JobSchedule] a WITH (NOLOCK) Left Outer Join
	dbo.Jobs b WITH (NOLOCK) on a.JobId = b.[Id]
WHERE a.[Date] = @QueryDate AND b.[Status] IN (0,1,2,3,4,6,11)
Group By a.[ScheduleId]

END

/*

---TEST CODE---
Declare @QueryDate datetime2(7) = '2017-03-06 00:00:00.0000000'

EXECUTE dbo.[JobTimeSlots_SelectAllCapacityByDate_v2]
	@QueryDate

*/