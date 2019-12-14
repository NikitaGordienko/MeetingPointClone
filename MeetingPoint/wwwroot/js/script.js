$(document).ready(function () {

    $(".owl-carousel").owlCarousel({
        loop: true,
        margin: 50,
        nav: true,
        navText: ["<div class='slider_arrow left'>", "<div class='slider_arrow right'>"],
        dots: true,
        items: 1,
    });


    $('.room_item .room_item_circle').click(function () {
        $('.room_item').each(function (i) {
            if ($(this).hasClass('active')) {
                $(this).removeClass('active');
            };
        });
        $('.room').each(function (j) {
            if ($(this).hasClass('active')) {
                $(this).removeClass('active');
            };
        });
        var roomID = $(this).parents('.room_item').data('room');
        $(this).parents('.room_item').toggleClass('active');
        $('#' + roomID).addClass('active');
        ShowCurrentWeekDates();
        DisableWeekButtons();
        // TODO: Добавить lazy-загрузку карты для комнаты
        //LoadRoomMap();
    });


    /* Работа с картами */

    // Подгруженные карты
    var roomMaps = [];

    function SendMarkerState(model) {
        $.ajax(
            {
                url: '/Room/AddMapMarker',
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(model)
            }
        ).done(function (result) {

        }).fail(function (a, b, c) {
            console.log(a);
            console.log(b);
            console.log(c);
        });
    }

    function LoadRoomMap() {

        // Выбор div'a для карты у активной комнаты
        var roomMapId = $('.room.active').find(".room_map").attr("id");

        DG.then(function () {
            // Проверка на наличие загруженной карты
            if (!$('.room.active').find(".room_map").hasClass('leaflet-container')) {

                // Создание карты в выбранном div'е
                var map = DG.map(roomMapId, {
                    center: [55.75, 37.61],
                    zoom: 11
                });

                var marker;
                // Загрузка маркеров из базы данных
                var UserMarkers;

                var GetMapMarkerModel = { RoomId: roomMapId.replace('room_map_', '') };

                $.ajax(
                    {
                        url: '/Room/GetMapMarker',
                        type: "POST",
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify(GetMapMarkerModel)
                    }
                ).done(function (result) {
                    UserMarkers = result;

                    if (UserMarkers != null) {
                        for (var i = 0; i < UserMarkers.length; i++) {


                            if ($('.user-profile-mini:first').text() == UserMarkers[i].userName) {
                                marker = DG.marker([UserMarkers[i].latitude, UserMarkers[i].longitude], {
                                    draggable: true
                                }).addTo(map);
                                marker.bindLabel(UserMarkers[i].userName, { static: true });

                                // Обновление информации в базе после перетаскивания маркера
                                marker.on('dragend', function (e) {
                                    var ltlg = marker.getLatLng();
                                    var MapMarkerModel = {
                                        RoomId: roomMapId.replace('room_map_', ''),
                                        Latitude: ltlg.lat,
                                        Longitude: ltlg.lng
                                    };
                                    SendMarkerState(MapMarkerModel);
                                })
                            }
                            else {
                                var markerLoad = DG.marker([UserMarkers[i].latitude, UserMarkers[i].longitude], {
                                }).addTo(map);
                                markerLoad.bindLabel(UserMarkers[i].userName, { static: true });
                            }
                        }

                    }
                }).fail(function (a, b, c) {
                    console.log(c);
                });





                // Создание маркеров на клик и сохранение информации в базу данных

                map.on('click', function (eventData) {
                    var userName = $('.user-profile-mini:first').text();
                    var userNameContains = + userName + '"';
                    var mapMarkerUserNameLabel = $("#" + roomMapId).find('.leaflet-map-pane').find('.dg-label').find(".dg-label__content:contains(" + userName + ")").text();



                    if (!(userName = mapMarkerUserNameLabel)) {

                        marker = DG.marker([eventData.latlng.lat, eventData.latlng.lng], {
                            draggable: true
                        }).addTo(map);
                        marker.bindLabel($('.user-profile-mini:first').text(), { static: true });

                        // TODO: Отправить состояние маркера на базу

                        var MapMarkerModel = {
                            RoomId: roomMapId.replace('room_map_', ''),
                            Latitude: eventData.latlng.lat,
                            Longitude: eventData.latlng.lng
                        }

                        SendMarkerState(MapMarkerModel);

                    }

                });







                var map_object = { roomMapId, map };

                roomMaps.push(map_object);
            }

        });

    }

    /* /Работа с картами */


    // Вызываем функции при первой загрузке страницы (порядок важен!!)
    CalcLobbyRoomNameWidth();
    CalcLobbyMemberIcons();


    function CalcLobbyRoomNameWidth() {
        $('.lobby_room_name').each(function (j) {
            var maxWidth = (($(document).width() - 310) / 4); // -310 / 4 при 4fr
            $(this).css('max-width', maxWidth);
        });
    }

    function CalcLobbyMemberIcons() {
        $('.lobby_room_member').each(function (i) {
            var width = (($(this).parents('.lobby_room').width() - 110) / 4);
            $(this).height(width);
        });
    }

    $(window).resize(function () {
        CalcLobbyRoomNameWidth();
        CalcLobbyMemberIcons();
    });

    $('.lobby_room_name').each(function (i) {
        var lobbyRoomName = $(this).text();
        $(this).attr('title', lobbyRoomName);
    });

    function CopyInviteLink() {
        $('input.invite_link').select();
        document.execCommand("Copy");
        $('.invite_link_copy img').attr('src', '/images/checkmark.png');
        $('.invite_link_copy img').css('padding-top', '2px');
    }

    $('.invite_link').click(function () {
        CopyInviteLink();
    });

    $('.invite_link_copy').click(function () {
        CopyInviteLink();
    });

    $('.invite_button').click(function () {
        var parent = $(this).parent();
        var inviteLink = parent.find('.invite_link_wrap')
        if (inviteLink.css('display') == 'none') {
            inviteLink.slideDown(150);
        }
        else {
            inviteLink.slideUp(150);
        }
    });


    // Вызов функций при загрузке страницы
    ShowCurrentWeekDates();
    DisableWeekButtons();
    ShowActiveTabName();
    //LoadRoomMap();

    function ShowCurrentWeekDates() {
        var activeRoom = $('.room.active');
        var currentWeek = activeRoom.find('.calendar_week.active');
        var currentWeekDates = currentWeek.data('weekdates');
        var calendarRoom = currentWeek.parents('.room_calendar');
        calendarRoom.find('.calendar_week_current').text(currentWeekDates);
    }

    // Функция отключения кнопок "крайних" недель 
    function DisableWeekButtons() {
        var activeRoom = $('.room.active');
        var activeRoomActiveCalendar = activeRoom.find('.room_calendar.active');
        // считаем количество недель в данном календаре
        var activeRoomWeeks = activeRoomActiveCalendar.find('.calendar_week');
        var calendarWeeksCount = activeRoomWeeks.length;
        var currentWeek = activeRoomActiveCalendar.find('.calendar_week.active');
        var currentWeekOrder = currentWeek.data('weekorder');
        // проверяем, является ли переключенная неделя крайней
        if (calendarWeeksCount == 1) {
            // если неделя единственная, блокируем обе кнопки переключения
            activeRoomActiveCalendar.find('.calendar_week_prev').addClass('disabled');
            activeRoomActiveCalendar.find('.calendar_week_next').addClass('disabled');
        }
        else if (currentWeekOrder - 1 == 0) {
            activeRoomActiveCalendar.find('.calendar_week_prev').addClass('disabled');
            activeRoomActiveCalendar.find('.calendar_week_next').removeClass('disabled');
        }
        else if (currentWeekOrder == calendarWeeksCount) {
            activeRoomActiveCalendar.find('.calendar_week_next').addClass('disabled');
            activeRoomActiveCalendar.find('.calendar_week_prev').removeClass('disabled');
        }
        else {
            activeRoomActiveCalendar.find('.calendar_week_change').removeClass('disabled');
        }

    }

    $('.calendar_week_change').click(function () {
        var activeRoom = $(this).parents('.room.active');
        var activeRoomActiveCalendar = activeRoom.find('.room_calendar.active');
        var currentWeek = activeRoomActiveCalendar.find('.calendar_week.active');
        // порядковый номер недели в данном календаре
        var currentWeekOrder = currentWeek.data('weekorder');
        if ($(this).hasClass('calendar_week_prev')) {
            currentWeekOrder--;
        }
        else {
            currentWeekOrder++;
        }
        // неделя, на которую пойдет переключение
        var changingWeek = activeRoomActiveCalendar.find('[data-weekorder="' + currentWeekOrder + '"]');
        currentWeek.removeClass('active');
        changingWeek.addClass('active');
        ShowCurrentWeekDates();
        DisableWeekButtons();
    });

    // Определяем календарь и дни, которые можно редактировать
    $('.room_calendar[data-editable="true"] .calendar_timeline_day[data-daystatus="available"] .timeline_hour_cells').bind('mousedown', function (e) {
        e.metaKey = true;
    }).selectable();

    $('.room_calendar[data-editable="false"] .timeline_hour_cells').css('cursor', 'auto');


    function ShowActiveTabName() {
        var activeRoom = $('.room.active');
        var activeTab = activeRoom.find('.room_tab.active');
        var activeTabName = activeTab.data('tabname');
        var divTabName = activeRoom.find('.room_tab_change .tab_name');
        divTabName.text(activeTabName);
    }


    // Переключение календарей пользователей в одной комнате
    $('.room_member').click(function () {
        if (!$(this).hasClass('active')) {
            var activeRoom = $(this).parents('.room.active');
            var memberId = $(this).data('memberid');
            activeRoom.find('.room_calendar.active').removeClass('active');
            activeRoom.find('.room_calendar[data-calendaruserid="' + memberId + '"]').addClass('active');
            activeRoom.find('.room_member.active').removeClass('active');
            $(this).addClass('active');
            DisableWeekButtons();
        }
    });

    // Очистка выбранных интервалов 
    $('.calendar_clear').click(function () {
        var activeCalendar = $(this).parents('.room_calendar.active');
        activeCalendar.find('.hour_cell.ui-selected').removeClass('ui-selected');
    });

    /* Переключение вкладок в комнате */
    $('.tab_change').click(function () {
        var activeTab = $('.room.active').find('.room_tab.active');
        var activeTabNumber = activeTab.data('tabnumber');
        if ($(this).hasClass('tab_change_next')) {
            if (activeTabNumber == 2) {
                activeTabNumber = 1;
            }
            else {
                activeTabNumber++;
            }
        }
        else {
            if (activeTabNumber == 1) {
                activeTabNumber = 2;
            }
            else {
                activeTabNumber--;
            }
        }

        activeTab.removeClass('active');
        $('.room.active').find('.room_tab[data-tabnumber="' + activeTabNumber + '"]').addClass('active');
        ShowActiveTabName();

        if ($(this).parents('.room_tabs').find('.room_tab_map').hasClass('active')) {
            LoadRoomMap();
        }

    });
    /* /Переключение вкладок в комнате */



    //Подтверждение выхода из комнаты
    $('.room_leave').click(function () {
        $('.room_leave_wrap').addClass('active');
    });

    $('.room_leave_cancel').click(function () {
        $('.room_leave_wrap').removeClass('active');
    });


    /* Вывод информации по пользователям при наведении на интервал (общий календарь) */
    $('.hour_matched:not(.hour_matched_gold)').mouseenter(function () {
        $('.interval_info').addClass('active');
        var selectedHourUsers = [];
        $(this).find('.hour_cell_user').each(function (i) {
            var selectedHourUser = $(this).text();
            selectedHourUsers.push(selectedHourUser);
        });
        if (selectedHourUsers.length > 0) {
            $.each(selectedHourUsers, function (key, value) {
                $('.interval_info').append("<div class='interval_user_info'>" + value + "</div>");
            });
        }
    });

    $('.hour_matched:not(.hour_matched_gold)').mouseleave(function () {
        $('.interval_info').removeClass('active');
        $('.interval_info').find('.interval_user_info').remove();
    });

    $(document).on('mousemove', function (e) {
        $('.interval_info').css({
            left: e.pageX + 15,
            top: e.pageY + 15
        });
    });
    /* /Вывод информации по пользователям при наведении на интервал (общий календарь) */


    $('.calendar_send').click(function () {
        $('.intervals_confirm_wrap').addClass('active');
        $('main').css('filter', 'blur(5px)');

        // Определяем календарь, в который вносятся изменения
        var currentCalendarId = $(this).parents('.room.active').data('calendarid');
        var currentCalendar = $(this).parents('.room_calendar.active');
        var arIntervals = []; // массив интервалов, который будет передаваться на сервер


        currentCalendar.find('.calendar_timeline_day[data-daystatus="available"]').each(function (i) {
            // информация по каждому дню
            var dayDate = $(this).data('daydate');
            var dayDateNext = $(this).next().data('daydate'); // дата для интервала с конечным часом 00:00
            $(this).find('.hour_cell').each(function (j) {
                if ($(this).hasClass('ui-selected') && !$(this).next().hasClass('ui-selected')) { // определяем крайний час интервала
                    var intervalEndHour = $(this).data('hour') + 1;
                    var intervalLength = $(this).prevUntil('.ui-selectee:not(.ui-selected)').length + 1; // определяем длительность интервала
                    var intervalStartHour = intervalEndHour - intervalLength;
                    // проверка на 24й час
                    if (intervalEndHour == 24) {
                        intervalEndHour = '23:59:59';
                    }
                    else {
                        intervalEndHour = intervalEndHour + ':00:00';
                    }

                    var interval = {
                        CalendarId: currentCalendarId,
                        DateBegin: FormatDate(dayDate) + ' ' + intervalStartHour + ':00:00',
                        DateEnd: FormatDate(dayDate) + ' ' + intervalEndHour
                    }
                    // добавляем интервал в общий массив
                    arIntervals.push(interval);
                }
                else return; // если час не отмечен, пропускаем его               
            });
        });

        if (arIntervals.length == 0) {
            var info = {
                Status: 'none-selected',
                CalendarId: currentCalendarId
            }
            arIntervals.push(info);
        }

        // отправка на сервер
        $.ajax(
            {
                url: '/Room/AddNewInterval',
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(arIntervals)
            }
        ).done(function (result) {
            currentCalendar.find('.calendar_send').text(result.status);
            currentCalendar.find('.calendar_send').css("pointer-events", "none");        
            var roomId = $('.room.active').attr("id");
            location.assign("/Room/Index/" + roomId);
            }).fail(function (a, b, c) {
            console.log(c);
        });

    });


    /* Форматирование даты (для записи интервалов в базу) */
    function FormatDate(date) {
        return date.substr(3, 2) + "." + date.substr(0, 2) + "." + date.substr(6, 4);
    }
    /* /Форматирование даты (для записи интервалов в базу) */



    /* Создание комнаты */
    $('.lobby_add_button').click(function () {
        $('.create_room_wrap').addClass('active');
        $('.create_room_loading').css('display', 'none');
        ResetCreateForm();
    });

    $('.room_add_button').click(function () {
        $('.create_room_wrap').addClass('active');
        $('.create_room_loading').css('display', 'none');
        ResetCreateForm();
    });

    $(".create_room_datefrom input, .create_room_dateto input").datepicker({
        firstDay: 1,
        dateFormat: 'dd.mm.yy',
        minDate: 0,
    });

    function ResetCreateForm() {
        $('#createroom input:not(#create_room_submit)').val("");
        $('#room_image_value').prop('value', $('.room_create_image_min:first').prop('src'));
    }

    function CloseCreateModal() {
        $('.create_room_wrap').removeClass('active');
        $('.create_room_loading').css('display', 'none');
        ResetCreateForm();
    }

    $('#createroom').submit(function () {
        $('.create_room_loading').css('display', 'block');
        setTimeout(CloseCreateModal, 1000);
    });

    $('.room_add_cancel').click(function () {
        CloseCreateModal();
    });

    // Проверяем введенные даты на корректность, если некорректны - сбрасываем введенное значение
    function ParseDateFrom() {
        var dateVal = $('#DateFrom').val();
        var dateDay = dateVal.substr(0, 2);
        var dateMonth = dateVal.substr(3, 2);
        var dateYear = dateVal.substr(6, 4);
        var dateParse = Date.parse(dateMonth + "." + dateDay + "." + dateYear);
        return dateParse;
    }

    function ParseDateTo() {
        var dateVal = $('#DateTo').val();
        var dateDay = dateVal.substr(0, 2);
        var dateMonth = dateVal.substr(3, 2);
        var dateYear = dateVal.substr(6, 4);
        var dateParse = Date.parse(dateMonth + "." + dateDay + "." + dateYear);
        return dateParse;
    }

    function IsValidDateFrom() {
        var dateParse = ParseDateFrom();
        if (isNaN(dateParse) == true) {
            return false;
        }
        else {
            return true;
        }
    }

    function IsValidDateTo() {
        var dateParse = ParseDateTo();
        if (isNaN(dateParse) == true) {
            return false;
        }
        else {
            return true;
        }
    }

    $('#DateFrom').focusout(function () {
        if (IsValidDateFrom() == false) {
            $('#DateFrom').val("");
        }
        else if ($('#DateTo').val() != "") {
            if (ParseDateTo() < ParseDateFrom()) {
                $('#DateTo').val("");
            }
        }
    });

    $('#DateTo').focusout(function () {
        if (IsValidDateTo() == false) {
            $('#DateTo').val("");
        }
        else if ($('#DateFrom').val() != "") {
            if (ParseDateTo() < ParseDateFrom()) {
                $('#DateTo').val("");
            }
        }
    });

    /* /Создание комнаты */


    /* Credits */
    $('#footer_credits').click(function () {
        $('.credits_wrap').addClass('active');
    });

    $('.credits_close').click(function () {
        $('.credits_wrap').removeClass('active');
    });
    /* /Credits */


    /* Выбор изображения комнаты из предложенных */



    // Выбор первого аватара по умолчанию
    $('.room_create_image_min:first').toggleClass('focus');



    //
    $('.room_create_image_min').click(function () {
        // Визуальное выделение выбранного аватара
        $('.room_create_image_min').removeClass('focus');
        $(this).toggleClass('focus');

        // Получение уникального id аватара и передача его в тег input type="hidden"
        var src = $(this).prop("src");
        $('.room_image_id_hidden').prop("value", src);
    });
    /* /Выбор изображения комнаты из предложенных */

});