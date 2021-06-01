$(function () {  
  $.getJSON("scripts/donators.json").done(function (donators) {
    console.log(donators.length);
  });
});
