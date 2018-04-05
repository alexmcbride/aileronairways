// Write your JavaScript code.
function disableFormElement(element, disabled) {
    if (disabled) {
        $(element).attr('disabled', 'disabled');
    }
    else {
        $(element).removeAttr("disabled");
    }
}

function displayDeleteModal(data, id, timelineId) {
    $('#myModalContent').html(data);
    $('#myModal').modal({ keyboard: true });
    $('#myModal').modal('show');

    // Hook event to edit view form.
    $('#delete-event-form').on('submit', function (e) {
        e.preventDefault();

        disableFormElement('#btnSave', true);

        var data = $('#delete-event-form').serialize();
        $.post('/timelines/@ViewBag.TimelineId/events/' + id + '/delete', data, function (response) {
            if (response.startsWith('OK')) {
                var eventId = response.split(' ')[1];
                location.href = '/timelines/' + timelineId+'/events';
                $('#myModal').modal('hide');
            }
            else {
                displayDeleteModal(response, id);
            }

            disableFormElement('#btnSave', false);
        }, 'html');
    });
}

function handleDelete() {
    var id = $('#deleteEvent').attr('data-id');
    var timelineId = $('#deleteEvent').attr('data-timeline-id');
    $.ajax({
        type: "GET",
        url: "/Timelines/" + timelineId + "/Events/" + id + "/Delete",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        success: function (data) {
            displayDeleteModal(data, id, timelineId);
        },
        error: function () {
            alert("Dynamic content load failed.");
        }
    });
}