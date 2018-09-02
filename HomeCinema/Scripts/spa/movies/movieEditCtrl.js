(function (app) {
    'use strict';

    app.controller('movieEditCtrl', movieEditCtrl);

    movieEditCtrl.$inject = ['$scope', '$location', '$routeParams', 'apiService', 'notificationService', 'fileUploadService'];

    function movieEditCtrl($scope, $location, $routeParams, apiService, notificationService, fileUploadService) {
        $scope.pageClass = 'page-movie';
        $scope.movie = {};
        $scope.genres = {};
        $scope.loadingMovie = true;
        $scope.isReadOnly = false;
        $scope.UpdateMovie = UpdateMovie;
        $scope.prepareFiles = prepareFiles;
        $scope.openDatePicker = openDatePicker;

        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 1
        };
        $scope.datepicker = {};

        var movieImage = null;

        function loadMovie() {
            $scope.loadingMovie = true;
            var config = {
                params: {
                    id: $routeParams.id
                }
            };
            apiService.get('/api/movies/details/', config, movieLoadCompleted, movieLoadFailed);
        }

        function movieLoadCompleted(result) {
            $scope.movie = result.data;
            $scope.loadingMovie = false;

            loadGenres();
        }

        function movieLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function loadGenres() {
            apiService.get('/api/genres/get', null, genresLoadCompleted, genresLoadFailed);
        }

        function genresLoadCompleted(response) {
            $scope.genres = response.data;
        }

        function genresLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function UpdateMovie() {
            if (movieImage) {
                fileUploadService.uploadImage(movieImage, $scope.movie.ID, UpdateMovieModel);
            } else {
                UpdateMovieModel();
            }
        }

        function UpdateMovieModel() {
            apiService.post('/api/movies/update', $scope.movie, updateMovieSucceded, updateMovieFailed);
        }

        function prepareFiles($files) {
            movieImage = $files;
        }

        function updateMovieSucceded(response) {
            console.log(response);
            notificationService.displaySuccess($scope.movie.Title + ' has been updated.');
            $scope.movie = response.data;
            movieImage = null;
        }

        function updateMovieFailed(response) {
            notificationService.displayError(response);
        }

        function openDatePicker($event) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope.datepicker.opened = true;
        }

        loadMovie();
    }
})(angular.module('homeCinema'));