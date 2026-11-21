// Form
var form = document.getElementById("form");

// Buttons
var btnAdd = document.getElementById("btnAdd");
var btnEdit = document.getElementById("btnEdit");
var btnDelete = document.getElementById("btnDelete");
var btnRefresh = document.getElementById("btnRefresh");
var btnDeleteConfirm = document.getElementById("btnDeleteConfirm");

// Inputs
var id = document.getElementById("id");
var firstName = document.getElementById("firstName");
var lastName = document.getElementById("lastName");
var email = document.getElementById("email");

// Validation Messages
var firstNameValidationMessage = document.getElementById("firstNameValidationMessage");
var lastNameValidationMessage = document.getElementById("lastNameValidationMessage");
var emailValidationMessage = document.getElementById("emailValidationMessage");

// Delete Modal
var deleteConfirmModal = document.getElementById("deleteConfirmModal");
var inDeleteConfirmModalId = document.getElementById("inDeleteConfirmModalId");
var deleteConfirmModalBody = document.getElementById("deleteConfirmModalBody");

// Details Modal
var detailsModal = document.getElementById("detailsModal");

// Others
var chkSelectAll = document.getElementById("selectAll");
var tbody = document.querySelector("tbody");
var resultMessage = document.getElementById("resultMessage");

// State
var selectedRows = [];
var allRowsCount = 0;

const API_BASE = "http://localhost:5209";

// Event Listeners
form.addEventListener("submit", Add);
btnRefresh.addEventListener("click", LoadData);
chkSelectAll.addEventListener("click", ToggleSelectAll);
btnEdit.addEventListener("click", Edit); // <--- برای دکمه ادیت
btnDeleteConfirm.addEventListener("click", Delete); 

deleteConfirmModal.addEventListener('show.bs.modal', function (event) {
    var button = event.relatedTarget;
    var rowId = button.getAttribute('data-id');
    inDeleteConfirmModalId.value = rowId;
});

window.addEventListener("load", LoadData);

// ---------------- Load ----------------
function LoadData() {
    RefreshForm();
    chkSelectAll.checked = false;
    selectedRows = [];
    btnAdd.disabled = false; 
    btnEdit.disabled = true; 
    btnDelete.disabled = true; 
    tbody.innerHTML = "";

    fetch(`${API_BASE}/Person/GetAll`)
        .then((res) => {
            if (res.status === 404) {
                allRowsCount = 0;
                return [];
            }
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
            return res.json();
        })
        .then((dto) => {
            let html = "";
            dto.forEach(function (item) {
                html += `<tr id="${item.id}">
                    <td><input class="form-check-input selectRow" type="checkbox" name="${item.id}" onClick="SelectRow(this);"></td>
                    <td class="fntd">${item.firstName}</td>
                    <td class="lntd">${item.lastName}</td>
                    <td class="etd">${item.email}</td>
                    <td>
                        <input class="btn btn-outline-primary btn-sm" type="button" value="Details"
                               data-bs-toggle="modal" data-bs-target="#detailsModal">
                        <input class="btn btn-outline-danger btn-sm" type="button" value="Delete"
                               data-id="${item.id}" data-bs-toggle="modal" data-bs-target="#deleteConfirmModal">
                    </td>
                </tr>`;
            });
            tbody.innerHTML = html;
            allRowsCount = dto.length;
        })
        .catch((err) => {
            console.error(err);
            TriggerResultMessage("Failed to load data");
        });
}

// ---------------- Add ----------------
function Add(e) {
    e.preventDefault();

    if (!ValidateFormData()) return;

    let dto = {
        FirstName: firstName.value,
        LastName: lastName.value,
        Email: email.value
    };

    fetch(`${API_BASE}/Person/Post`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Accept: "*/*"
        },
        body: JSON.stringify(dto)
    }).then((res) => {
        if (res.status === 409) {
            emailValidationMessage.innerText = `Person with Email : ${email.value} already exist`;
            email.classList.add("is-invalid");
        } else if (res.ok) {
            TriggerResultMessage("Operation Successful");
            LoadData();
        } else {
            TriggerResultMessage("Operation Failed");
        }
    }).catch((err) => {
        console.error(err);
        TriggerResultMessage("Operation Failed");
    });
}

