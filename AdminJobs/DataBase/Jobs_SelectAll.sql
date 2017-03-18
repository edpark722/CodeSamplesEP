ALTER proc [dbo].[Jobs_SelectAll_v2]
	@CurrentPage int = 1
	,@ItemsPerPage int = 5
	,@Query nvarchar(50) = null
	,@QueryWebsiteId int = null
	,@QueryStatus int = null
	,@QueryJobType int = null
	,@QueryStartDate datetime2(7) = null
	,@QueryEndDate datetime2(7) = null
	,@QueryNewDate datetime2(7) = null


As


Begin

IF (@QueryStartDate IS NOT NULL AND @QueryEndDate IS NULL)
	Begin			
		Set @QueryNewDate = DATEADD(day, 1, @QueryStartDate)
	End
ELSE IF (@QueryStartDate IS NOT NULL AND @QueryEndDate IS NOT NULL)
	Begin
		Set @QueryNewDate = DATEADD(day, 1, @QueryEndDate)
	End

SELECT [Id]
	,[UserId]
	,[Status]
        ,[JobType]
        ,[Price]
        ,[Phone]
        ,[JobRequest]
        ,[SpecialInstructions]
        ,[Created]
        ,[Modified]
	,[WebsiteId]

From dbo.Jobs
WHERE (@QueryWebsiteId IS NULL OR WebsiteId = @QueryWebsiteId)
	AND (@QueryStatus IS NULL OR [Status] = @QueryStatus)
	AND (@QueryJobType IS NULL OR [JobType] = @QueryJobType)
	AND (@QueryStartDate IS NULL OR [Created] >= @QueryStartDate)
	AND (@QueryNewDate IS NULL OR [Created] <= @QueryNewDate)
	AND (@Query IS NULL OR [UserId] Like ('%'+@Query+'%') OR [Phone] Like ('%'+@Query+'%'))

	ORDER BY [Created] DESC

	OFFSET ((@CurrentPage - 1) * @ItemsPerPage) ROWS
             FETCH NEXT  @ItemsPerPage ROWS ONLY 	

	
SELECT COUNT('Id')
FROM [dbo].[Jobs]
WHERE (@QueryWebsiteId IS NULL OR WebsiteId = @QueryWebsiteId)
	AND (@QueryStatus IS NULL OR [Status] = @QueryStatus)
	AND (@QueryJobType IS NULL OR [JobType] = @QueryJobType)
	AND (@QueryStartDate IS NULL OR [Created] >= @QueryStartDate)
	AND (@QueryNewDate IS NULL OR [Created] <= @QueryNewDate)
	AND (@Query IS NULL OR [UserId] Like ('%'+@Query+'%') OR [Phone] Like ('%'+@Query+'%'))
	



End

/*

---TEST CODE---
Declare @CurrentPage int = 1
,@ItemsPerpage int = 10
,@QueryStartDate nvarchar(100) = '2017-02-21'
Exec dbo.Jobs_SelectAll_v2

*/