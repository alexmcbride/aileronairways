﻿
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

function handleDelete(linkElement) {
    var id = linkElement.attr('data-id');
    var timelineId = linkElement.attr('data-timeline-id');
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

function loadFlashPartial() {
    $.get('/TimelineEvents/FlashMessages', function (data) {
        if (data) {
            $('#flash-messages-wrapper').css({ 'position': 'fixed' });
            $('#flash-messages-wrapper').html(data);
        }
    });
}

function handleApiOffline(callback) {
    console.log('Error: IdeaGen API is offline...');

    var html = '<div class="alert alert-danger alert-dismissible text-center alert-flash" role="alert">' +
        '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
        '<strong class="text-capitalize"><i class="fas fa-power-off"></i> ' +
        'API offline</strong> The site has been set to read-only mode' +
        '</div>';

    $('.flash-messages').html(html);

    callback();
}

function checkApiOffline(callback) {
    $.ajax({
        url: '/api/timelines/offline',
        dataType: 'json',
        success: function (data, status, xhr) {
            if (data.offline) {
                handleApiOffline(callback);
            }
        },
        fail: function () {
            // Call anyway if error
            handleApiOffline(callback);
        }
    });
}