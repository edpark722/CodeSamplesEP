using Braintree;
using Microsoft.Practices.Unity;
using bringpro.Web.Domain;
using bringpro.Web.Enums;
using bringpro.Web.Models.Requests;
using bringpro.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;


namespace bringpro.Web.Services
{
    public class BrainTreeService : BaseService, IBrainTreeService
    {

        [Dependency]
        public ITransactionLogService _TransactionLogService { get; set; }
        [Dependency]
        public IUserProfileService _UserProfileService { get; set; }
        [Dependency]
        public IActivityLogService _ActivityLogService { get; set; }
        [Dependency]
        public IUserCreditsService _CreditsService { get; set; }


        private static BraintreeGateway _Gateway = new BraintreeGateway
        {
            Environment = Braintree.Environment.SANDBOX,
            MerchantId = ConfigService.BraintreeMerchantId,
            PublicKey = ConfigService.BraintreePublicKey,
            PrivateKey = ConfigService.BraintreePrivateKey
        };


        public string CreateToken()
        {
            return _Gateway.ClientToken.generate();
        }

        //BrainTree Customer FIND
        public Customer customerGetByUserId(string UserId)
        {
            Customer customer = _Gateway.Customer.Find(UserId);

            return customer;
        }

        //BrainTree Customer Creation for Register User
        public bool newCustomerInsert(CustomerPaymentRequest model)
        {
            CustomerRequest request = new CustomerRequest
            {
                Id = model.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone
            };

            Result<Customer> result = _Gateway.Customer.Create(request);

            bool isSuccessful = result.IsSuccess();

            return isSuccessful;
        }


        public string GuestPayment(CustomerPaymentRequest model)
        {
            bool DuplicateBool = false;

            bool.TryParse(ConfigService.DuplicatePaymentMethod, out DuplicateBool);

            bool VerifyBool = false;
            bool.TryParse(ConfigService.VerifyCard, out VerifyBool);
            //For guest checkout - Braintree does not create a new customer
            Result<Customer> customerResult = _Gateway.Customer.Create(new CustomerRequest());

            Customer customer = customerResult.Target;
            string customerId = customer.Id;

            PaymentMethodRequest request = new PaymentMethodRequest
            {
                CustomerId = customerId,
                PaymentMethodNonce = model.ExternalCardIdNonce,
                Options = new PaymentMethodOptionsRequest
                {
                    FailOnDuplicatePaymentMethod = DuplicateBool,
                    VerifyCard = VerifyBool
                },
            };


            Result<PaymentMethod> result = _Gateway.PaymentMethod.Create(request);

            bool isSuccessful = result.IsSuccess();

            if (isSuccessful == false)
            {
                throw new System.ArgumentException(result.Message);
            }

            return result.Target.Token;
        }


        //Update Customer on BrainTree with Payment Method Request
        public string UpdateCustomerWithNewCardDuplicateCheck(UserCreditCards model)
        {
            bool DuplicateBool = false;

            bool.TryParse(ConfigService.DuplicatePaymentMethod, out DuplicateBool);

            bool VerifyBool = false;

            bool.TryParse(ConfigService.VerifyCard, out VerifyBool);


            PaymentMethodRequest request = new PaymentMethodRequest
            {
                CustomerId = model.UserId,
                PaymentMethodNonce = model.ExternalCardIdNonce,
                Options = new PaymentMethodOptionsRequest
                {
                    FailOnDuplicatePaymentMethod = DuplicateBool,
                    VerifyCard = VerifyBool
                },
            };




            Result<PaymentMethod> result = _Gateway.PaymentMethod.Create(request);

            bool isSuccessful = result.IsSuccess();

            if (isSuccessful == false)
            {
                throw new System.ArgumentException(result.Message);
            }

            return result.Target.Token;

        }

