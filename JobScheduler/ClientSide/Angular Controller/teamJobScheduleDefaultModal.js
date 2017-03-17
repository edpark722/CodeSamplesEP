(function () {
    "use strict";

    angular.module(APPNAME)
        .controller('jobScheduleModalController', JobScheduleModalController);

    //  $uibModalInstance is coming from the UI Bootstrap library and is a reference to the modal window itself so we can work with it
    //  items is the array passed in from the main controller above through the resolve property
    JobScheduleModalController.$inject = ['$scope', '$baseController', '$uibModalInstance', 'items', '$jobScheduleService', "$uibModal", "teamId"]

    function JobScheduleModalController(
        $scope
        , $baseController
        , $uibModalInstance
        , items
        , $jobScheduleService
        , $uibModal
        , teamId) {

        var vm = this;
        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.$jobScheduleService = $jobScheduleService;
        vm.$uibModal = $uibModal;

        vm.updateTimeSlotResult = null;
        vm.modalTitle = '';
        vm.timeSlotInfo = {
            teamId: teamId
        };
        vm.insertTimeSlotId = null;

        vm.submitEdit = _submitEdit;

        $baseController.merge(vm, $baseController);

        vm.notify = vm.$jobScheduleService.getNotifier($scope);

        vm.capacityOptions = [0, 1, 2, 3, 4, 5, 6, 7, 8];

        vm.dayOfWeekOptions = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

        vm.editTimeSlotId = items;

        render();

        function render() {
            console.log("Default Id to Edit Val: ", vm.editTimeSlotId);
            if (vm.editTimeSlotId > 0) {
                console.log("Edit Mode");
                _getTimeSlotById();
                _editMode();
            }
            if (vm.editTimeSlotId === 0 || vm.editTimeSlotId === null) {
                console.log("Create Mode");
                _createMode();
            }
        }

        function _editMode() {
            vm.modalTitle = "Edit Default Time Slot"
            vm.modalButton = "Edit"
        }

        function _createMode() {
            vm.modalTitle = "Create New Default Time Slot"
            vm.modalButton = "New Default"
        }

        function _getTimeSlotById() {
            vm.$jobScheduleService.getTimeSlotsById(vm.editTimeSlotId, _getTimeSlotByIdSuccess, _getTimeSlotByIdError);
        }

        function _getTimeSlotByIdSuccess(data) {
            vm.notify(function () {
                vm.timeSlotInfo = data.item;
                console.log("time Slot edit info:", vm.timeSlotInfo);
            });
        }

        function _getTimeSlotByIdError(jqXhr, error) {
            console.error(error);
        }

        function _submitEdit() {
            console.log("submitting");
            if (vm.timeSlotInfo.timeStart >= vm.timeSlotInfo.timeEnd) {
                vm.$alertService.error("Time Start Must be EARLIER THAN Time End", "Error!");
            }
            else {
                _openModal();
            }
        }

        function _openModal() {
            var modalInstance = vm.$uibModal.open({
                animation: true,
                templateUrl: '/Assets/Themes/bringpro/js/features/backoffice/templates/Websites/teamJobScheduleConfirmDefaultModal.html',
                controller: 'modalConfirmController as mcc',
                size: 'sm',
                resolve: {
                    editId: function () {
                        return vm.editTimeSlotId;
                    }
                }
            });

            modalInstance.result.then(function () {
                if (vm.editTimeSlotId > 0) {
                    console.log("Update Time");
                    _updateTimeSlot();
                }
                if (vm.editTimeSlotId === 0 || vm.editTimeSlotId === null) {
                    console.log("Insert Time");
                    _insertNewTimeSlot();
                }
            }, function () {
                console.log('Modal dismissed at: ' + new Date());
            });
        }

        function _insertNewTimeSlot() {
            //vm.timeSlotInfo.scheduleType = true;
            console.log("New Time Slot Data: ", vm.timeSlotInfo);
            vm.$jobScheduleService.insertNewTimeSlot(vm.timeSlotInfo, _insertNewTimeSlotSuccess, _insertNewTimeSlotError);
        }

        function _insertNewTimeSlotSuccess(data) {
            vm.notify(function () {
                vm.insertTimeSlotId = data.item;
                console.log("New Override Id: ", vm.insertTimeSlotId);
                vm.$alertService.success("Insert was Successful!");
            });
            vm.ok();
        }

        function _insertNewTimeSlotError(jqXhr, error) {
            console.error(error);
        }

        function _updateTimeSlot() {
            //vm.timeSlotInfo.scheduleType = true;
            console.log("Update Slot Data: ", vm.timeSlotInfo);
            vm.$jobScheduleService.updateTimeSlot(vm.editTimeSlotId, vm.timeSlotInfo, _updateTimeSlotSuccess, _updateTimeSlotError)
        }

        function _updateTimeSlotSuccess(data) {
            vm.notify(function () {
                vm.updateTimeSlotResult = data.item;
                console.log("Update Result: ", vm.updateTimeSlotResult);
                vm.$alertService.success("Update was Successful!");
            });
            vm.ok();
        }

        function _updateTimeSlotError(jqXhr, error) {
            console.error(error);
        }

        //  $uibModalInstance is used to communicate and send data back to main controller
        vm.ok = function () {
            vm.$uibModalInstance.close();
        };

        vm.cancel = function () {
            vm.$uibModalInstance.dismiss('cancel');
        };
    }
})();