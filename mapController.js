'use strict';

var application = angular.module("main", []);

application.controller('MapController', ['$scope', '$rootScope',
	function ($scope, $rootScope) {
		$scope.message="Harita Test";
	}]);
