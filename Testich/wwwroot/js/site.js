// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.



function deleteLevel(form) {
    $('[data-toggle="tooltip"]').tooltip('hide');
    $(form).parents('div').remove('.card');
}

function deleteQuestion(form, id) {
    $('[data-toggle="tooltip"]').tooltip('hide');
    $(form).parents('div').remove('.card-title');
    $('#' + id).remove();
}

function deleteCQO(form) {
    $('[data-toggle="tooltip"]').tooltip('hide');
    $(form).parents('li').remove();
}

$(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

// ===== Scroll to Top ==== 
$(window).scroll(function () {
	if ($(this).scrollTop() >= 100) {        // If page is scrolled more than 50px
        $('#return-to-top').fadeIn(200);    // Fade in the arrow
        $('#return-to-down').fadeOut(200);    // Fade in the arrow
	} else {
        $('#return-to-top').fadeOut(200);   // Else fade out the arrow
        $('#return-to-down').fadeIn(200);   
	}
});

$('#return-to-top').click(function () {      // When arrow is clicked
	$('body,html').animate({
		scrollTop: 0                       // Scroll to top of body
	}, 500);
});

$('#return-to-down').click(function () {      // When arrow is clicked
	$('body,html').animate({
		scrollTop: $("#end").offset().top                       // Scroll to top of body
	}, 500);
});

// Add the following code if you want the name of the file appear on select
$(".custom-file-input").on("change", function () {
	var fileName = $(this).val().split("\\").pop();
	$(this).siblings(".custom-file-label").addClass("selected").html(fileName);
});


//var password = document.getElementById("Password")
//    , confirm_password = document.getElementById("PasswordConfirm");

//function validatePassword() {
//    if (password.value != confirm_password.value) {
//        confirm_password.setCustomValidity("Пароли не совпадают");
//    } else {
//        confirm_password.setCustomValidity('');
//    }
//}

//password.onchange = validatePassword;
//confirm_password.onkeyup = validatePassword;