$(document).ready(function () {

    $('#new-password-input').keyup(function () {
        var pswd = $(this).val();

        //validate the length
        if (pswd.length < 8) {
            $('#validator-length').removeClass('valid').addClass('invalid');
        } else {
            $('#validator-length').removeClass('invalid').addClass('valid');
        }

        //validate letter
        if (pswd.match(/[A-z]/)) {
            $('#validator-letter').removeClass('invalid').addClass('valid');
        } else {
            $('#validator-letter').removeClass('valid').addClass('invalid');
        }

        //validate capital letter
        if (pswd.match(/[A-Z]/)) {
            $('#validator-capital').removeClass('invalid').addClass('valid');
        } else {
            $('#validator-capital').removeClass('valid').addClass('invalid');
        }

        //validate number
        if (pswd.match(/\d/)) {
            $('#validator-number').removeClass('invalid').addClass('valid');
        } else {
            $('#validator-number').removeClass('valid').addClass('invalid');
        }

        //validate space
        if (pswd.match(/[^a-zA-Z0-9\-\/]/)) {
            $('#validator-space').removeClass('invalid').addClass('valid');
        } else {
            $('#validator-space').removeClass('valid').addClass('invalid');
        }

    }).focus(function () {
        $('#password-helpinfo').show();
    }).blur(function () {
        $('#password-helpinfo').hide();
    });

});