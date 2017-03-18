using Microsoft.Practices.Unity;
using bringpro.Data;
using bringpro.Web.Classes.Tasks.Bringg.Interfaces;
using bringpro.Web.Domain;
using bringpro.Web.Enums;
using bringpro.Web.Models.Requests;
using bringpro.Web.Models.Responses;
using bringpro.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace bringpro.Web.Services
{

    public class JobsService : BaseService, IJobsService

    {
        //Single Get for getting all data with queries/pagination
        public PaginatedItemsResponse<Job> GetAllJobsWithFilter(PaginatedRequest model)
        {
            List<Job> JobList = null;
            PaginatedItemsResponse<Job> response = null;


            DataProvider.ExecuteCmd(GetConnection, "dbo.Jobs_SelectAll"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@CurrentPage", model.CurrentPage);
                  paramCollection.AddWithValue("@ItemsPerPage", model.ItemsPerPage);
                  paramCollection.AddWithValue("@Query", model.Query);
                  paramCollection.AddWithValue("@QueryWebsiteId", model.QueryWebsiteId);
                  paramCollection.AddWithValue("@QueryStatus", model.QueryStatus);
                  paramCollection.AddWithValue("@QueryJobType", model.QueryJobType);
                  paramCollection.AddWithValue("@QueryStartDate", model.QueryStartDate);
                  paramCollection.AddWithValue("@QueryEndDate", model.QueryEndDate);

              }, map: delegate (IDataReader reader, short set)

              {

                  if (set == 0)
                  {
                      Job SingleJob = new Job();
                      int startingIndex = 0; //startingOrdinal

                      SingleJob.Id = reader.GetSafeInt32(startingIndex++);
                      SingleJob.UserId = reader.GetSafeString(startingIndex++);
                      SingleJob.JobStatus = reader.GetSafeEnum<JobStatus>(startingIndex++);
                      SingleJob.JobType = reader.GetSafeEnum<JobsType>(startingIndex++);
                      SingleJob.Price = reader.GetSafeDecimal(startingIndex++);
                      SingleJob.Phone = reader.GetSafeString(startingIndex++);
                      SingleJob.JobRequest = reader.GetSafeInt32(startingIndex++);
                      SingleJob.SpecialInstructions = reader.GetSafeString(startingIndex++);
                      SingleJob.Created = reader.GetSafeDateTime(startingIndex++);
                      SingleJob.Modified = reader.GetSafeDateTime(startingIndex++);
                      SingleJob.WebsiteId = reader.GetSafeInt32(startingIndex++);

                      if (JobList == null)
                      {
                          JobList = new List<Job>();
                      }
                      JobList.Add(SingleJob);

                  }
                  else if (set == 1)

                  {
                      response = new PaginatedItemsResponse<Job>();
                      response.TotalItems = reader.GetSafeInt32(0);

                  }
              }

           );

            response.Items = JobList;
            response.CurrentPage = model.CurrentPage;
            response.ItemsPerPage = model.ItemsPerPage;

            return response;
        }

        public int InsertJob(JobInsertRequest model)
        {
            int id = 0;
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Jobs_Insert"
                      , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                      {
                          paramCollection.AddWithValue("@UserId", model.UserId);
                          paramCollection.AddWithValue("@Status", (int) JobStatus.Draft);
                          paramCollection.AddWithValue("@JobType", model.JobType);
                          paramCollection.AddWithValue("@Price", model.Price);
                          paramCollection.AddWithValue("@Phone", model.Phone);
                          paramCollection.AddWithValue("@JobRequest", model.JobRequest);
                          paramCollection.AddWithValue("@SpecialInstructions", model.SpecialInstructions);
                          paramCollection.AddWithValue("@WebsiteId", model.WebsiteId);
                          paramCollection.AddWithValue("@ExternalCustomerId", model.ExternalCustomerId);

                          SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                          p.Direction = System.Data.ParameterDirection.Output;

                          paramCollection.Add(p);

                      }, returnParameters: delegate (SqlParameterCollection param)
                      {
                          int.TryParse(param["@Id"].Value.ToString(), out id);
                      });


            return id;
        }

        public bool UpdateJob(JobUpdateRequest model)
        {
            bool success = false;
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Jobs_Update"
                      , inputParamMapper: delegate (SqlParameterCollection paramCollection)

                      {
                          paramCollection.AddWithValue("@Id", model.Id);
                          paramCollection.AddWithValue("@UserId", model.UserId);
                          paramCollection.AddWithValue("@Status", model.Status);
                          paramCollection.AddWithValue("@JobType", model.JobType);
                          paramCollection.AddWithValue("@Price", model.Price);
                          paramCollection.AddWithValue("@JobRequest", model.JobRequest);
                          paramCollection.AddWithValue("@ContactName", model.ContactName);
                          paramCollection.AddWithValue("@Phone", model.Phone);
                          paramCollection.AddWithValue("@SpecialInstructions", model.SpecialInstructions);
                          paramCollection.AddWithValue("@WebsiteId", model.WebsiteId);
                          paramCollection.AddWithValue("@ExternalJobId", model.ExternalJobId);
                          paramCollection.AddWithValue("@ExternalCustomerId", model.ExternalCustomerId);
                          paramCollection.AddWithValue("@PaymentNonce", model.PaymentNonce);
                          paramCollection.AddWithValue("@BillingId", model.BillingId);

                      }, returnParameters: delegate (SqlParameterCollection param)
                      {
                          success = true;
                      });


            return success;
        }


        public bool DeleteJob(JobDeleteRequest model)
        {
            bool success = false;
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.Jobs_DeleteById"
                      , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                      {
                          paramCollection.AddWithValue("@Id", model.Id);

                      }, returnParameters: delegate (SqlParameterCollection param)
                      {
                          success = true;
                      });

            return success;
        }
    }
}