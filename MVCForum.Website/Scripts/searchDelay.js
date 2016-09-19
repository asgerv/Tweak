var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();
$("#keyword").keypress(function () {
    delay(function () {
        $("form").submit();
    },
        700);
});