(function () {
    "use strict";

    angular.module(APPNAME)
        .controller("jobsFormController", jobsFormController);
    jobsFormController.$inject = ["$scope", "$baseController", "$jobFormService", "$uibModal", "$routeParams", "$websiteService"];

    function jobsFormController(
        $scope
        , $baseController
        , $jobFormService
        , $uibModal
        , $routeParams
        , $websiteService) {

        var vm = this;
        vm.$routeParams = $routeParams

        //you need the RouteParams.ID
        console.log("Jobs Id", vm.$routeParams.id);

        //instantiate the values
        vm.$jobFormService = $jobFormService;
        vm.$websiteService = $websiteService;
        vm.$scope = $scope;
        vm.$uibModal = $uibModal;
        vm.openModal = _openModal;
        vm.websites = null;
        vm.currentJob = null;
        vm.jobId = vm.$routeParams.id;

        //Section below is from the Database
        vm.jobsTypeEnum = JSON.parse($("#jobsType").html());
        vm.jobsStatusEnum = JSON.parse($("#jobsStatus").html());
        vm.statusConverted = convertJobsStatusEnumToArray(vm.jobsStatusEnum);
        vm.jobTypeConverted = convertJobsTypeEnumToArray(vm.jobsTypeEnum);

        //Hoist Page Functions
        vm.jobFormSubmit = _jobFormSubmit;
        vm.deleteSetting = _deleteSetting;

        $baseController.merge(vm, $baseController);

        vm.notify = vm.$jobFormService.getNotifier($scope);

        render();

        function render() {

            _getWebsiteAccounts();

            if (vm.jobId) {
                console.log("Edit Mode");
                vm.$jobFormService.getById(vm.jobId, vm.receiveJobItem, vm.onRenderJobError);
            }
            else {
                console.log("create mode");
            }

        }

        function _receiveJobItem(data) {
            vm.notify(function () {
                vm.currentJob = data.item;
                vm.currentJob.jobType = data.item.jobType.toString();
                vm.currentJob.status = data.item.status.toString();
            });
        }

        function _onRenderJobError(jqXhr, error) {
            console.log("Error ", error);

        }

        function _getWebsiteAccounts() {
            vm.$websiteService.get(_getWebsiteAccountsSuccess, _getWebsiteAccountsError);
        }

        function _getWebsiteAccountsSuccess(data) {
            vm.notify(function () {
                vm.websites = data.items;
                console.log("websites:", vm.websites);
            });
        }

        function _getWebsiteAccountsError(jqXhr, error) {
            console.error("error with get all", error);
        }

        function _jobFormSubmit(isValid) {
            if (isValid) {
                console.log("Form Valid")
                if (vm.jobId) {
                    vm.$jobFormService.update(vm.jobId, vm.currentJob, vm.onUpdateJobsSuccess, vm.onUpdateJobError);

                }
                else {
                    vm.$jobFormService.insert(vm.currentJob, vm.onInsertJobSuccess, vm.onInsertJobError);

                }
            }
            else {
                console.log("Form Invalid")
            }
        }

        function _deleteSetting(jobId) {
            vm.openModal();
        }

        function _onDeleteSuccess(data) {

            vm.notify(function () {

                console.log("Successfully Deleted this Job. Success:", data.item);
            });
        }

        function _onDeleteError(jqXhr, error) {
            console.error(error);
        }

        function _onUpdateJobsSuccess(data) {

            console.log("Successfully Updated this Job. Success:", data.item);
            location.href = "/backoffice";

        }

        function _onUpdateJobError(jqXhr, error) {
            console.error(error);
        }

        function _onInsertJobSuccess(data) {
            console.log("Insert Job Successful", data);
            location.href = "/backoffice";

        }

        function _onInsertJobError(jqXhr, error) {
            console.error('Error on insert', error);
        }


        function convertJobsTypeEnumToArray(jobsTypeEnum) {
            console.log("what's in jobs type enum", jobsTypeEnum);

            var typeArrayObj = [];
            $.each(jobsTypeEnum, function (index, value) {
                var jobTypeObj = {};
                jobTypeObj.id = index;
                jobTypeObj.name = value;
                typeArrayObj.push(jobTypeObj);
            })

            console.log("types array", typeArrayObj);
            return typeArrayObj;
        }

        function convertJobsStatusEnumToArray(jobsStatusEnum) {
            console.log("what's in jobs status enum", jobsStatusEnum);

            var statusArrayObj = [];
            $.each(jobsStatusEnum, function (index, value) {
                var jobStatusObj = {};
                jobStatusObj.id = index;
                jobStatusObj.name = value;
                statusArrayObj.push(jobStatusObj);

            })

            console.log("status array", statusArrayObj);
            return statusArrayObj;
        }

        function _openModal() {
            var modalInstance = vm.$uibModal.open({
                animation: true,
                templateUrl: '/Assets/Themes/bringpro/js/features/backoffice/templates/jobs/jobsFormDeleteModal.html',
                controller: 'jobsFormModalController as mc',
                size: 'md',
            });

            modalInstance.result.then(function () {
                console.log("current Id when delete happens: ", vm.jobId);
                vm.$jobFormService.delete(vm.jobId, vm.onDeleteSuccess, vm.onDeleteError);
                location.href = "/jobs/";

            }, function () {
                console.log('Modal dismissed at: ' + new Date());
            });
        }
    }

})();