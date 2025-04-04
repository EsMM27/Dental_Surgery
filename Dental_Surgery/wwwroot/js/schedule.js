document.addEventListener("DOMContentLoaded", function () {
    feather.replace();

    // Make rows clickable unless clicking on attended column
    document.querySelectorAll('.clickable-row').forEach(row => {
        row.addEventListener('click', (event) => {
            let target = event.target;
            while (target && target !== row) {
                if (target.classList.contains('no-click')) return;
                target = target.parentElement;
            }
            if (target === row) {
                window.location = row.getAttribute('data-href');
            }
        });
    });

    // Toggle patient search field
    window.togglePatientSearch = function () {
        const searchContainer = document.querySelector('.patient-search-container');
        const patientHeaderText = document.getElementById('patientHeaderText');
        const searchIcon = document.querySelector('.search-icon');
        const isVisible = searchContainer.style.display === 'block';

        if (isVisible) {
            searchContainer.style.display = 'none';
            patientHeaderText.style.display = '';
            searchIcon.style.display = '';
        } else {
            searchContainer.style.display = 'block';
            patientHeaderText.style.display = 'none';
            searchIcon.style.display = 'none';
            document.getElementById('patientSearch').focus();
        }
    }

    // Live search filter for patient name
    window.searchPatients = function () {
        const searchTerm = document.getElementById('patientSearch').value.toLowerCase();
        const rows = document.querySelectorAll('.appointment-row');

        rows.forEach(row => {
            const patientName = row.getAttribute('data-patient-name').toLowerCase();
            row.style.display = patientName.includes(searchTerm) ? '' : 'none';
        });
    }

    // Close patient search when clicking outside
    document.addEventListener('click', function (event) {
        const searchContainer = document.querySelector('.patient-search-container');
        const searchIcon = document.querySelector('.search-icon');
        const patientHeaderText = document.getElementById('patientHeaderText');

        if (!searchContainer.contains(event.target) && event.target !== searchIcon) {
            searchContainer.style.display = 'none';
            patientHeaderText.style.display = '';
            searchIcon.style.display = '';
        }
    });

    // Attendance checkbox toggle with backend update
    document.addEventListener('click', async function (event) {
        const icon = event.target.closest('.attendance-icon');
        if (!icon) return;

        const appointmentId = parseInt(icon.getAttribute('data-id'));
        const checkbox = document.querySelector(`.attendance-checkbox[data-id="${appointmentId}"]`);
        const isChecked = !checkbox.checked;
        const cell = icon.closest("td");

        icon.setAttribute('data-feather', isChecked ? 'check-square' : 'square');
        cell.classList.toggle('attended-cell', isChecked);
        feather.replace();
        checkbox.checked = isChecked;
        localStorage.setItem(`attendance-${appointmentId}`, JSON.stringify({ attend: isChecked }));

        const response = await fetch('/Admin2/Appointments/UpdateAttendance?handler=Update', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id: appointmentId, attend: isChecked })
        });

        if (!response.ok) {
            alert("Failed to update attendance.");
            icon.setAttribute('data-feather', isChecked ? 'square' : 'check-square');
            feather.replace();
            checkbox.checked = !isChecked;
        }
    });

    // Restore attendance state from localStorage
    document.querySelectorAll('.attendance-icon').forEach(icon => {
        const appointmentId = parseInt(icon.getAttribute('data-id'));
        const storedState = localStorage.getItem(`attendance-${appointmentId}`);
        const cell = icon.closest('td');

        if (storedState) {
            const { attend } = JSON.parse(storedState);
            icon.setAttribute('data-feather', attend ? 'check-square' : 'square');
            cell.classList.toggle('attended-cell', attend);
            feather.replace();
        }
    });
});
