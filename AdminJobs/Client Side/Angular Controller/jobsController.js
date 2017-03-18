(function () {

    "use strict";

    angular.module(APPNAME)
        .controller('jobsController', JobsController)
        .filter('utcToLocal', Filter)
        .filter('phoneNumber', filter);
    JobsController.$inject = ["$scope", "$baseController", "$jobsService", "$uibModal", "$websiteService", "$filter"]

    function JobsController(
        $scope
        , $baseController
        , $jobsService
        , $uibModal
        , $websiteService
        , $filter) {

        var vm = this;
        vm.jobs = null;
        vm.selectedJobs = null;
        vm.search = null;
        vm.websites = {};
        vm.filters = {
            websiteId: null
            , itemsPerPage: 10
            , currentPage: 1
            , totalItems: 0
        };
        vm.firstNumber = null;
        vm.secondNumber = null;

        vm.$jobsService = $jobsService;

        console.log(vm.$jobsService);
        vm.$scope = $scope;
        vm.$uibModal = $uibModal;
        vm.$websiteService = $websiteService;
        vm.$filter = $filter;
        vm.getsResultsPage = 1;

        //Parse and convert Enum to Array for ng-option
        vm.jobsTypeEnum = JSON.parse($("#jobsType").html());
        vm.jobsStatusEnum = JSON.parse($("#jobsStatus").html());
        vm.statusConverted = convertJobsStatusEnumToArray(vm.jobsStatusEnum);
        vm.jobTypeConverted = convertJobsTypeEnumToArray(vm.jobsTypeEnum);

        //Hoist Page Functions
        vm.emptySearch = _emptySearch;
        vm.grabDate = _grabDate;
        vm.openModal = _openModal;
        vm.deleteSetting = _deleteSetting;
        vm.pageChanged = _pageChanged;
        vm.searchJobs = _searchJobs;
        vm.open1 = _open1;
        vm.open2 = _open2;

        vm.popup1 = {
            opened: false
        }
        vm.popup2 = {
            opened: false
        }
        vm.startDateOptions = {
            formatYear: 'yy',
            minDate: new Date(2017, 0, 22),
            showWeeks: false
        };

        vm.endDateOptions = {
            formatYear: 'yy',
            minDate: new Date(),
            showWeeks: false
        };

        vm.notify = vm.$jobsService.getNotifier($scope);


        render();


        function render() {
            console.log('vm.filters', vm.filters);
            _getPaginationWithFilter();
            _getWebsiteAccounts();

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

        function _pageChanged(newPage) {
            vm.filters.currentPage = newPage;
            _getPaginationWithFilter();
        }

        function _deleteJob(jobId) {
            vm.$jobsService.delete(jobId, vm.onDeleteSuccess, vm.onDeleteError);

        }

        function _deleteSetting(jobId) {
            vm.jobId = jobId;
            vm.openModal();
        }

        function _onDeleteSuccess(data) {
            console.log('data: ' + data);
            _getPaginationWithFilter();
        }

        function _onDeleteError(jqXhr, error) {
            console.error(error);
        }

        function convertJobsStatusEnumToArray(statusEnum) {
            console.log("what's in jobs status enum", statusEnum);

            var statusArrayObj = [];
            $.each(statusEnum, function (index, value) {
                var jobStatusObj = {};
                jobStatusObj.id = index;
                jobStatusObj.name = value;
                statusArrayObj.push(jobStatusObj);

            })

            console.log("status array", statusArrayObj);
            return statusArrayObj;
        }

        //These functions used for Dropdowns for ng-options.
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

        function _searchJobs() {
            _getPaginationWithFilter();
            console.log('search firing', vm.filters.query);
        }

        //Clear Filters/Search 
        function _emptySearch() {
            vm.filters.query = "";
            vm.filters.queryJobType = "";
            vm.filters.queryStatus = "";
            vm.filters.queryWebsiteId = "";
            vm.filters.startDate = "";
            vm.filters.endDate = "";
            vm.filters.queryStartDate = "";
            vm.filters.queryEndDate = "";
            _getPaginationWithFilter();
        }

        //Modal for Delete Confirmation
        function _openModal() {
            var modalInstance = vm.$uibModal.open({
                animation: true,
                templateUrl: "/Assets/Themes/bringpro/js/features/backoffice/templates/jobs/jobsIndexModal.html",
                controller: 'jobsModalController as mc',
                size: 'md',
            });

            modalInstance.result.then(function () {
                console.log("current Id when delete happens: ", vm.jobId);
                vm.$jobsService.delete(vm.jobId, vm.onDeleteSuccess, vm.onDeleteError);
                _getPaginationWithFilter();

            }, function () {
                console.log('Modal dismissed at: ' + new Date());
            });
        }

        //ng-click to open the Calendar
        function _open1() {
            vm.popup1.opened = true;
        }

        function _open2() {
            vm.popup2.opened = true;
        }

        //logging the change
        function _grabDate() {
            vm.endDateOptions.minDate = vm.filters.startDate;
            console.log("Website Filter: ", vm.filters.queryWebsiteId);
            console.log("Status Filter: ", vm.filters.queryStatus);
            console.log("Job Type Filter: ", vm.filters.queryJobType);
            console.log("Start Date: ", vm.filters.startDate);
            console.log("End Date: ", vm.filters.endDate);

            vm.filters.queryStartDate = _convertDateTime(vm.filters.startDate);
            vm.filters.queryEndDate = _convertDateTime(vm.filters.endDate);

            _getPaginationWithFilter();
        }

        function _convertDateTime(date) {
            date = vm.$filter('date')(date, "yyyy/MM/dd HH:mm:ss", 'UTC');
            console.log("C# Format Date: ", date);
            return date
        }

        //Main function to grab new data with every Query Filter, Search Bar, and Pagination
        function _getPaginationWithFilter() {
            vm.$jobsService.getPaginationWithFilter(vm.filters, _getPaginationWithFilterSuccess, _getPaginationWithFilterError)

        }

        //Logic for the pagination
        function _getPaginationWithFilterSuccess(data) {
            console.log('Filter Data', data.items)

            vm.notify(function () {
                vm.jobs = data.items;
                vm.filters.totalItems = data.totalItems;

                vm.firstNumber = ((vm.filters.currentPage - 1) * vm.filters.itemsPerPage) + 1;

                if (vm.filters.currentPage * 10 < vm.filters.totalItems) {
                    vm.secondNumber = (vm.filters.currentPage * vm.filters.itemsPerPage);
                } else {
                    vm.secondNumber = vm.filters.totalItems;
                }

                window.scrollTo(0, 0);
            });
        }

        function _getPaginationWithFilterError(jqXhr, error) {
            console.error("error with filter", error);
        }



    }

})();