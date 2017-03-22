(function () {
    "use strict";

    angular.module(APPNAME)
        .controller('modalController', ModalController);

    //  $uibModalInstance is coming from the UI Bootstrap library and is a reference to the modal window itself so we can work with it
    ModalController.$inject = ['$scope', '$baseController', '$uibModalInstance', '$dashboardService'];

    function ModalController(
        $scope
        , $baseController
        , $uibModalInstance
        , $dashboardService) {

        var vm = this;

        $baseController.merge(vm, $baseController);

        vm.$scope = $scope;
        vm.$uibModalInstance = $uibModalInstance;
        vm.$dashboardService = $dashboardService;

        vm.submitCard = _submitCard;

        vm.paymentInfo = {};
        vm.resultId = null;
        vm.paymentInfo.defaultCard = true;

        vm.clientToken = $("#clientCCToken").val();

        //this is a wrapper for our small dependency on $scope
        vm.notify = vm.$dashboardService.getNotifier($scope);

        render();

        function render() {
            braintree.client.create({
                authorization: vm.clientToken
            },
            _clientInstance);
        }

        function _clientInstance(err, clientInstance) {
            if (err) {
                console.error(err);
                return;
            }
            braintree.hostedFields.create({
                client: clientInstance,
                styles: {
                    'input': {
                        'font-size': '12px',
                        'font-family': 'helvetica, tahoma, calibri, sans-serif',
                        'color': '#3a3a3a'
                    },
                    ':focus': {
                        'color': 'black'
                    },
                    'input.invalid': {
                        'color': 'red'
                    },
                    'input.valid': {
                        'color': 'green'
                    }
                },
                fields: {
                    number: {
                        selector: '#card-number',
                        placeholder: '4111 1111 1111 1111'
                    },
                    cvv: {
                        selector: '#cvv',
                        placeholder: '123'
                    },
                    expirationMonth: {
                        selector: '#expiration-month',
                        placeholder: 'MM'
                    },
                    expirationYear: {
                        selector: '#expiration-year',
                        placeholder: 'YY'
                    },
                    postalCode: {
                        selector: '#postal-code',
                        placeholder: '90210'
                    }
                }
            }, _hostedFieldsInstance);
        }

        function _hostedFieldsInstance(err, hostedFieldsInstance) {
            if (err) {
                console.error(err);
                return;
            }

            //New Validation for EACH Field
            hostedFieldsInstance.on('validityChange', _onValidityChange);
            hostedFieldsInstance.on('cardTypeChange', _onCardTypeChange);

            vm.hostedFieldsInstance = hostedFieldsInstance;


        }

        function _onCardTypeChange(event) {
            // Handle a field's change, such as a change in validity or credit card type
            if (event.cards.length === 1) {
                $('#card-type').text(event.cards[0].niceType);
            } else {
                $('#card-type').text('Card');
            }
        }

        function _onValidityChange(event) {
            var field = event.fields[event.emittedBy];

            if (field.isValid) {
                if (event.emittedBy === 'expirationMonth') {
                    $('#expiration-month').next('small').text('');
                }
                else if (event.emittedBy === 'expirationYear') {
                    $('#expiration-year').next('small').text('');
                }
                else if (event.emittedBy === 'number') {
                    $('#card-number').next('small').text('');
                }
                else if (event.emittedBy === 'postalCode') {
                    $('#postal-code').next('small').text('');
                }
                else if (event.emittedBy === 'cvv') {
                    $('#cvv').next('small').text('');
                }

                // Apply styling for a valid field
                $(field.container).parents('.form-group').addClass('has-success');
            } else if (field.isPotentiallyValid) {
                // Remove styling  from potentially valid fields
                $(field.container).parents('.form-group').removeClass('has-warning');
                $(field.container).parents('.form-group').removeClass('has-success');
                if (event.emittedBy === 'number') {
                    $('#card-number').next('small').text('');
                }
                if (event.emittedBy === 'postalCode') {
                    $('#postal-code').next('small').text('');
                }
                if (event.emittedBy === 'cvv') {
                    $('#cvv').next('small').text('');
                }
                if (event.emittedBy === 'expirationMonth') {
                    $('#expiration-month').next('small').text('');
                }
                if (event.emittedBy === 'expirationYear') {
                    $('#expiration-year').next('small').text('');
                }
            } else {
                // Add styling to invalid fields
                $(field.container).parents('.form-group').addClass('has-warning');

                if (event.emittedBy === 'postalCode') {
                    $('#postal-code').next('small').text('Check Postal Code');
                }
                if (event.emittedBy === 'cvv') {
                    $('#cvv').next('small').text('Check CVV');
                }
                if (event.emittedBy === 'expirationMonth') {
                    $('#expiration-month').next('small').text('Check Month');
                }
                if (event.emittedBy === 'expirationYear') {
                    $('#expiration-year').next('small').text('Check Year');
                }
                // Add helper text for an invalid card number
                if (event.emittedBy === 'number') {
                    $('#card-number').next('small').text('Check Card Number');
                }
            }
        }

        function _tokenize(err, payload) {
            if (err) {
                console.error(err);
                return;
            }

            // This is where you would submit payload.nonce to your server
            console.log('Got a nonce: ' + payload.nonce);
            console.log(typeof payload.nonce);
            console.log('Got last Two: ' + payload.details.lastTwo);
            console.log('Got card type: ' + payload.details.cardType)
            //put these ng-models into the modal as hidden div
            vm.paymentInfo.externalCardIdNonce = payload.nonce;
            vm.paymentInfo.last4DigitsCC = payload.details.lastTwo;
            vm.paymentInfo.cardType = payload.details.cardType;



            //synchronize to Insert
            __insertCurrentUserCCInfo();


        }

        function _submitCard() {
            vm.hostedFieldsInstance.tokenize(_tokenize);
        }

        function __insertCurrentUserCCInfo() {
            vm.$dashboardService.insertCurrentUserCCInfo(vm.paymentInfo, _insertCurrentUserCCInfoSuccess, _insertCurrentUserCCInfoError)

        }

        function _insertCurrentUserCCInfoSuccess(data) {
            vm.notify(function () {
                vm.resultId = data.item;
                console.log("Insert Result Id:", vm.resultId);
                vm.$alertService.success("Credit Card saved successfully!", "Save Successful!");
                vm.$uibModalInstance.close();
            });
        }

        function _insertCurrentUserCCInfoError(jqXhr, error) {
            console.error(error);
            console.log(jqXhr);
            vm.$alertService.error(jqXhr.responseText, "Save Unsuccessful!");
            vm.$uibModalInstance.close();
        }



        //  $uibModalInstance is used to communicate and send data back to main controller
        vm.ok = function () {
            vm.$uibModalInstance.close(vm.selected.item);
        };

        vm.cancel = function () {
            vm.$uibModalInstance.dismiss('cancel');
        };
    }
})();


