(function () {

    var newPasswordField = document.getElementById("new-password")

    newPasswordField.addEventListener("input", function () {
        onPasswordChange(newPasswordField, newPasswordField.value)
    })

    var errorList = document.getElementById("error-list")

    var submitButton = document.getElementById("submit")

    function onPasswordChange(field, value) {
        errorList.innerHTML = ''

        var valid = true
        if (value.length < 8) {
            add_error_item("Requires length of 8")
            valid = false
        }

        if (!/[0-9]/.test(value)) {
            add_error_item("Requires at least one digit (0-9)")
            valid = false
        }

        if (!/[a-z]/.test(value)) {
            add_error_item("Requires at least lower-case character (a-z)")
            valid = false
        }

        if (!/[A-Z]/.test(value)) {
            add_error_item("Requires at least one upper-case character (A-Z)")
            valid = false
        }

        if (valid) {
            submitButton.disabled = false
            submitButton.classList.remove("disabled-button")
            submitButton.classList.add("enabled-button")
        } else {
            submitButton.disabled = true
            submitButton.classList.remove("enabled-button")
            submitButton.classList.add("disabled-button")
        }
    }

    function add_error_item(text) {
        var item = document.createElement("li")
        item.innerHTML = text
        item.classList.add("error-item")

        errorList.appendChild(item)
    }
}());