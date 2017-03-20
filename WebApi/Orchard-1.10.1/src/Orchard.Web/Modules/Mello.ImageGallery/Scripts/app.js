'use strict';

// Declare app level module which depends on views, and components
angular.module('myApp', [
  'ngRoute',
  'myApp.view1'
]).
config(['$locationProvider', '$routeProvider', function($locationProvider, $routeProvider) {
  $locationProvider.hashPrefix('!');

  //$stateProvider
  //       .state('app', {
  //           url: '/',
  //           templateUrl: 'view1/view1.html',
  //           controller: 'View1Ctrl'
  //       });

  $routeProvider.otherwise({redirectTo: 'app'});
}]);
