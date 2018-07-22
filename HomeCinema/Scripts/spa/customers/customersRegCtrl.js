(function (app) {
    'use strict';

    app.controller('customersRegCtrl', customersRegCtrl);
    customersRegCtrl.$inject = ['$scope', '$location', '$rootScope', 'apiService'];

    function customersRegCtrl($scope, $location, $rootScope, apiService) {
        $scope.newCustomer = {};
        $scope.Register = register;
        $scope.openDatePicker = openDatePicker;
        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1
        };
        $scope.datepicker = {};

        $scope.submission = {
            successMessages: ['Successfull submission will appear here.'],
            errorMessages: ['Submission errors will appear here.']
        };

        function register() {
            apiService.post('/api/customers/register', $scope.newCustomer, registerCustomerSucceded, registerCustomerFailed);
        }

        function registerCustomerSucceded(response) {
            var customerRegistered = response.data;
            $scope.submission.successMessages = [];
            $scope.submission.successMessages.push($scope.newCustomer.LastName + ' has been successfully registered.');
            $scope.submission.successMessages.push('Check ' + customerRegistered.UniqueKey + ' for reference number.');
            $scope.newCustomer = {};
        }

        function registerCustomerFailed(response) {
            if (response.status == '400') {
                $scope.submission.errorMessages = response.data;
            } else {
                $scope.submission.errorMessages = response.statusText;
            }
        }

        function openDatePicker($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.datepicker.opened = true;
        }
    }
})(angular.module('homeCinema'));