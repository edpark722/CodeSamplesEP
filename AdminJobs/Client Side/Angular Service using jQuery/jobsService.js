(function () {
    "use strict";
    angular.module(APPNAME)
        .factory('$jobService', JobServiceFactory);
    //  manually identify dependencies for injection: https://github.com/johnpapa/angular-styleguide#style-y091
    //  $services is a reference to bringpro.page object. bringpro.page is created in bringpro.js
    JobServiceFactory.$inject = ['$baseService', '$services'];

    function JobServiceFactory($baseService, $services) {
        //  bringpro.page has been injected as $services so we can reference anything that is attached to bringpro.page here
        var aBringProServiceObject = bringpro.services.jobs;

        //  merge the jQuery object with the angular base service to simulate inheritance
        var newJobService = $baseService.merge(true, {}, aBringProServiceObject, $baseService);

        console.log("job service", aBringProServiceObject);

        return newJobService;
    }
})();