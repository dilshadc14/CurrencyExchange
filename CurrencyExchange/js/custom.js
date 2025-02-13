document.addEventListener('DOMContentLoaded', function () {
    const dateInputs = document.querySelectorAll('input[type="date"]');
    console.log('invoked');
    dateInputs.forEach(input => {
        input.type = 'text';
        input.addEventListener('focus', () => {
            input.type = 'date';
        });
    });
});