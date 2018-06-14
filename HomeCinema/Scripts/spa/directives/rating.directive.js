(function (app) {
    'use strict';

    app.directive('componentRating', componentRating);

    function componentRating() {
        return {
            restrict: 'A',
            link: function ($scope, $element, $attrs) {
                $element.raty({
                    score: $attrs.componentRating,
                    halfShow: false,
                    readOnly: $scope.isReadOnly,
                    noRateMsg: "Not Rated yet!",
                    startHalf: "../Content/images/raty/star-half.png",
                    startOff: "../Content/images/raty/star-off.png",
                    startOn: "../../Content/images/raty/star-on.png",
                    hints: ["Poor", "Average", "Good", "Very Good", "Excellent"],
                    click: function (score, event) {
                        //set the model value
                        $scope.movie.Rating = score;
                        $scope.$apply();
                    }
                })
            }
        }
    }
})(angular.module('common.ui'));