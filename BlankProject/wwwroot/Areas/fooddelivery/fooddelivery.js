var foodDelivery = {
    deliver: {
        save: function (e) {
            e.preventDefault();

            var $form = $(".delivery-form");

            $form.validate();

            if (!$form.valid())
                return false;

            $.post($form.attr("action"), $form.serialize(), function (res) {
                if (res.status) {
                    $("#delivery-result").show();

                    var model = res.model || {};

                    $("#delivery-person").text((model.fName || "") + " " + (model.lName || ""));
                    $("#delivery-food").text("غذا: " + (model.foodTitle || "-"));
                    $("#delivery-token").text("نوع ژتون: " + (model.tokenType || "-"));

                    $("#DeliveryCode").val("").focus();

                    swal({
                        title: "تحویل شد!",
                        text: res.message || "غذا با موفقیت تحویل شد",
                        type: "success",
                        confirmButtonClass: "btn btn-success",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });
                }
                else {
                    $("#delivery-result").hide();

                    swal({
                        title: "خطا!",
                        text: res.message || "تحویل غذا با خطا همراه بوده است",
                        type: "error",
                        confirmButtonClass: "btn btn-danger",
                        confirmButtonText: "باشه",
                        buttonsStyling: false
                    });

                    $("#DeliveryCode").select().focus();
                }
            }).fail(function () {
                swal({
                    title: "خطا!",
                    text: "تحویل غذا با خطا همراه بوده است",
                    type: "error",
                    confirmButtonClass: "btn btn-danger",
                    confirmButtonText: "باشه",
                    buttonsStyling: false
                });
            });

            return false;
        }
    }
};