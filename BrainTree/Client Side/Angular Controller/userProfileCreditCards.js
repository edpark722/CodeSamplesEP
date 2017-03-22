(function () {
    "use strict";

    angular.module(APPNAME)
        .controller('billingController', BillingController);

    BillingController.$inject = ['$scope', '$baseController', "$billingService", "$userService", "$dashboardService", '$uibModal'];

    function BillingController(
        $scope
        , $baseController
        , $billingService
        , $userService
        , $dashboardService
        , $uibModal) {


        var vm = this;

        vm.customerCards = {};
        vm.delete = null;
        vm.update = null;



        //instantiate values
        vm.$billingService = $billingService;
        vm.$userService = $userService;
        vm.$dashboardService = $dashboardService;
        vm.$uibModal = $uibModal;
        vm.deleteCard = _deleteCard;
        vm.openModal = _openModal;
        vm.openConfirmModal = _openConfirmModal;
        vm.makeDefaultCard = _makeDefaultCard;
        vm.openDeleteModal = _openDeleteModal;

        vm.$scope = $scope;

        //-- this line to simulate inheritance
        $baseController.merge(vm, $baseController);

        //this is a wrapper for our small dependency on $scope
        vm.notify = vm.$billingService.getNotifier($scope);

        render();

        function render() {
            _getCustomerbyUserId();
            _getCurrentUserCCInfo();
            $("#jobsTab").removeClass("active");
            $("#aboutTab").removeClass("active");
            $("#creditCardsTab").addClass("active");

        }

        //TEST CODE FOR GETTING CUSTOMER INFO FROM BT
        function _getCustomerbyUserId() {

            vm.$billingService.getCustomerByUserId(_getCustomerbyUserIdSuccess, _getCustomerbyUserIdError);

        }

        function _getCustomerbyUserIdSuccess(data) {
            vm.notify(function () {
                vm.customerInfo = data.item;
                console.log("Braintree Customer Info:", vm.customerInfo);
            });
        }

        function _getCustomerbyUserIdError(jqXhr, error) {
            console.error(error);
        }

        function _getCurrentUserCCInfo() {

            vm.$dashboardService.getCurrentUserCCInfo(_getCurrentUserCCInfoSuccess, _getCurrentUserCCInfoError);

        }

        function _getCurrentUserCCInfoSuccess(data) {
            vm.notify(function () {
                for (var i = 0; i < data.items.length; i++) {
                    data.items[i].cardType = _renderPicture(data.items[i].cardType);
                }
                vm.customerCards = data.items;
                console.log("Card List:", vm.customerCards);
            });
        }

        function _getCurrentUserCCInfoError(jqXhr, error) {
            console.error(error);
        }

        function _deleteCard(nonce) {
            console.log("delete that shit", nonce);
            vm.$dashboardService.deleteUserCreditCardByNonce(nonce, _deleteUserCreditCardByNonceSuccess, _deleteUserCreditCardByNonceError)
        }

        function _deleteUserCreditCardByNonceSuccess(data) {
            vm.notify(function () {
                vm.delete = data.item;
                console.log("Ish Deleted:", vm.delete);
                _getCurrentUserCCInfo();
            });
        }

        function _deleteUserCreditCardByNonceError(jqXhr, error) {
            console.error(error);
        }

        function _renderPicture(type) {
            switch (type) {
                case "Visa":
                    return "http://www.credit-card-logos.com/images/visa_credit-card-logos/visa_logo_7.gif";
                    break;
                case "MasterCard":
                    return "http://www.credit-card-logos.com/images/mastercard_credit-card-logos/mastercard_logo_6.gif";
                    break;
                case "Discover":
                    return "http://www.credit-card-logos.com/images/discover_credit-card-logos/discover_network1.jpg";
                    break;
                case "American Express":
                    return "http://www.credit-card-logos.com/images/american_express_credit-card-logos/american_express_logo_9.gif";
                    break;
                case "JCB":
                    return "http://www.jcbeurope.eu/about/emblem_slogan/images/index/logo_img01.jpg";
                    break;
                case "Maestro":
                    return "http://support.worldpay.com/support/images/cardlogos/maestro.gif";
                    break;
                default:
                    return "https://d30y9cdsu7xlg0.cloudfront.net/png/48448-200.png";
                    break;
            }
        }

        function _openModal() {
            var modalInstance = vm.$uibModal.open({
                animation: true,
                templateUrl: "/Assets/Themes/bringpro/js/features/dashboard/templates/creditCardsTabModal.html",
                controller: 'modalController as mc',
            });

            modalInstance.result.then(function () {
                _getCurrentUserCCInfo();
            }, function () {
                console.log('Modal dismissed at: ' + new Date());
            });
        }

        function _makeDefaultCard(nonce) {
            vm.$dashboardService.updateDefaultCard(nonce, _updateDefaultCardSuccess, _updateDefaultCardError)
        }

        function _updateDefaultCardSuccess(data) {
            vm.notify(function () {
                vm.update = data.item;
                console.log("Updated Default Card:", vm.update);
                _getCurrentUserCCInfo();
            });
        }

        function _updateDefaultCardError(jqXhr, error) {
            console.error(error);
            console.log(jqXhr);
        }

        function _openConfirmModal(nonce) {
            var modalInstance = vm.$uibModal.open({
                animation: true,
                templateUrl: 'modalConfirmContent.html',
                controller: 'modalConfirmController as mcc',
                size: 'sm'
            });

            modalInstance.result.then(function () {
                _makeDefaultCard(nonce);
            }, function () {
                console.log('Modal dismissed at: ' + new Date());
            });
        }

        function _openDeleteModal(nonce) {
            var modalInstance = vm.$uibModal.open({
                animation: true,
                templateUrl: 'modalDeleteContent.html',
                controller: 'modalDeleteController as mdc',
                size: 'sm'
            });

            modalInstance.result.then(function () {
                _deleteCard(nonce);
            }, function () {
                console.log('Modal dismissed at: ' + new Date());
            });
        }

    }
})();
