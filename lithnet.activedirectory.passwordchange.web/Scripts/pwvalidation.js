$(document).ready(function () {

    $('#new-password-input').keyup(throttle(function () {
        var pswd = $(this).val();
        $('#password-test-info').html('');
        $.ajax({
            url: '/Check/Password',
            data: JSON.stringify({
                username: $('#user-name').val(),
                newPassword: pswd
            }),
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                if (data.valid == true) {
                    $('#password-test-info').text('Strength: Excellent');
                } else if (data.valid == false) {
                    $('#password-test-info').text(data.details);
                }
                
            },
            error: function () {
                $('#password-test-info').text('Error checking password');
            }
        });

        
    }));

});

function throttle(f, delay) {
    var timer = null;
    return function () {
        var context = this, args = arguments;
        clearTimeout(timer);
        timer = window.setTimeout(function () {
            f.apply(context, args);
        },
            delay || 1000);
    };
}