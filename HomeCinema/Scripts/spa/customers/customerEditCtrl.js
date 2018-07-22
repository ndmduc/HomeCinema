(function(app){
    'use strict';

    app.controller('customerEditCtrl', customerEditCtrl);

    customerEditCtrl.$inject = ['$scope', '$modalInstance', '$timeout', 'apiService', 'notificationService'];

    function customerEditCtrl($scope, $modalInstance, $timeout, apiService, notificationService){
        $scope.cancelEdit = cancelEdit;
        $scope.updateCustomer = updateCustomer;
        $scope.openDatePicker = openDatePicker;
        $scope.dateOptions ={
            formatYear: 'YY',
            startingDay: 1
        };
        $scope.datepicker =  {};

        function updateCustomer(){
            console.log($scope.EditCustomer);
            apiService.post('/api/customers/update/', $scope.EditCustomer, updateCustomerCompleted, updateCustomerLoadFailed);

        }

        function updateCustomerCompleted(){
            notificationService.displaySuccess($scope.EditCustomer.FirstName + " " + $scope.EditCustomer.LastName + " has been updated");
            $scope.EditCustomer = {};
            $modalInstance.dismiss();   // close the modal popup windows.
        }

        function updateCustomerLoadFailed(response){
            notificationService.displayError(response.data);
        }

        function cancelEdit(){
            $scope.isEnabled = false;
            $modalInstance.dismiss();
        }

        function openDatePicker($event){
            $event.preventDefault();
            $event.stopPropagation();

            $timeout(function(){
                $scope.datepicker.opened = true;
            })
        }
    }
})(angular.module('homeCinema'));