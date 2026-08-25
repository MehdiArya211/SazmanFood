$("a.remove").click(function (e) {
    e.preventDefault();

    var url = $(this).attr("href");
    var row = $(this).parents("tr").eq(0);

    swal({
        title: 'آیا مطمئنید ؟',
        text: "پس حذف ، این مورد قابل بازگشت نیست ",
        type: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'بله ، حذف شود !',
        cancelButtonText: 'لغو'
    }).then(function () {
        $.get(url,
            function () {
                row.remove();
                swal(
                    'حذف شد !',
                    'مورد با موفقیت حذف شد',
                    'success'
                );
            });

    });

});