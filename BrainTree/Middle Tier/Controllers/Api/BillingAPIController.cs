using Braintree;
using Microsoft.Practices.Unity;
using bringpro.Web.Domain;
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
    [Authorize]
    [RoutePrefix("api/billing")]
    public class BillingAPIController : ApiController
    {
        [Dependency]
        public ICreditCardService _CreditCardService { get; set; }

        [Dependency]
        public IBrainTreeService _BrainTreeService { get; set; }

        [Route("adminPayment")]
        [HttpPost]
        public HttpResponseMessage APIAdminPayment(PaymentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            bool isSuccessful = false;

            try
            {
                isSuccessful = _BrainTreeService.AdminPaymentService(model);
            }
            catch (ArgumentException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            ItemResponse<bool> response = new ItemResponse<bool>();

            response.Item = isSuccessful;

            return Request.CreateResponse(response);

        }


        //Route to make purchase and log as GUEST*** for later
        [Route()]
        [HttpPost]
        public HttpResponseMessage CreatePurchase(CustomerPaymentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            string gotUserId = UserService.GetCurrentUserId();

            model.UserId = gotUserId;

            bool isSuccessful = false;

            try
            {
                isSuccessful = _BrainTreeService.PaymentAndLogService(model);
            }
            catch (ArgumentException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            ItemResponse<bool> response = new ItemResponse<bool>();

            response.Item = isSuccessful;

            return Request.CreateResponse(response);
           
        }

        //Testing API to Add a NEW CUSTOMER and make a purchase. 
        [Route("payment"), HttpPost]
        public HttpResponseMessage AddPurchaseAndCustomer(CustomerPaymentRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            bool isSuccessful = false;

            try
            {
                isSuccessful = _BrainTreeService.CreateCustomerTransaction(model);
            }
            catch (ArgumentException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            ItemResponse<bool> response = new ItemResponse<bool>();

            response.Item = isSuccessful;

            return Request.CreateResponse(response);

        }

        //BrainTree Customer Get Request
        [Route("payment"), HttpGet]
        public HttpResponseMessage GetById()
        {   
            string gotUserId = UserService.GetCurrentUserId();
            ItemResponse<Customer> response = new ItemResponse<Customer>();
            try
            {
                response.Item = _BrainTreeService.customerGetByUserId(gotUserId);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }

            return Request.CreateResponse(response);
        }

        //Get for ADMIN - Get ALL users with a credit card
        [Route(), HttpGet]
        public HttpResponseMessage List()
        {

            ItemsResponse<AdminCreditCards> response = new ItemsResponse<AdminCreditCards>();
            response.Items = _CreditCardService.GetAllUserIdWithCC();
            return Request.CreateResponse(response);
        }

        //Get for Admin - Get All Credit Cards from Selected User
        [Route("{UserId}"), HttpGet]
        public HttpResponseMessage GetById(string UserId)
        {

            ItemsResponse<UserCreditCards> response = new ItemsResponse<UserCreditCards>();
            response.Items = _CreditCardService.GetAllCreditCardsByUserId(UserId);
            return Request.CreateResponse(response);

        }

    }
}
