(function () {
    "use strict";
    angular.module(APPNAME)
        .factory('$billingService', BillingServiceFactory);
    //  manually identify dependencies for injection: https://github.com/johnpapa/angular-styleguide#style-y091
    //  $services is a reference to bringpro.page object. bringpro.page is created in bringpro.js
    BillingServiceFactory.$inject = ['$baseService', '$services'];

    function BillingServiceFactory($baseService, $services) {
        //  bringpro.page has been injected as $services so we can reference anything that is attached to bringpro.page here
        var aBringProServiceObject = bringpro.services.billing;

        //  merge the jQuery object with the angular base service to simulate inheritance
        var newBillingService = $baseService.merge(true, {}, aBringProServiceObject, $baseService);

        console.log("billing service", aBringProServiceObject);

        return newBillingService;
    }
})();