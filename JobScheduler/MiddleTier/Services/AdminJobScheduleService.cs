using Microsoft.Practices.Unity;
using Sabio.Data;
using Sabio.Web.Domain;
using Sabio.Web.Models.Requests;
using Sabio.Web.Models.Responses;
using Sabio.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Sabio.Web.Services
{
    public class AdminJobScheduleService : BaseService, IAdminJobScheduleService
    {
        //Triple Select for grabbing ALL Types of Time Slots (Default, Override, ASAP)
        public TimeSlotsItemsResponse<JobTimeSlots> GetAllTimeSlotsByTeam(int TeamId, JobTimeSlotsQueryRequest model)
        {
            List<JobTimeSlots> TimeSlotList = new List<JobTimeSlots>();
            List<JobTimeSlots> OverrideTimeSlotList = new List<JobTimeSlots>();
            List<JobTimeSlots> AsapTimeSlotList = new List<JobTimeSlots>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.JobTimeSlots_SelectAllByTeamId"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@TeamId", TeamId);
                  paramCollection.AddWithValue("@QueryDay", model.QueryDay);

              }, map: delegate (IDataReader reader, short set)
              {
                  if (set == 0)
                  {
                      JobTimeSlots c = new JobTimeSlots();
                      int startingIndex = 0;

                      c.Id = reader.GetSafeInt32(startingIndex++);
                      c.Date = reader.GetSafeDateTimeNullable(startingIndex++);
                      c.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                      c.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                      c.TimeStart = reader.GetSafeInt32(startingIndex++);
                      c.TimeEnd = reader.GetSafeInt32(startingIndex++);
                      c.Capacity = reader.GetSafeInt32(startingIndex++);
                      c.DefaultId = reader.GetSafeInt32Nullable(startingIndex++);
                      c.DayOfWeek = reader.GetSafeString(startingIndex++);
                      c.TeamId = reader.GetSafeInt32(startingIndex++);
                      c.ScheduleType = reader.GetSafeBool(startingIndex++);

                      TimeSlotList.Add(c);
                  }
                  else if (set == 1)
                  {

                      JobTimeSlots d = new JobTimeSlots();
                      int startingIndex = 0;

                      d.Id = reader.GetSafeInt32(startingIndex++);
                      d.Date = reader.GetSafeDateTimeNullable(startingIndex++);
                      d.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                      d.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                      d.TimeStart = reader.GetSafeInt32(startingIndex++);
                      d.TimeEnd = reader.GetSafeInt32(startingIndex++);
                      d.Capacity = reader.GetSafeInt32(startingIndex++);
                      d.DefaultId = reader.GetSafeInt32Nullable(startingIndex++);
                      d.DayOfWeek = reader.GetSafeString(startingIndex++);
                      d.TeamId = reader.GetSafeInt32(startingIndex++);
                      d.ScheduleType = reader.GetSafeBool(startingIndex++);


                      OverrideTimeSlotList.Add(d);
                  }
                  else if (set == 2)
                  {

                      JobTimeSlots e = new JobTimeSlots();
                      int startingIndex = 0;

                      e.Id = reader.GetSafeInt32(startingIndex++);
                      e.Date = reader.GetSafeDateTimeNullable(startingIndex++);
                      e.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                      e.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                      e.TimeStart = reader.GetSafeInt32(startingIndex++);
                      e.TimeEnd = reader.GetSafeInt32(startingIndex++);
                      e.Capacity = reader.GetSafeInt32(startingIndex++);
                      e.DefaultId = reader.GetSafeInt32Nullable(startingIndex++);
                      e.DayOfWeek = reader.GetSafeString(startingIndex++);
                      e.TeamId = reader.GetSafeInt32(startingIndex++);
                      e.ScheduleType = reader.GetSafeBool(startingIndex++);


                      AsapTimeSlotList.Add(e);
                  }

              });

            TimeSlotsItemsResponse<JobTimeSlots> response = new TimeSlotsItemsResponse<JobTimeSlots>();
            response.Items = TimeSlotList;
            response.OverrideItems = OverrideTimeSlotList;
            response.AsapItems = AsapTimeSlotList;

            return response;
        }

        public JobTimeSlots GetTimeSlotById(int Id)
        {
            JobTimeSlots c = new JobTimeSlots();

            DataProvider.ExecuteCmd(GetConnection, "dbo.JobTimeSlots_SelectById"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@Id", Id);

              }, map: delegate (IDataReader reader, short set)
              {
                  int startingIndex = 0;

                  c.Id = reader.GetSafeInt32(startingIndex++);
                  c.Date = reader.GetSafeDateTimeNullable(startingIndex++);
                  c.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                  c.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                  c.TimeStart = reader.GetSafeInt32(startingIndex++);
                  c.TimeEnd = reader.GetSafeInt32(startingIndex++);
                  c.Capacity = reader.GetSafeInt32(startingIndex++);
                  c.DefaultId = reader.GetSafeInt32Nullable(startingIndex++);
                  c.DayOfWeek = reader.GetSafeString(startingIndex++);
                  c.TeamId = reader.GetSafeInt32(startingIndex++);
                  c.ScheduleType = reader.GetSafeBool(startingIndex++);

              }

              );

            return c;
        }

        public bool UpdateTimeSlot(JobTimeSlots model)
        {
            bool success = false;
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.JobTimeSlots_Update"
                      , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                      {
                          paramCollection.AddWithValue("@Id", model.Id);
                          paramCollection.AddWithValue("@Date", model.Date);
                          paramCollection.AddWithValue("@TimeStart", model.TimeStart);
                          paramCollection.AddWithValue("@TimeEnd", model.TimeEnd);
                          paramCollection.AddWithValue("@Capacity", model.Capacity);
                          paramCollection.AddWithValue("@DayOfWeek", model.DayOfWeek);
                          paramCollection.AddWithValue("@TeamId", model.TeamId);
                          paramCollection.AddWithValue("@ScheduleType", model.ScheduleType);

                      }, returnParameters: delegate (SqlParameterCollection param)
                      {
                          success = true;
                      });


            return success;
        }

        //Adding Overrides And Default Time Slots.  Business Logic for transferring all scheduled jobs existing default Id's to Override Id's
        public int InsertNewTimeSlot(JobTimeSlots model)
        {
            int id = 0;
            int NewOverrideId = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.JobTimeSlots_Insert"
                      , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                      {
                          paramCollection.AddWithValue("@Date", model.Date);
                          paramCollection.AddWithValue("@TimeStart", model.TimeStart);
                          paramCollection.AddWithValue("@TimeEnd", model.TimeEnd);
                          paramCollection.AddWithValue("@Capacity", model.Capacity);
                          paramCollection.AddWithValue("@DefaultId", model.DefaultId);
                          paramCollection.AddWithValue("@DayOfWeek", model.DayOfWeek);
                          paramCollection.AddWithValue("@TeamId", model.TeamId);
                          paramCollection.AddWithValue("@ScheduleType", model.ScheduleType);

                          SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                          p.Direction = System.Data.ParameterDirection.Output;

                          paramCollection.Add(p);

                      }, returnParameters: delegate (SqlParameterCollection param)
                      {
                          int.TryParse(param["@Id"].Value.ToString(), out id);
                      });

            TimeSlotAvailabilityRequest Availabilities = new TimeSlotAvailabilityRequest();
            NewOverrideId = id;
            //Add If Statement here to grab ONLY Overrides, then do logic for Assuming then a override is added and needs to check if default jobs exist on the same time/date
            if (model.DefaultId != null && model.Date != null && model.ScheduleType == true)
            {
                //Check Availabilities with the Override Info Coming in.  Get Default ID and Check Availabilities that already exist
                Availabilities = CheckExistingDefaultsForOverride(model.Date, model.DefaultId);
                if (Availabilities.CurrentUsed > model.Capacity)
                {
                    //delete the insert in case the capacity is too high
                    DeleteTimeSlot(NewOverrideId);
                    int OverCapacityAmount = (Availabilities.CurrentUsed - model.Capacity);
                    string message = "Unable to create Override due to Max Capacity.  Over Amount: " + OverCapacityAmount;
                    throw new System.ArgumentException(message);
                }

                if (Availabilities.CurrentUsed <= model.Capacity)
                {
                    //service to update all ScheduleId to NEW Override Schedule Id
                    UpdateJobScheduleId(NewOverrideId, model.DefaultId, model.Date);
                }



            }
            return id;
        }

        //Grab Time Slots By Date for default or override.  Then checks to see if a ASAP slot exists for the day and adds one.  Hangfire backup still required.
        public List<JobTimeSlots> GetAllTimeSlotByDate(JobTimeSlotsQueryRequest model)
        {
            List<JobTimeSlots> TimeSlotList = new List<JobTimeSlots>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.JobTimeSlots_SelectByDate"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@TeamId", model.TeamId);
                  paramCollection.AddWithValue("@ScheduleType", model.ScheduleType);
                  paramCollection.AddWithValue("@QueryDate", model.QueryDate);
                  paramCollection.AddWithValue("@QueryDay", model.QueryDay);

              }, map: delegate (IDataReader reader, short set)
              {
                  JobTimeSlots c = new JobTimeSlots();
                  int startingIndex = 0;

                  c.Id = reader.GetSafeInt32(startingIndex++);
                  c.Date = reader.GetSafeDateTimeNullable(startingIndex++);
                  c.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                  c.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                  c.TimeStart = reader.GetSafeInt32(startingIndex++);
                  c.TimeEnd = reader.GetSafeInt32(startingIndex++);
                  c.Capacity = reader.GetSafeInt32(startingIndex++);
                  c.DefaultId = reader.GetSafeInt32Nullable(startingIndex++);
                  c.DayOfWeek = reader.GetSafeString(startingIndex++);
                  c.TeamId = reader.GetSafeInt32(startingIndex++);
                  c.ScheduleType = reader.GetSafeBool(startingIndex++);

                  TimeSlotList.Add(c);

              });

            //Checking to see if a ASAP Time Slot exists for ASAP, and then adds one based on if it does or not
            if (TimeSlotList.Count == 0 && model.ScheduleType == false)
            {
                int AsapTimeSlotId = 0;

                JobTimeSlots AsapTimeSlot = new JobTimeSlots();
                AsapTimeSlot.Date = getTodayDate();
                AsapTimeSlot.TimeStart = 0;
                AsapTimeSlot.TimeEnd = 2400;
                AsapTimeSlot.Capacity = 1;
                AsapTimeSlot.DefaultId = null;
                AsapTimeSlot.DayOfWeek = (getTodayDate()).ToString("dddd");
                AsapTimeSlot.TeamId = model.TeamId;
                AsapTimeSlot.ScheduleType = false;

                AsapTimeSlotId = InsertNewTimeSlot(AsapTimeSlot);

                if (AsapTimeSlotId > 0)
                {
                    List<JobTimeSlots> AsapTimeSlotList = new List<JobTimeSlots>();

                    AsapTimeSlotList = GetAllTimeSlotByDate(model);

                    return AsapTimeSlotList;
                }
            }


            return TimeSlotList;
        }

        public bool DeleteTimeSlot(int Id)
        {
            //Get Delete Time Slot Info
            JobTimeSlots TimeSlotInfo = GetTimeSlotById(Id);
            //Check if Override or Default
            if (TimeSlotInfo.Date != null)
            {
                //service to update all ScheduleId to NEW Default Schedule Id
                UpdateJobScheduleId(TimeSlotInfo.DefaultId, Id, TimeSlotInfo.Date);
            }

            bool success = false;
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.JobTimeSlots_Delete"
                      , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                      {
                          paramCollection.AddWithValue("@Id", Id);

                      }, returnParameters: delegate (SqlParameterCollection param)
                      {
                          success = true;
                      });

            return success;
        }


        //Insert for Job Schedule
        public int InsertNewJobSchedule(JobSchedule model)
        {
            int id = 0;

            DataProvider.ExecuteNonQuery(GetConnection, "dbo.JobSchedule_Insert"
                      , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                      {
                          paramCollection.AddWithValue("@JobId", model.JobId);
                          paramCollection.AddWithValue("@ScheduleId", model.ScheduleId);
                          paramCollection.AddWithValue("@Date", model.Date);

                          SqlParameter p = new SqlParameter("@Id", System.Data.SqlDbType.Int);
                          p.Direction = System.Data.ParameterDirection.Output;

                          paramCollection.Add(p);

                      }, returnParameters: delegate (SqlParameterCollection param)
                      {
                          int.TryParse(param["@Id"].Value.ToString(), out id);
                      });


            return id;
        }

        //This API grabs all available slots, then filters out the defaults that have overrides.  Then grabs capacity.
        public List<JobTimeSlots> GetAllAvailableByDate(JobTimeSlotsQueryRequest model)
        {
            List<JobTimeSlots> TimeSlotList = new List<JobTimeSlots>();

            DataProvider.ExecuteCmd(GetConnection, "dbo.JobTimeSlots_SelectAvailableByDate"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@TeamId", model.TeamId);
                  paramCollection.AddWithValue("@QueryDate", model.QueryDate);
                  paramCollection.AddWithValue("@QueryDay", model.QueryDay);

              }, map: delegate (IDataReader reader, short set)
              {

                  JobTimeSlots c = new JobTimeSlots();
                  int startingIndex = 0;

                  c.Id = reader.GetSafeInt32(startingIndex++);
                  c.Date = reader.GetSafeDateTimeNullable(startingIndex++);
                  c.CreatedDate = reader.GetSafeDateTime(startingIndex++);
                  c.ModifiedDate = reader.GetSafeDateTime(startingIndex++);
                  c.TimeStart = reader.GetSafeInt32(startingIndex++);
                  c.TimeEnd = reader.GetSafeInt32(startingIndex++);
                  c.Capacity = reader.GetSafeInt32(startingIndex++);
                  c.DefaultId = reader.GetSafeInt32Nullable(startingIndex++);
                  c.DayOfWeek = reader.GetSafeString(startingIndex++);
                  c.TeamId = reader.GetSafeInt32(startingIndex++);
                  c.ScheduleType = reader.GetSafeBool(startingIndex++);

                  TimeSlotList.Add(c);

              });

            //Replace the Defaults with Overrides that exist
            for (int i = 0; i < TimeSlotList.Count; i++)
            {
                if (TimeSlotList[i].DefaultId != null)
                {
                    for (int x = 0; x < TimeSlotList.Count; x++)
                    {
                        if (TimeSlotList[x].Id == TimeSlotList[i].DefaultId)
                        {
                            TimeSlotList.RemoveAt(x);
                        }
                    }
                }
            }

            //Grab Availabilities By Date
            List<JobCountUsed> Availabilities = new List<JobCountUsed>();
            Availabilities = GetCapacityByQuery(model.QueryDate);

            //Grab Capacity Used From Data Received From Service
            for (int i = 0; i < TimeSlotList.Count; i++)
            {
                int currentUsed = 0;

                for (int y = 0; y < Availabilities.Count; y++)
                {
                    if (TimeSlotList[i].Id == Availabilities[y].ScheduleId)
                    {
                        currentUsed = Availabilities[y].CurrentUsed;
                    }
                }

                TimeSlotList[i].UsedCapacity = currentUsed;

            }

            return TimeSlotList;
        }


        public List<JobCountUsed> GetCapacityByQuery(DateTime? QueryDate)
        {
            List<JobCountUsed> c = new List<JobCountUsed>();

            //will need to switch to v2 soon
            DataProvider.ExecuteCmd(GetConnection, "dbo.JobTimeSlots_SelectAllCapacityByDate"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@QueryDate", QueryDate);

              }, map: delegate (IDataReader reader, short set)
              {

                  JobCountUsed d = new JobCountUsed();
                  int startingIndex = 0; //startingOrdinal

                  d.CurrentUsed = reader.GetSafeInt32(startingIndex++);
                  d.ScheduleId = reader.GetSafeInt32(startingIndex++);

                  c.Add(d);

              });

            return c;
        }

        //Check Availabilities when changing default jobs to override jobs & vice versa
        public TimeSlotAvailabilityRequest CheckExistingDefaultsForOverride(DateTime? QueryDate, int? QueryScheduleId)
        {

            TimeSlotAvailabilityRequest c = new TimeSlotAvailabilityRequest();

            //will need to switch to v2 soon
            DataProvider.ExecuteCmd(GetConnection, "dbo.JobTimeSlots_SelectCapacityByDate"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@QueryDate", QueryDate);
                  paramCollection.AddWithValue("@QueryScheduleId", QueryScheduleId);

              }, map: delegate (IDataReader reader, short set)
              {
                  if (set == 0)
                  {
                      c.CurrentAvailable = reader.GetSafeInt32(0);

                  }
                  else if (set == 1)
                  {
                      c.CurrentUsed = reader.GetSafeInt32(0);
                  }
              });



            return c;
        }

        public bool UpdateJobScheduleId(int? NewId, int? OldId, DateTime? Date)
        {
            bool success = false;
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.JobSchedule_UpdateScheduleId"
                      , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                      {
                          paramCollection.AddWithValue("@QueryOverrideId", NewId);
                          paramCollection.AddWithValue("@QueryDefaultId", OldId);
                          paramCollection.AddWithValue("@QueryDate", Date);


                      }, returnParameters: delegate (SqlParameterCollection param)
                      {
                          success = true;
                      });


            return success;
        }

        public void UpdateJobScheduleById(JobSchedule model)
        {
            DataProvider.ExecuteNonQuery(GetConnection, "dbo.JobSchedule_UpdateById"
                      , inputParamMapper: delegate (SqlParameterCollection paramCollection)
                      {
                          paramCollection.AddWithValue("@Id", model.Id);
                          paramCollection.AddWithValue("@JobId", model.JobId);
                          paramCollection.AddWithValue("@ScheduleId", model.ScheduleId);
                          paramCollection.AddWithValue("@Date", model.Date);


                      }, returnParameters: delegate (SqlParameterCollection param)
                      {

                      });

        }

        //Add this to JobApiController Logic for JobSchedule
        public JobSchedule GetJobScheduleByJobId(int? JobId)
        {
            JobSchedule c = new JobSchedule();

            DataProvider.ExecuteCmd(GetConnection, "dbo.JobSchedule_SelectByJobId"
              , inputParamMapper: delegate (SqlParameterCollection paramCollection)
              {
                  paramCollection.AddWithValue("@JobId", JobId);

              }, map: delegate (IDataReader reader, short set)
              {

                  int startingIndex = 0; //startingOrdinal

                  c.Id = reader.GetSafeInt32(startingIndex++);
                  c.JobId = reader.GetSafeInt32(startingIndex++);
                  c.ScheduleId = reader.GetSafeInt32(startingIndex++);
                  c.Date = reader.GetSafeDateTime(startingIndex++);

              });

            return c;
        }

        public DateTime getTodayDate()
        {
            DateTime today = DateTime.Today;
            return today;
        }

        //This Upsert figures out whether the Job already exists or not, and updates or inserts depending on the result.  Used on Job Flow.
        public int JobScheduleUpsert(JobSchedule model)
        {
            //Do If Check for if there is JobScheduleId Already exists or not.  If exists, do update, if not, then insert.
            int JobScheduleId = 0;

            JobSchedule scheduleObject = GetJobScheduleByJobId(model.JobId);

            if (scheduleObject.Date != null && scheduleObject.JobId != null)
            {
                model.Id = scheduleObject.Id;
                UpdateJobScheduleById(model);
                JobScheduleId = scheduleObject.Id;
            }
            else
            {

                JobScheduleId = InsertNewJobSchedule(model);

            }

            return JobScheduleId;
        }


    }
}