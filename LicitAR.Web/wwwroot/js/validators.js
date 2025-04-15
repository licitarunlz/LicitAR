$(function () { 
    if ($.validator && $.validator.unobtrusive && $.validator.unobtrusive.adapters) {
        $.validator.addMethod("cuit", function (value, element) {
            if (!value) return true;
            value = value.replace(/-/g, '');

            if (!/^\d{11}$/.test(value)) return false;

            let mult = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2];
            let total = 0;

            for (let i = 0; i < 10; i++) {
                total += parseInt(value[i]) * mult[i];
            }

            let mod = total % 11;
            let checkDigit = mod === 0 ? 0 : mod === 1 ? 9 : 11 - mod;

            return checkDigit === parseInt(value[10]);
        });

        $.validator.unobtrusive.adapters.addBool("cuit");
    }
});