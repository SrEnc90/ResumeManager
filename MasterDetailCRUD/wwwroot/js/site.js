// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// $(".custom-file-input").on("change", function () {
//     let fileName = $(this).val().split("\\").pop();
//     $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
// });

function CalcTotalExperiencies() {
    let x = document.getElementsByClassName('YearsWorked');
    let totalExp = 0;
    let i;

    for (i = 0; i < x.length; i++) {
        totalExp = totalExp + eval(x[i].value);
    }

    document.getElementById('TotalExperiencie').value = totalExp;

    return;
}

document.addEventListener('change', function (e) {
    if (e.target.classList.contains('YearsWorked')) {
        CalcTotalExperiencies();
    }
}, false);

function DeleteItem(btn) {

    let table = document.getElementById("ExpTable");
    let rows = table.getElementsByTagName("tr");

    if (rows.length == 2) {
        alert("This Row Cannot be Deleted");
        return;
    }

    let btnIdx = btn.id.replaceAll('btnRemove-', '');
    let idofIsDeleted = btnIdx + "__IsDeleted";

    let hidIsDelId = document.querySelector(`[id$='${idofIsDeleted}']`).id;

    document.getElementById(hidIsDelId).value = "true";

    //$(btn).closest('tr').remove();
    $(btn).closest('tr').hide();

    CalcTotalExperiencies();
}

function AddItem(btn) {
    var table = document.getElementById("ExpTable")
    var rows = table.querySelectorAll('tr');

    var rowOuterHTML = rows[rows.length - 1].outerHTML;

    // var lastRowIdx = document.getElementById('hdnLastIndex').value;
    //document.getElementById('hdnLastIndex').value = nextRowIdx;
    var lastRowIdx = rows.length - 2;

    var nextRowIdx = eval(lastRowIdx) + 1;

    rowOuterHTML = rowOuterHTML.replaceAll('_' + lastRowIdx + '_', '_' + nextRowIdx + '_');
    rowOuterHTML = rowOuterHTML.replaceAll('[' + lastRowIdx + ']', '[' + nextRowIdx + ']');
     rowOuterHTML = rowOuterHTML.replaceAll('-' + lastRowIdx, '-' + nextRowIdx); //Es para reemplazar en el botón delete

    var newRow = table.insertRow();
    newRow.innerHTML = rowOuterHTML;

    var x = table.getElementsByTagName("INPUT");

    console.log(x.length);

    for (let cnt = 0; cnt < x.length; cnt++) {
        if (x[cnt].type == "text" && x[cnt].id.indexOf(`_${nextRowIdx}_`) > 0) {
            x[cnt].value = '';
        }
        else if (x[cnt].type == "number" && x[cnt].id.indexOf(`_${nextRowIdx}_`) > 0) {
            x[cnt].value = 0;
        }
    }

     //var btnAddId = btn.id;
     //var btnDeleteId = btnAddId.replaceAll('btnAdd', 'btnRemove');

    // var delBtn = document.getElementById(btnDeleteId);
    // delBtn.classList.add("visible");
    // delBtn.classList.remove("invisible");

    // var addBtn = document.getElementById(btnAddId);
    // addBtn.classList.add("invisible");
    // addBtn.classList.remove("visible");

    rebIndValidators();
}

/*
El problema con jquery es que solo los inputs que están disponibles al momento de correr la aplicación van a seguir las validaciones 
(el archivo jquery.validate.unobtrusive.js no funciona con las etiquetas insertadas dinámicamente solo los que están en tiempo de ejecución) 
*/

function rebIndValidators() {

    var $form = $("#ApplicantForm");

    $form.unbind(); //Remueve todos los controles de jqery del formulario

    $form.data("validator", null); //Limpiamos todas la validaciones del formulario

    $.validator.unobtrusive.parse($form); //Añade las validaciones de nuevo

    $form.validate($form.data("unobtrusiveValidation").options);
}