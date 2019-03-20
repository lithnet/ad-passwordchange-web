$(document).ready(function () {
    $("input:text").focus(function () { $(this).select(); });
    $("input:password").focus(function () { $(this).select(); });
});