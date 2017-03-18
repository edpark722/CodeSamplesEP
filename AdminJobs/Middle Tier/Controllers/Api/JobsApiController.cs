using Microsoft.Practices.Unity;
using bringpro.Web.Domain;
using bringpro.Web.Enums;
using bringpro.Web.Models.Requests;
using bringpro.Web.Models.Responses;
using bringpro.Web.Services;
using bringpro.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace bringpro.Web.Controllers.Api
{
    [RoutePrefix("api/jobs")]
    public class JobsApiController : ApiController

    {
        [Dependency]
        public IJobsService _JobsService { get; set; }
        //job table data/pagination/queries
        [Route(), HttpGet]
        public HttpResponseMessage GetAllJobsWithFilter([FromUri] PaginatedRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            PaginatedItemsResponse<Job> response = _JobsService.GetAllJobsWithFilter(model);

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [Route(), HttpPost]
        public HttpResponseMessage JobInsert(JobInsertRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            ItemResponse<int> response = new ItemResponse<int>();
            UserProfile user = null;
            // Check to see if user is logged use userservice.isloggedin
            if (UserService.IsLoggedIn())
            {               
                user = _UserProfileService.GetUserById(UserService.GetCurrentUserId());

                model.UserId = user.UserId;
                model.Phone = user.Phone;

                if(user.ExternalUserId != null )
                {
                    int extId = 0;

                    int.TryParse(user.ExternalUserId, out extId);

                    if(extId > 0)
                    {
                        model.ExternalCustomerId = extId;
                    }
                }
            }

            int tempJobId = _JobsService.InsertJob(model);

            response.Item = tempJobId;

            //Activity Log Service
            if (user != null && user.UserId != null)
            {
                ActivityLogRequest Activity = new ActivityLogRequest();

                Activity.ActivityType = ActivityTypeId.CreatedJob;

                Activity.JobId = tempJobId;
                Activity.TargetValue = (int)JobStatus.BringgCreated;

                _ActivityLogService.InsertActivityToLog(model.UserId, Activity);
            }

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [Route("{id:int}"), HttpPut]
        public HttpResponseMessage JobEdit(JobUpdateRequest model, int id)
        {

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            model.Id = id;

            bool isSuccessful = _JobsService.UpdateJob(model);

            ItemResponse<bool> response = new ItemResponse<bool>();

            response.Item = isSuccessful;

            //Activity Log Service
            ActivityLogRequest Activity = new ActivityLogRequest();

            Activity.ActivityType = ActivityTypeId.JobUpdated;
            Activity.JobId = id;
            Activity.TargetValue = (int)JobStatus.BringgCreated;

            _ActivityLogService.InsertActivityToLog(model.UserId, Activity);

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }

        [Route("{id:int}"), HttpDelete]
        public HttpResponseMessage JobDelete(int Id)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            JobDeleteRequest model = new JobDeleteRequest();

            model.Id = Id;

            Job JobList = new Job();
            JobList = _JobsService.GetJobById(Id);

            bool isSuccessful = _JobsService.DeleteJob(model);

            ItemResponse<bool> response = new ItemResponse<bool>();

            response.Item = isSuccessful;

            //Activity Log Service
            ActivityLogRequest Activity = new ActivityLogRequest();

            Activity.ActivityType = ActivityTypeId.JobDeleted;
            Activity.JobId = Id;

            _ActivityLogService.InsertActivityToLog(JobList.UserId, Activity);

            return Request.CreateResponse(HttpStatusCode.OK, response);

        }
    }
}





