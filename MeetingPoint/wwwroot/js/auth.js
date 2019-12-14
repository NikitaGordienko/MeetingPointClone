$(document).ready(function () {

    $('.form_auth input[type=text], .form_auth input[type=password]').focus(function () {
        $(this).addClass('focus');
    });
    $('.form_auth input[type=text], .form_auth input[type=password]').focusout(function () {
        var inputVal = $.trim($(this).val());
        if (inputVal.length > 0) {
            $(this).addClass('focus');
        }
        else {
            $(this).removeClass('focus');
        }
    });
    //можно через toggle?
    $('.auth_checkbox').click(function () {
        if ($('input[type=checkbox]').prop('checked') == false) {
            $(this).addClass('active');
            $('.check_l, .check_r').addClass('active');
            $('input[type=checkbox]').prop('checked', true);
        }
        else {
            $(this).removeClass('active');
            $('.check_l, .check_r').removeClass('active');
            $('input[type=checkbox]').prop('checked', false);
        }
    });



    /* Выбор автара из предложенных */

    $('#avatar_id').prop("value", $('.avatar_registration_min:first').prop("src"));
        // Выбор первого аватара по умолчанию
        $('.avatar_registration_min:first').toggleClass('focus');

        //
        $('.avatar_registration_min').click(function () {
            // Визуальное выделение выбранного аватара
            $('.avatar_registration_min').removeClass('focus');
            $(this).toggleClass('focus');

            // Получение уникального id аватара и передача его в тег input type="hidden"
            var src = $(this).prop("src");
            $('.avatar_id_hidden').prop("value", src);
        });

    /* Выбор автара из предложенных */

});
