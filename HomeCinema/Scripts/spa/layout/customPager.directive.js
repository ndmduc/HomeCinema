(function (app) {
    'use strict';

    app.directive('customerPager', customerPager);

    function customerPager() {
        return {
            scope: {
                page: '@',
                pagesCount: '@',
                totalCount: '@',
                searchFunc: '&',
                customPath: '@'
            },
            replace: true,
            restrict: 'E',
            templateUrl: '/scripts/spa/layout/pager.html',
            controller: ['$scope', function ($scope) {
                $scope.search = function (i) {
                    if ($scope.searchFunc) {
                        $scope.searchFunc({ page: i });
                    }
                };

                $scope.range = function () {
                    if (!$scope.pagesCount) {
                        return [];
                    }
                    var step = 2;

                }
            }]
        }
    }
})