        //Service for making Payment with ADMIN
        public bool AdminPaymentService(PaymentRequest model)
        {
            string nonceFromTheClient = model.ExternalCardIdNonce;

            int currentTransactionId = 0;

            //Temporary code below
            ITransactionLogService _transactionLogService = UnityConfig.GetContainer().Resolve<ITransactionLogService>();

            currentTransactionId = _transactionLogService.BillingTransactionInsert(model);

            TransactionRequest request = new TransactionRequest
            {
                Amount = model.ItemCost,
                PaymentMethodToken = nonceFromTheClient,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = _Gateway.Transaction.Sale(request);

            var transactionJson = new JavaScriptSerializer().Serialize(result);

            //Instantiate values into Transaction Log Model
            BillingTransactionLog LogModel = new BillingTransactionLog();
            LogModel.Id = currentTransactionId;
            LogModel.RawResponse = transactionJson;


            if (result.Message == null && result.Errors == null)
            {
                //Instatiate Success Values
                LogModel.AmountConfirmed = result.Target.Amount;
                LogModel.TransactionId = result.Target.Id;
                LogModel.CardExpirationDate = result.Target.CreditCard.ExpirationDate;
                LogModel.CardLastFour = result.Target.CreditCard.LastFour;

                ActivityLogRequest Activity = new ActivityLogRequest();

                Activity.ActivityType = ActivityTypeId.MadePayment;


                _ActivityLogService.InsertActivityToLog(model.UserId, Activity);



                return _transactionLogService.TransactionLogUpdateSuccess(LogModel);

            }
            else
            {
                //Instatiate Error Values
                LogModel.ErrorCode = result.Message;

                bool response = _transactionLogService.TransactionLogUpdateError(LogModel);
                throw new System.ArgumentException(result.Message, "CreditCard");
            }
        }


        public bool AdminPaymentService(PaymentRequest model, int jobId)
        {
            string nonceFromTheClient = model.ExternalCardIdNonce;

            int currentTransactionId = 0;

            currentTransactionId = _TransactionLogService.BillingTransactionInsert(model);

            TransactionRequest request = new TransactionRequest
            {
                Amount = model.ItemCost,
                PaymentMethodToken = nonceFromTheClient,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = _Gateway.Transaction.Sale(request);

            var transactionJson = new JavaScriptSerializer().Serialize(result);

            //Instantiate values into Transaction Log Model
            BillingTransactionLog LogModel = new BillingTransactionLog();
            LogModel.Id = currentTransactionId;
            LogModel.RawResponse = transactionJson;


            if (result.Message == null && result.Errors == null)
            {
                //Instatiate Success Values
                LogModel.AmountConfirmed = result.Target.Amount;
                LogModel.TransactionId = result.Target.Id;
                LogModel.CardExpirationDate = result.Target.CreditCard.ExpirationDate;
                LogModel.CardLastFour = result.Target.CreditCard.LastFour;

                ActivityLogRequest Activity = new ActivityLogRequest();

                Activity.ActivityType = ActivityTypeId.MadePayment;


                _ActivityLogService.InsertActivityToLog(model.UserId, Activity);
                JobsService.UpdateBillingId(jobId, currentTransactionId);


                return _TransactionLogService.TransactionLogUpdateSuccess(LogModel);

            }
            else
            {
                //Instatiate Error Values
                LogModel.ErrorCode = result.Message;

                bool response = _TransactionLogService.TransactionLogUpdateError(LogModel);
                throw new System.ArgumentException(result.Message, "CreditCard");
            }
        }


        //Test Code for possibly making payment as guest
        public bool PaymentAndLogService(PaymentRequest model)
        {
            string nonceFromTheClient = model.ExternalCardIdNonce;

            int currentTransactionId = 0;

            currentTransactionId = _TransactionLogService.BillingTransactionInsert(model);

            TransactionRequest request = new TransactionRequest
            {
                Amount = model.ItemCost,
                PaymentMethodNonce = nonceFromTheClient,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = _Gateway.Transaction.Sale(request);

            var transactionJson = new JavaScriptSerializer().Serialize(result);

            //Instantiate values into Transaction Log Model
            BillingTransactionLog LogModel = new BillingTransactionLog();
            LogModel.Id = currentTransactionId;
            LogModel.RawResponse = transactionJson;

            if (result.Message == null && result.Errors == null)
            {
                //Instatiate Success Values
                LogModel.AmountConfirmed = result.Target.Amount;
                LogModel.TransactionId = result.Target.Id;
                LogModel.CardExpirationDate = result.Target.CreditCard.ExpirationDate;
                LogModel.CardLastFour = result.Target.CreditCard.LastFour;

                ActivityLogRequest Activity = new ActivityLogRequest();

                Activity.ActivityType = ActivityTypeId.MadePayment;

                _ActivityLogService.InsertActivityToLog(model.UserId, Activity);

                return _TransactionLogService.TransactionLogUpdateSuccess(LogModel);
            }
            else
            {
                //Instatiate Error Values
                LogModel.ErrorCode = result.Message;

                bool response = _TransactionLogService.TransactionLogUpdateError(LogModel);
                throw new System.ArgumentException(result.Message, "CreditCard");
            }
        }


        //BrainTree Customer Creation for maybe if GUEST wants to become member before Checkout.
        public bool CreateCustomerTransaction(CustomerPaymentRequest model)
        {
            //get user id with currentuserId
            string gotUserId = UserService.GetCurrentUserId();

            model.UserId = gotUserId;

            //Grab email from UserProfileService
            UserProfile userObject = _UserProfileService.GetUserById(model.UserId);

            string userEmail = userObject.Email;

            //Add the new card nonce & info into credit cards table
            UserCreditCards CardModel = new UserCreditCards();
            CardModel.UserId = model.UserId;
            CardModel.ExternalCardIdNonce = model.ExternalCardIdNonce;
            CardModel.Last4DigitsCC = model.Last4DigitsCC;
            CardModel.CardType = model.CardType;
            CreditCardService NewCardService = new CreditCardService();
            NewCardService.creditCardInsertModel(CardModel);

            int currentTransactionId = 0;

            //Post first Transaction log before result
            currentTransactionId = _TransactionLogService.BillingTransactionInsert(model);

            bool DuplicateBool = false;

            bool.TryParse(ConfigService.DuplicatePaymentMethod, out DuplicateBool);

            bool VerifyBool = false;

            bool.TryParse(ConfigService.VerifyCard, out VerifyBool);

            //create transaction and create customer in braintree system
            //***NEED TO ADD THE JOB_ID AS WELL LATER***
            TransactionRequest request = new TransactionRequest
            {
                Amount = model.ItemCost,
                PaymentMethodNonce = model.ExternalCardIdNonce,
                Customer = new CustomerRequest
                {
                    Id = model.UserId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = userEmail,

                    CreditCard = new CreditCardRequest
                    {
                        Options = new CreditCardOptionsRequest
                        {
                            FailOnDuplicatePaymentMethod = DuplicateBool,
                            VerifyCard = VerifyBool
                        }
                    }
                },
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true,
                    StoreInVaultOnSuccess = true,
                }
            };
            Result<Transaction> result = _Gateway.Transaction.Sale(request);

            //serialize the whole result object for backup purposes
            var transactionJson = new JavaScriptSerializer().Serialize(result);

            //Instantiate values into Transaction Log Model
            BillingTransactionLog LogModel = new BillingTransactionLog();
            LogModel.Id = currentTransactionId;
            LogModel.RawResponse = transactionJson;

            //if else for whether the payment was successful or not
            if (result.Message == null && result.Errors == null)
            {
                //Instatiate Success Values
                LogModel.AmountConfirmed = result.Target.Amount;
                LogModel.TransactionId = result.Target.Id;
                LogModel.CardExpirationDate = result.Target.CreditCard.ExpirationDate;
                LogModel.CardLastFour = result.Target.CreditCard.LastFour;

                ActivityLogRequest Activity = new ActivityLogRequest();

                Activity.ActivityType = ActivityTypeId.MadePayment;

                _ActivityLogService.InsertActivityToLog(model.UserId, Activity);



                return _TransactionLogService.TransactionLogUpdateSuccess(LogModel);

            }
            else
            {
                //Instatiate Error Values
                LogModel.ErrorCode = result.Message;

                bool response = _TransactionLogService.TransactionLogUpdateError(LogModel);
                throw new System.ArgumentException(result.Message, "CreditCard");
            }
        }

        private DateTime _timeCompleted;
        private DateTime _timeCreated;
        private double _webPrice;
        private double _totalPrice;
        private double _minJobDuration;
        private double _basePrice;

        //Complete 
        public bool CompleteTransaction(Job job, ActivityLogAddRequest add)
        {
            bool success = false;

            List<ActivityLog> list = _ActivityLogService.GetByJobId(add.JobId);
            foreach (var activity in list)
            {

                int currentStatus = activity.TargetValue;

                if (currentStatus == (int)JobStatus.BringgOnTheWay)
                {
                    _timeCreated = activity.IdCreated;
                }
                if (currentStatus == (int)JobStatus.BringgDone)
                {
                    _timeCompleted = activity.IdCreated;
                }
            }
            TimeSpan timeDifference = _timeCompleted.Subtract(_timeCreated);
            double toMinutes = timeDifference.TotalMinutes;

            CreditCardService cardService = new CreditCardService();
            PaymentRequest payment = new PaymentRequest();
            BrainTreeService brainTreeService = new BrainTreeService();

            List<string> Slugs = new List<string>();

            Slugs.Add("base-price");
            Slugs.Add("price-per-minute");
            Slugs.Add("minimum-job-duration");
            Slugs.Add("website-pricing-model");

            Dictionary<string, WebsiteSettings> dict = WebsiteSettingsServices.getWebsiteSettingsDictionaryBySlug(job.WebsiteId, Slugs);

            WebsiteSettings pricingModel = (dict["website-pricing-model"]);
            WebsiteSettings basePrice = (dict["base-price"]);
            WebsiteSettings pricePerMin = (dict["price-per-minute"]);
            WebsiteSettings jobDuration = (dict["minimum-job-duration"]);

            // - Switch statement to calculate service cost depending on the website's pricing model

            int pricingModelValue = Convert.ToInt32(pricingModel.SettingsValue);
            switch (pricingModelValue)
            {
                case 1:
                    _basePrice = Convert.ToDouble(basePrice.SettingsValue);
                    _totalPrice = _basePrice;
                    break;

                case 2:
                    _webPrice = Convert.ToDouble(pricePerMin.SettingsValue);
                    _minJobDuration = Convert.ToDouble(jobDuration.SettingsValue);

                    if (toMinutes <= _minJobDuration)
                    {
                        _totalPrice = _webPrice * _minJobDuration;
                    }
                    else
                    {
                        _totalPrice = _webPrice * toMinutes;
                    }

                    break;

                case 3:
                    _webPrice = Convert.ToDouble(pricePerMin.SettingsValue);
                    _basePrice = Convert.ToDouble(basePrice.SettingsValue);
                    _totalPrice = _webPrice + _basePrice;
                    break;
            }


            JobsService.UpdateJobPrice(add.JobId, _totalPrice);

            if (job.UserId != null)
            {
                payment.UserId = job.UserId;
            }
            else
            {
                payment.UserId = job.Phone;
            }

            payment.ExternalCardIdNonce = job.PaymentNonce;
            payment.ItemCost = (decimal)_totalPrice;

            String TokenHash = _UserProfileService.GetTokenHashByUserId(job.UserId);
            Guid TokenGuid;
            Guid.TryParse(TokenHash, out TokenGuid);


            //This section was added by a team member
            if (TokenHash != null)
            {
                bool TokenUsed = TokenService.isTokenUsedReferral(TokenHash);

                //then find the user who referred userB
                //second service: get the userId of userA by using the tokenhash of userB
                //take User B's email and find the user id of the person who referred them (userA) and 
                //use the [dbo].[Token_SelectByUserIdAndTokenType] and take UserId from the stored proc

                //[dbo].[Token_GetByGuid]

                //NOTE: User A is the initial friend who referred User B.

                Token GetUserA = TokenService.userGetByGuid(TokenGuid);

                string UserAId = GetUserA.UserId;
                int CouponReferral = GetUserA.TokenType;
                TokenType referral = (TokenType)CouponReferral; //parsing the int into an enum

                if (UserAId != null && referral == TokenType.Invite && TokenUsed == false) //if this user was referred from a friend && that referral coupon type  is 3 && if that coupon is not used, do the thing
                {

                    //give User A a credit of 25 dollars
                    CouponsDomain userCoupon = TokenService.GetReferralTokenByGuid(TokenHash);

                    UserCreditsRequest insertUserACredits = new UserCreditsRequest();
                    insertUserACredits.Amount = userCoupon.CouponValue;
                    insertUserACredits.TransactionType = "Add";
                    insertUserACredits.UserId = UserAId;
                    //_CreditsService.InsertUserCredits(insertUserACredits); // get int value for it and plug it ino the targetValue in activitylogrequest
                    int forTargetValue = _CreditsService.InsertUserCredits(insertUserACredits);


                    //then update the activity log for USER A to tell them that their friend completed their first order and that they were rewarded credits
                    ActivityLogAddRequest addCreditFriend = new ActivityLogAddRequest();

                    addCreditFriend.ActivityType = ActivityTypeId.CreditsFriend;
                    addCreditFriend.JobId = job.Id;
                    addCreditFriend.TargetValue = forTargetValue;
                    addCreditFriend.RawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(insertUserACredits);
                    _ActivityLogService.Insert(UserAId, addCreditFriend);

                    //update user B's activity log to show that they used the credits for their first payment
                    ActivityLogAddRequest addCredit = new ActivityLogAddRequest();

                    addCredit.ActivityType = ActivityTypeId.Credits;
                    addCredit.JobId = job.Id;
                    addCredit.TargetValue = forTargetValue;
                    addCredit.RawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(insertUserACredits);
                    _ActivityLogService.Insert(UserAId, addCredit);
                }
            }

            bool successpay = AdminPaymentService(payment, job.Id);

            if(successpay)
            {
                JobsService.UpdateJobStatus(JobStatus.Complete, job.ExternalJobId);

                ActivityLogAddRequest log = new ActivityLogAddRequest();
                log.JobId = job.Id;
                log.TargetValue = (int)JobStatus.Complete;
                log.ActivityType = ActivityTypeId.BringgTaskStatusUpdated;

                _ActivityLogService.Insert((job.UserId == null) ? job.Phone : job.UserId, log);
            }
            else
            {
                success = false;
            }

            return success;
        }

    }
}