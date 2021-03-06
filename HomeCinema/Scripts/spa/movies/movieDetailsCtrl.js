﻿(function (app) {
    'use strict';
    app.controller('movieDetailsCtrl', movieDetailsCtrl);

    movieDetailsCtrl.$inject = ['$scope', '$location', '$routeParams', '$modal', 'apiService', 'notificationService'];

    function movieDetailsCtrl($scope, $location, $routeParams, $modal, apiService, notificationService) {
        $scope.pageClass = 'page-movie';
        $scope.movie = {};
        $scope.loadingMovie = true;
        $scope.loadingRentals = true;
        $scope.isReadOnly = true;
        $scope.openRentDialog = openRentDialog;
        $scope.returnMovie = returnMovie;
        $scope.rentalHistory = [];
        $scope.getStatusColor = getStatusColor();
        $scope.clearSearch = clearSearch;
        $scope.isBorrowed = isBorrowed;

        function loadMovie() {
            $scope.loadingMovie = true;
            var config = {
                params: {
                    id: $routeParams.id
                }
            };
            apiService.get('/api/movies/details/', config, movieLoadCompleted, movieLoadFailed);
        }

        function loadRentalHistory() {
            $scope.loadingRentals = true;
            var config = {
                params: {
                    movieid: $routeParams.id
                }
            };
            apiService.get('/api/rentals/rentalhistory/', config, rentalHistoryLoadCompleted, rentalHistoryLoadFailed);
            //apiService.get('/api/rentals/' + $routeParams.id + '/rentalhistory', null,
            //            rentalHistoryLoadCompleted,
            //            rentalHistoryLoadFailed);
        }

        function loadMovieDetails() {
            loadMovie();
            loadRentalHistory();
        }

        function returnMovie(rentalID) {
            apiService.post('/api/rentals/return/' + rentalID, returnMovieSucceeded, returnMovieFailed);
        }

        function isBorrowed(rental) {
            return rental.Status == 'Borrowed';
        }

        function getStatusColor(status) {
            if (status == 'Borrowed') {
                return 'red';
            } else {
                return 'green';
            }
        }

        function clearSearch() {
            $scope.filterRentals = '';
        }

        function movieLoadCompleted(result) {
            $scope.movie = result.data;
            $scope.loadingMovie = false;
        }

        function movieLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function rentalHistoryLoadCompleted(result) {
            $scope.rentalHistory = result.data;
            $scope.loadingRentals = false;
        }

        function rentalHistoryLoadFailed(response) {
            notificationService.displayError(response);
        }

        function returnMovieSucceeded(response) {
            notificationService.displaySuccess('Movie returned to HomeCinema successfully.');
            loadMovieDetails();
        }

        function returnMovieFailed(response) {
            notificationService.displayError(response.data);
        }

        function openRentDialog() {
            $modal.open({
                templateUrl: 'scripts/spa/rental/rentMovieModal.html',
                controller: 'rentMovieCtrl',
                scope: $scope
            }).result.then(function ($scope) {
                loadMovieDetails();
            }, function () { });
        }

        loadMovieDetails();
    }
})(angular.module('homeCinema'));