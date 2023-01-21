$(document).ready(function () {
    let footerHeight = $('#footer').outerHeight();

    // Hide And Show Password Input
    // console.log("asf");
    $('input[type="password"]').wrap("<div class='password-container'></div>");
    $('input[type="password"]').after("<i class='fa fa-eye hide-show-password'></i>");
    $("i.hide-show-password").on("click", function () {
        if ($(this).hasClass("fa-eye")) {
            $(this).siblings("input").attr("type", "text");
        }
        if ($(this).hasClass("fa-eye-slash")) {
            $(this).siblings("input").attr("type", "password");
        }
        $(this).toggleClass("fa-eye-slash").toggleClass("fa-eye");
    });

    $(".moved-label").on("focus", function () {
        if ($(this).attr("type").toLowerCase() == "password") {
            $(this).parent().siblings("label").css({
                "top": "-20px",
                "font-size": "16px",
                "color": "#111"
            });
        } else {
            $(this).siblings("label").css({
                "top": "-20px",
                "font-size": "16px",
                "color": "#111"
            });
        }
        //let checkIfNull = $(this).val();
        //console.log(checkIfNull);
        //console.log("Arrived");
    });
    $(".moved-label").on("blur", function () {
        let checkIfNull = $(this).val();
        if (checkIfNull == "" && $(this).attr("type").toLowerCase() == "password") {
            $(this).parent().siblings("label").css({
                "top": "20px",
                "font-size": "20px",
                "color": "#555"
            });
        } else if (checkIfNull == "" && $(this).attr("type").toLowerCase() != "password") {
            $(this).siblings("label").css({
                "top": "20px",
                "font-size": "20px",
                "color": "#555"
            });
        }
        //console.log(checkIfNull);
        //console.log("Arrived");
    });
    // console.log("test");
    $('#Input_UserName').focus();
    $('#account').on('submit', function (e) {
        let username = $("#Input_UserName").val();
        let password = $("#Input_Password").val();
        if (username != "" && password != "") {
            // e.preventDefault();
            let but = $(this).find('[type="submit"]').toggleClass('sending').blur();
            setTimeout(function () {
                but.removeClass('sending').blur();
            }, 4500);
        } else {
            e.preventDefault();
            let but = $(this).find('[type="submit"]').toggleClass('sending').blur();

            setTimeout(function () {
                but.removeClass('sending').blur();
            }, 1500);
        }
    });

    // For Other Information Button
    $('#otherInfo').on('click', function () {
        $(this).find('i').toggleClass('fa-angle-double-down');
        $(this).find('i').toggleClass('fa-angle-double-up');
    });

    // Hide Show Footer
    $('#hideShowFooter').on('click', function () {
        //$('#footer').slideToggle();
        if ($('#footer').hasClass('hidden')) {
            $('#footer').animate({ height: footerHeight }).removeClass('hidden');
            $(this).find('i').addClass('fa-chevron-down').removeClass('fa-chevron-up');
            console.log('has class');
        } else {
            $('#footer').animate({ height: '2px' }).addClass('hidden');
            $(this).find('i').removeClass('fa-chevron-down').addClass('fa-chevron-up');
            console.log('no class');
        }
        //$(this).find('i').toggleClass('fa-chevron-down').toggleClass('fa-chevron-up');
    });
});