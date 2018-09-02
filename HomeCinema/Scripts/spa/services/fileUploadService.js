﻿(function (app) {
    'use strict';

    app.factory('fileUploadService', fileUploadService);
    fileUploadService.$inject = ['$rootScope', '$http', '$timeout', '$upload', 'notificationService'];
    function fileUploadService($rootScope, $http, $timeout, $upload, notificationService) {
        $rootScope.upload = [];
        var service = {
            uploadImage: uploadImage
        }

        function uploadImage($file, movieId, callback) {
            for (var i = 0; i < $file.length; i++) {
                var $file = $file[i];
                (function (index) {
                    $rootScope.upload[index] = $upload.upload({
                        url: 'api/movies/images/upload?movieId=' + movieId,
                        method: 'POST',
                        file: $file
                    }).progress(function (evt) {
                    }).success(function (data, status, headers, config) {
                        notificationService.displaySuccess(data.FileName + " uploaded successfully.");
                        callback();
                    }).error(function (data, status, headers, config) {
                        notificationService.displayError(data.Message);
                    })
                })(i);
            }
        }

        return service;
    }
})(angular.module('common.core'));