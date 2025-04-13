$(document).ready(function () {
    const currentUrl = window.location.pathname.toLowerCase();
    $(".kotBtns a").each(function () {
        let linkUrl = $(this).attr('href').toLowerCase();

        if (currentUrl.includes(linkUrl)) {
            $(this).addClass('activePageInOrderApp');
        }else{
            $(this).removeClass('activePageInOrderApp');
        }
    });
});