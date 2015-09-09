(function () {
    var app = angular.module('app', []);

    app.config(['$httpProvider', function ($httpProvider) {
        $httpProvider.defaults.useXDomain = true;
        $httpProvider.defaults.withCredentials = true;
        delete $httpProvider.defaults.headers.common["X-Requested-With"];
        $httpProvider.defaults.headers.common["Accept"] = "application/json";
        $httpProvider.defaults.headers.common["Content-Type"] = "application/json";
    }]);

    app.factory('eFormService', ['$http', function ($http) {
        //var form = this;
        //form.data = {};
        //var eFormService = {};

        var getApplication = function () {
            return $http.get('/Applications/GetApplication');
        };

        var updateApplication = function (application) {
            return $http.post("/Payment/Purchase", application)
        };

        //var setData = function (data) {
        //    form.data = data;
        //};

        //var getData = function (tabNum) {
        //    return form.data;
        //};

        return {
            getApplication: getApplication,
            updateApplication: updateApplication
            //getData: getData,
            //setData: setData
        }
    }]);

    app.controller('ApplicationController', ['eFormService', '$http',
            function (eFormService, $http) {

                var vm = this;
                vm.form = {};

                clearForm();

                function clearForm() {
                    vm.form = {
                        Username: '',
                        OrderNumber: '',
                        PaymentReference: '',
                        SubTotal: 0.00,
                        Shipping: 0.00,
                        Tax: 0.00,
                        Fees: 0.00
                    }
                }

                this.submitForm = function () {
                    eFormService.updateApplication(vm.form)
                        .success(function (studs) {
                            alert("success!");
                        })
                        .error(function (error) {
                            alert('Unable to save customer data: ' + error.message);
                        });
                };

            }]);



})();
