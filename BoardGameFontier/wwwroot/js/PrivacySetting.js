// 給所有Setting中的Checkbox通用
$(document).on('change', '.privacy-toggle', function () {
    const $input = $(this);

    const payload = {
        __RequestVerificationToken: this.form.elements['__RequestVerificationToken'].value,
        field: this.name,     // 例如 IsCityHide
        value: this.checked   // true/false
    };

    // 避免連點
    $input.prop('disabled', true);
    //$.post(網址, 要送的資料, 成功回應時的函式, 回應資料型態'json')
    $.post('/Member/UpdatePrivacy', payload, function (result) {
        // 這裡接收 JSON(result)後端回傳回來的
        if (!result || result.success !== true) {
            // 失敗就把開關復原一下
            $input.prop('checked', !$input.prop('checked'));//(把前面的,改成後面的)
            alert(result?.message || '更新失敗');
        }
    }, 'json')
        .fail(function () {
            // 連線錯誤：也復原
            $input.prop('checked', !$input.prop('checked'));
            alert('連線失敗');
        })
        .always(function () {
            $input.prop('disabled', false);
        });
});