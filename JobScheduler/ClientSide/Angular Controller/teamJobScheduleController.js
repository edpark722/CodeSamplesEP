(function () {
    "use strict";

    angular.module(APPNAME)
        .controller('scheduleController', ScheduleController)
        .filter('utcToLocal', Filter);

    ScheduleController.$inject = ['$scope', '$baseController', "$websiteTeamService", "$jobScheduleService", "$filter", "$uibModal", "$routeParams"];

    function ScheduleController(
        $scope
        , $baseController
        , $websiteTeamService
        , $jobScheduleService
        , $filter
        , $uibModal
        , $routeParams) {



        var vm = this;//this points to a new {}
        vm.$filter = $filter;
        vm.$websiteTeamService = $websiteTeamService;
        vm.$uibModal = $uibModal;
        vm.$jobScheduleService = $jobScheduleService;
        vm.$scope = $scope;
        vm.$routeParams = $routeParams;
        vm.teams = {
            id: parseInt($routeParams.id)
        };
        vm.timeSlots = {};
        vm.timeSlotOverride = {};
        vm.asapTimeSlotToday = {};
        vm.currentDate = _convertDateToTodayFormat(new Date());
        vm.currentHistoryDate = _convertDateToTodayFormat(new Date());
        vm.zeroDateToday = _convertDateToZeroTime(new Date());
        vm.renderCalendar = false;
        vm.editTimeSlotId = null;
        vm.addOverrideShow = false;
        vm.newOverrideSlot = {};
        vm.insertTimeSlotId = null;
        vm.updateTimeSlotResult = null;
        vm.deleteResult = null;
        vm.deleteSlotId = null;
        vm.renderOverrideTable = false;
        vm.renderOverrideHistoryTable = false;
        vm.modalType = null;
        vm.showDefaultTimeSlots = true;
        vm.showAddOverrideButton = true;
        vm.queryDay = _getDayOfWeekFromDate(new Date());
        vm.overrideTimeSlotsByDate = {};
        vm.defaultTimeSlotsByDay = {};
        vm.currentZeroDate = {
            queryDate: null
        };
        vm.defaultTimeSlotsCheckBoxOptions = [];
        vm.dateOptions = {
            customClass: _getOverrideDayClass,
            minDate: new Date(),
            showWeeks: true
        };
        vm.dateHistoryOptions = {
            customClass: _getOverrideDayClass,
            maxDate: new Date(),
            showWeeks: true
        };
        vm.capacityOptions = [0, 1, 2, 3, 4, 5, 6, 7, 8];
        vm.dayOfWeekOptions = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
        
        //Hoists
        vm.today = _today;
        vm.clear = _clear;
        vm.grabDate = _grabDate;
        vm.openModal = _openModal;
        vm.insertModal = _insertModal;
        vm.deleteModal = _deleteModal;
        vm.todayHistory = _todayHistory;
        vm.clearHistory = _clearHistory;
        vm.grabHistoryDate = _grabHistoryDate;
        vm.filterByNewDay = _filterByNewDay;
        vm.onCheckChange = _onCheckChange;
        vm.updateAsapModal = _updateAsapModal;
        vm.teamChange = _teamChange;

        $baseController.merge(vm, $baseController);
        vm.notify = vm.$jobScheduleService.getNotifier($scope);

        console.log("slugRoute", vm.$routeParams.Slug);

        render();

        function render() {
            _getAllTeams();
            _getAllTimeSlotsByTeamId();
            _today();

        }

        function _grabDate() {
            _getOverrideByDate();
            vm.addOverrideShow = false;
        }

        //Used for getting Defaults to fill the checkbox options on the Add override section
        function _getDefaultsByDay() {
            console.log("Current Date Selected: ", vm.currentDate);
            var payload = {
                queryDate: null
            , queryDay: _getDayOfWeekFromDate(new Date(vm.currentDate))
            , teamId: vm.teams.id
            }
            console.log("Get Defaults For Time Zones DropDown Payload: ", payload);
            vm.$jobScheduleService.getTimeSlotByDate(payload, _getDefaultsByDaySuccess, _getDefaultsByDayError)
        }

        function _getDefaultsByDaySuccess(data) {
            vm.notify(function () {
                vm.defaultTimeSlotsByDay = data.items;
                vm.defaultTimeSlotsCheckBoxOptions = _convertDefaultTimeListToArray(vm.defaultTimeSlotsByDay);
                if (vm.defaultTimeSlotsCheckBoxOptions.length === 0) {
                    vm.showAddOverrideButton = false;
                }
                if (vm.defaultTimeSlotsCheckBoxOptions.length > 0) {
                    vm.showAddOverrideButton = true;
                }
                console.log("Default Time Slots For Override Drop Down By Day: ", vm.defaultTimeSlotsByDay);
            });
        }

        function _getDefaultsByDayError(jqXhr, error) {
            console.error(error);
        }

        //Used to convert the Time Slots for the Override Checkbox into an array, to be used for the override checkbox form
        function _convertDefaultTimeListToArray(list) {
            var array = [];
            for (var i = 0; i < list.length; i++) {
                var object = {}
                object.id = list[i].id;
                object.name = list[i].timeStart + " to " + list[i].timeEnd;
                object.selected = false;
                object.timeStart = list[i].timeStart;
                object.timeEnd = list[i].timeEnd;
                object.dayOfWeek = list[i].dayOfWeek;
                object.capacity = list[i].capacity;
                object.teamId = list[i].teamId;
                object.scheduleType = list[i].scheduleType;
                array.push(object);
            }
            console.log("Override Time Slots By Date to be used for cancelling existing slots: ", vm.overrideTimeSlotsByDate);

            //Omit existing override slots from checkbox
            for (var i = 0; i < array.length; i++) {
                for (var x = 0; x < vm.overrideTimeSlotsByDate.length; x++) {
                    if (array[i].id === vm.overrideTimeSlotsByDate[x].defaultId) {
                        console.log("Array with match: ", array[i].id);
                        console.log("Override Time Slot with Match: ", vm.overrideTimeSlotsByDate[x].defaultId);
                        array.splice(i, 1);
                    }
                }
            }

            console.log("New AVAILBLE Default Time Slots For Override Drop Down By Day: ", array);
            return array;
        }

        function _grabHistoryDate() {
            _getOverrideHistoryByDate();
        }

        function _deleteTimeSlot() {
            console.log("Delete Id: ", vm.deleteSlotId);
            vm.$jobScheduleService.deleteTimeSlot(vm.deleteSlotId, _deleteTimeSlotSuccess, _deleteTimeSlotError);
        }

        function _deleteTimeSlotSuccess(data) {
            vm.notify(function () {
                vm.deleteResult = data.item;
                console.log("Delete Result: ", vm.deleteResult)
                _getAllTimeSlotsByTeamId();
                _getOverrideByDate();
                vm.$alertService.success("Delete was Successful!");

            });
        }

        function _deleteTimeSlotError(jqXhr, error) {
            console.error(error);
        }

        function _getOverrideByDate() {
            console.log("Current Date Selected: ", vm.currentDate);
            vm.currentZeroDate.queryDay = _getDayOfWeekFromDate(new Date(vm.currentDate));
            vm.currentZeroDate.queryDate = _convertDateToZeroTime(new Date(vm.currentDate));
            vm.currentZeroDate.teamId = vm.teams.id;
            console.log("Data Payload For Getting Override Schedule Time Slots For Date: ", vm.currentZeroDate);
            vm.$jobScheduleService.getAvailableByDate(vm.currentZeroDate, _getOverrideByDateSuccess, _getOverrideByDateError)
        }

        function _getOverrideByDateSuccess(data) {
            vm.notify(function () {
                vm.overrideTimeSlotsByDate = data.items;
                _getDefaultsByDay();
                for (var i = 0; i < vm.overrideTimeSlotsByDate.length; i++) {
                    if (vm.overrideTimeSlotsByDate[i].defaultId === null) {
                        vm.overrideTimeSlotsByDate[i].disableShow = true;
                    }
                }
                console.log("Override Time Slots For Selected Date: ", vm.overrideTimeSlotsByDate);
                if (vm.overrideTimeSlotsByDate.length === 0) {
                    vm.renderOverrideTable = false;
                }
                else {
                    vm.renderOverrideTable = true;
                }
            });
        }

        function _getOverrideByDateError(jqXhr, error) {
            console.error(error);
        }

        function _organizeOverrideAndDefault() {

        }

        function _onCheckChange(id) {
            console.log("Check Box Change Data: ", id);
        }

        function _submitOverride() {
            console.log("Defaults Selected", vm.defaultTimeSlotsCheckBoxOptions);
            //newOverrideSlot comes from Time Frame Ng-Model of Overrides
            console.log("Before Converted Date: ", vm.currentDate);
            for (var i = 0; i < vm.defaultTimeSlotsCheckBoxOptions.length; i++) {
                if (vm.defaultTimeSlotsCheckBoxOptions[i].selected === true) {
                    if (vm.defaultTimeSlotsCheckBoxOptions[i].capacity != vm.newOverrideSlot.capacity) {
                        //transferring the original default time slot data to override
                        var newOverrideObject = {};
                        newOverrideObject.timeStart = vm.defaultTimeSlotsCheckBoxOptions[i].timeStart;
                        newOverrideObject.timeEnd = vm.defaultTimeSlotsCheckBoxOptions[i].timeEnd
                        newOverrideObject.date = _convertDateToZeroTime(vm.currentDate);
                        newOverrideObject.defaultId = vm.defaultTimeSlotsCheckBoxOptions[i].id;
                        newOverrideObject.dayOfWeek = vm.defaultTimeSlotsCheckBoxOptions[i].dayOfWeek;
                        newOverrideObject.capacity = vm.newOverrideSlot.capacity;
                        newOverrideObject.teamId = vm.defaultTimeSlotsCheckBoxOptions[i].teamId;
                        newOverrideObject.scheduleType = vm.defaultTimeSlotsCheckBoxOptions[i].scheduleType;
                        //date and capacity made here*  capacity from ng-model and date from currrentdate to zero time
                        _insertNewTimeSlot(newOverrideObject);
                    }
                    else {
                        vm.$alertService.error("Capacity cannot be the same as the current default!");
                    }
                }

            }
        }

        function _insertNewTimeSlot(data) {
            console.log("New Override Object Data: ", data);
            vm.$jobScheduleService.insertNewTimeSlot(data, _insertNewTimeSlotSuccess, _insertNewTimeSlotError)
        }

        function _insertNewTimeSlotSuccess(data) {
            vm.notify(function () {
                vm.insertTimeSlotId = data.item;
                console.log("New Override Id: ", vm.insertTimeSlotId);
                //re render the Time Slots and Calendar
                vm.$alertService.success("Insert was Successful!");
                _clearOverrideForm();
                //re render override table
                _getAllTimeSlotsByTeamId();
                _getOverrideByDate();
            });
        }

        function _insertNewTimeSlotError(jqXhr, error) {
            console.error(error);
            console.log(jqXhr);
            vm.$alertService.error(jqXhr.responseText);
        }

        function _convertDateToZeroTime(date) {
            var zeroFormat = vm.$filter('date')(date, "yyyy/MM/dd");
            //console.log("Zero Format Date: ", date);
            return zeroFormat;
        }

        function _convertDateToTodayFormat(date) {
            var todayDate = vm.$filter('date')(date, "fullDate");
            return todayDate;
        }

        function _getDayOfWeekFromDate(date) {
            var dayOfWeek = vm.$filter('date')(date, "EEEE");
            return dayOfWeek;
        }

        //Custom class to show overridden days as red on calendar
        function _getOverrideDayClass(date) {
            if (vm.timeSlotOverride.length > 0) {
                if (date.mode === 'day') {
                    var calendarDate = _convertDateToZeroTime(date.date);
                    for (var i = 0; i < vm.timeSlotOverride.length; i++) {
                        var overrideDate = _convertDateToZeroTime(vm.timeSlotOverride[i].date);
                        if (calendarDate === overrideDate) {
                            return "full";
                            continue;
                        }
                    }
                }
            }
            return '';
        }

        //Date functions
        function _today() {
            vm.currentDate = _convertDateToTodayFormat(new Date());
        }

        function _clear() {
            vm.currentDate = null;
        }

        function _todayHistory() {
            vm.currentHistoryDate = _convertDateToTodayFormat(new Date());
        }

        function _clearHistory() {
            vm.currentHistoryDate = null;
        }

        function _onTeamSelect() {


        }

        //New Service to get the teams
        function _getAllTeams() {
            vm.$websiteTeamService.getAllTeams(_getAllTeamsSuccess, _getAllTeamsError)
        }

        function _getAllTeamsSuccess(data) {
            vm.notify(function () {
                vm.teams = data.items;
                vm.teams.id = parseInt($routeParams.id)
                for (var i = 0; i < vm.teams.length; i++) {
                    if (vm.teams[i].id == vm.teams.id) {
                        vm.teams[i].selected = true;
                    }
                }

                console.log("teams: ", vm.teams);
                console.log("Team Id after Get ALL: ", vm.teams.id);
            });

        }

        function _getAllTeamsError(jqXhr, error) {
            console.error(error);
        }

        function _getAllTimeSlotsByTeamId() {
            var payload = {
                queryDay: vm.queryDay
            }
            console.log("Team Id for Payload: ", vm.teams.id);
            console.log("Get Team Slots Payload: ", payload);
            vm.$jobScheduleService.getByTeamId(vm.teams.id, payload, _getAllTimeSlotsByTeamIdSuccess, _getAllTimeSlotsByTeamIdError)

        }

        function _getAllTimeSlotsByTeamIdSuccess(data) {
            vm.notify(function () {
                console.log("Full Time Slots: ", data);
                vm.timeSlots = data.items;
                vm.timeSlotOverride = data.overrideItems;
                vm.asapTimeSlot = data.asapItems;
                vm.renderCalendar = false;
                _grabTodayAsapTimeSlot();
                _getOverrideByDate();
                _getOverrideHistoryByDate();
            });
            vm.notify(function () {
                vm.renderCalendar = true;
                if (vm.timeSlots.length === 0) {
                    vm.showDefaultTimeSlots = false;
                }
                if (vm.timeSlots.length > 0) {
                    vm.showDefaultTimeSlots = true;
                }
            });
        }

        function _getAllTimeSlotsByTeamIdError(jqXhr, error) {
            console.error(error);
        }

        //Grabs ASAP Time Slot if Made for the day.  Still require hangfire backup...
        function _grabTodayAsapTimeSlot() {
            for (var i = 0; i < vm.asapTimeSlot.length; i++) {
                vm.asapTimeSlot[i].date = _convertDateToZeroTime(vm.asapTimeSlot[i].date);
                console.log("Asap Item Date: ", vm.asapTimeSlot[i].date);
                console.log("Zero Date Today: ", vm.zeroDateToday);
                if (vm.asapTimeSlot[i].date === vm.zeroDateToday) {
                    vm.asapTimeSlotToday = vm.asapTimeSlot[i];
                    if (vm.asapTimeSlotToday.capacity === 0) {
                        vm.asapTimeSlotToday.killOn = "On";
                    }
                    if (vm.asapTimeSlotToday.capacity === 1) {
                        vm.asapTimeSlotToday.killOn = "Off";
                    }
                }
            }
            console.log("ASAP Time Slot For Today: ", vm.asapTimeSlotToday);
        }

        //activate kill switch for ASAP time slot
        function _onKillSwitchClick() {
            if (vm.asapTimeSlotToday.killOn === "On") {
                vm.asapTimeSlotToday.capacity = 1;
            }
            else if (vm.asapTimeSlotToday.killOn === "Off") {
                vm.asapTimeSlotToday.capacity = 0;
            }
            _updateTimeSlot();
        }

        function _updateTimeSlot() {
            console.log("Update Kill Switch Data: ", vm.asapTimeSlotToday);
            vm.$jobScheduleService.updateTimeSlot(vm.asapTimeSlotToday.id, vm.asapTimeSlotToday, _updateTimeSlotSuccess, _updateTimeSlotError)
        }

        function _updateTimeSlotSuccess(data) {
            vm.notify(function () {
                vm.updateTimeSlotResult = data.item;
                console.log("Update ASAP Result: ", vm.updateTimeSlotResult);
                if (vm.asapTimeSlotToday.killOn === "On") {
                    vm.$alertService.success("Kill Switch Successfully Turned Off!");
                    _getAllTimeSlotsByTeamId();

                }
                else if (vm.asapTimeSlotToday.killOn === "Off") {
                    vm.$alertService.warning("Kill Switch Successfully Turned On!");
                    _getAllTimeSlotsByTeamId();

                }
            });
        }

        function _updateTimeSlotError(jqXhr, error) {
            console.error(error);
        }

        //Open Modal for DEFAULT insert/update
        function _openModal(id) {
            vm.editTimeSlotId = id;
            console.log("Time Slot to Edit: ", vm.editTimeSlotId);
            var modalInstance = vm.$uibModal.open({
                animation: true,
                templateUrl: '/Assets/Themes/bringpro/js/features/backoffice/templates/Websites/teamJobScheduleDefaultModal.html',
                controller: 'jobScheduleModalController as mc',
                size: 'lg',
                resolve: {
                    items: function () {
                        return vm.editTimeSlotId;
                    },
                    teamId: function () {
                        return vm.teams.id;
                    }
                }
            });

            modalInstance.result.then(function () {
                _getAllTimeSlotsByTeamId();
                _getDefaultsByDay();
                _getOverrideByDate();
            }, function () {
                console.log('Update Modal dismissed at: ' + new Date());
            });
        }

        function _clearOverrideForm() {
            vm.newOverrideSlot.id = "";
            vm.newOverrideSlot.capacity = "";
            vm.overrideForm.$setPristine();
            vm.addOverrideShow = false;
        }

        //Functions to activate different Modal Confirmations
        function _insertModal() {
            vm.modalType = 1;
            _openConfirmModal();
        }

        function _deleteModal(id) {
            vm.deleteSlotId = id;
            vm.modalType = 2;
            _openConfirmModal();
        }

        function _updateAsapModal() {
            vm.modalType = 3;
            _openConfirmModal();
        }

        //Modal for confirming Overrides
        function _openConfirmModal() {
            console.log("Modal Type: ", vm.modalType);
            var modalInstance = vm.$uibModal.open({
                animation: true,
                templateUrl: '/Assets/Themes/bringpro/js/features/backoffice/templates/Websites/teamJobScheduleConfirmOverrideModal.html',
                controller: 'modalConfirmInsertController as mcc',
                size: 'sm',
                resolve: {
                    modalType: function () {
                        return vm.modalType;
                    },
                    killStatus: function () {
                        return vm.asapTimeSlotToday.capacity;
                    }
                }
            });

            modalInstance.result.then(function () {
                if (vm.modalType === 1) {
                    console.log("Inserting Into Table!");
                    _submitOverride();
                }
                if (vm.modalType === 2) {
                    console.log("Deleting Time Slot!");
                    _deleteTimeSlot();
                }
                if (vm.modalType === 3) {
                    console.log("Updating ASAP Time Slot!");
                    _onKillSwitchClick();
                }


            }, function () {
                console.log('Update Modal dismissed at: ' + new Date());
            });
        }

        function _getOverrideHistoryByDate() {
            console.log("Current History Date Selected: ", vm.currentHistoryDate);
            vm.currentZeroDate.queryDay = _getDayOfWeekFromDate(new Date(vm.currentHistoryDate));
            vm.currentZeroDate.queryDate = _convertDateToZeroTime(new Date(vm.currentHistoryDate));
            vm.currentZeroDate.teamId = vm.teams.id;
            console.log("Data Payload For Getting Override History Time Slots For Date: ", vm.currentZeroDate);
            vm.$jobScheduleService.getTimeSlotByDate(vm.currentZeroDate, _getOverrideHistoryByDateSuccess, _getOverrideHistoryByDateError)
        }

        function _getOverrideHistoryByDateSuccess(data) {
            vm.notify(function () {
                vm.overrideTimeSlotsHistoryByDate = data.items;
                console.log("Override Time Slots For Selected HISTORY Date: ", vm.overrideTimeSlotsHistoryByDate);
                if (vm.overrideTimeSlotsHistoryByDate.length === 0) {
                    vm.renderOverrideHistoryTable = false;
                }
                else {
                    vm.renderOverrideHistoryTable = true;
                }
            });
        }

        function _getOverrideHistoryByDateError(jqXhr, error) {
            console.error(error);
        }

        //Filter for Default Time Slots
        function _filterByNewDay() {
            console.log("New Query Day: ", vm.queryDay);
            _getAllTimeSlotsByTeamId();
        }

        //On Dropdown Change
        function _teamChange() {
            console.log("Current Team Id Selected: ", vm.teams.id);
            _getAllTimeSlotsByTeamId();
        }


    }
})();