// ---------------- Edit ----------------
function Edit() {

    if (selectedRows.length !== 1) return;


    if (!ValidateFormData()) return;

    let updateDto = {
        Id: id.value,
        FirstName: firstName.value,
        LastName: lastName.value,
        Email: email.value
    };

    fetch(`${API_BASE}/Person/Put`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            Accept: "*/*"
        },
        body: JSON.stringify(updateDto)
    }).then((res) => {
        if (res.ok) {
            TriggerResultMessage("Edit Successful");
            LoadData();
        } else {
            TriggerResultMessage("Edit Failed");
        }
    }).catch((err) => {
        console.error(err);
        TriggerResultMessage("Edit Failed");
    });
}

// ---------------- Delete ----------------
function Delete() {
    let idToDelete = inDeleteConfirmModalId.value;

    if (!idToDelete) return;

    fetch(`${API_BASE}/Person/Delete`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
            Accept: "*/*"
        },
        body: JSON.stringify({ Id: idToDelete })
    }).then((res) => {
        if (res.ok) {
            TriggerResultMessage("Delete Successful");
            LoadData();
        } else {
            TriggerResultMessage("Delete Failed");
        }
    }).catch((err) => {
        console.error(err);
        TriggerResultMessage("Delete Failed");
    });
}

// ---------------- Select / Deselect ----------------
function SelectRow(checkBox) {
    if (checkBox.checked) {
        selectedRows.push(checkBox.name);
    } else {
        let idx = selectedRows.indexOf(checkBox.name);
        if (idx > -1) selectedRows.splice(idx, 1);
    }

    chkSelectAll.checked = (allRowsCount > 0 && selectedRows.length === allRowsCount);

    btnDelete.disabled = selectedRows.length === 0;
    btnAdd.disabled = selectedRows.length > 0;
    btnEdit.disabled = selectedRows.length !== 1;

    if (selectedRows.length === 1) {
        let row = document.getElementById(selectedRows[0]);
        id.value = selectedRows[0];
        firstName.value = row.querySelector(".fntd").innerText;
        lastName.value = row.querySelector(".lntd").innerText;
        email.value = row.querySelector(".etd").innerText;
    } else {
        RefreshForm(); 
    }
}

function ToggleSelectAll() {
    document.querySelectorAll(".selectRow").forEach((cb) => {
        cb.checked = chkSelectAll.checked;
        let idx = selectedRows.indexOf(cb.name);
        if (chkSelectAll.checked && idx === -1) selectedRows.push(cb.name);
        if (!chkSelectAll.checked && idx > -1) selectedRows.splice(idx, 1);
    });

    btnDelete.disabled = selectedRows.length === 0;
    btnAdd.disabled = selectedRows.length > 0;
    btnEdit.disabled = selectedRows.length !== 1;

    if (selectedRows.length === 1) {
        let row = document.getElementById(selectedRows[0]);
        id.value = selectedRows[0];
        firstName.value = row.querySelector(".fntd").innerText;
        lastName.value = row.querySelector(".lntd").innerText;
        email.value = row.querySelector(".etd").innerText;
    } else {
        RefreshForm();
    }
}

function DeselectAll() {
    document.querySelectorAll(".selectRow").forEach((cb) => { cb.checked = false; });
    chkSelectAll.checked = false;
    selectedRows = [];
    RefreshForm(); 
}

// ---------------- Form helpers ----------------
function RefreshForm() {
    firstName.classList.remove("is-invalid", "is-valid");
    lastName.classList.remove("is-invalid", "is-valid");
    email.classList.remove("is-invalid", "is-valid");

    id.value = ""; 
    firstName.value = "";
    lastName.value = "";
    email.value = "";

    firstNameValidationMessage.innerText = "";
    lastNameValidationMessage.innerText = "";
    emailValidationMessage.innerText = "";
}

function ValidateFormData() {
    let isValidData = true;

    if (firstName.value.trim() === "") {
        firstNameValidationMessage.innerText = "First name is required";
        firstName.classList.add("is-invalid");
        isValidData = false;
    } else {
        firstName.classList.add("is-valid");
    }

    if (lastName.value.trim() === "") {
        lastNameValidationMessage.innerText = "Last name is required";
        lastName.classList.add("is-invalid");
        isValidData = false;
    } else {
        lastName.classList.add("is-valid");
    }

    const emailRegex = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    if (!emailRegex.test(email.value)) {
        emailValidationMessage.innerText = "Wrong email";
        email.classList.add("is-invalid");
        isValidData = false;
    } else {
        email.classList.add("is-valid");
    }

    return isValidData;
}

function TriggerResultMessage(message) {
    resultMessage.innerText = message;
    resultMessage.style.opacity = "1";
    setTimeout(function () {
        resultMessage.style.opacity = "0";
    }, 2000);
}