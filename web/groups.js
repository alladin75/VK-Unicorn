function loadGroups() {
    // Сначала определяем правильные ли настройки у программы. Если настройки не правильные, то загружаем экран настроек
    if (!isSettingsValid()) {
        $.hulla.send("Программа пока ещё не настроена. Загружаем окно настроек", "success");
        loadSettings(true);
        return;
    }

    // Настройки правильные, загружаем сообщества
    clear_workspace();
    start_loading();

    $.getJSON('groups', {
    }, function(result) {
        // Добавляем отдельные категории для сообществ
        function addGroupCategory(isClosed) {
            $(`
                <div id="groups-header-closed-${isClosed}">
                    <h5 class="ml-1 mb-1 mt-2">${isClosed ? 'Закрытые' : 'Открытые'} сообщества<span class="badge badge-success opaque-5 ml-1" id="badge">0</span></h5>
                    <hr class="mx-1 my-0">
                </div>
                <div class="row mx-0" id="groups-closed-${isClosed}">
                </div>
            `).appendTo($('#workspace'));
        };

        let closedGroupsCount = 0;
        addGroupCategory(true);
        let openGroupsCount = 0;
        addGroupCategory(false);

        // Добавляем загруженные сообщества в сооветствующие категории
        for (let groupExtraInfo of result) {
            let group = groupExtraInfo.data;

            let lockElement = '';
            let lockHintElement = '';
            let warningElement = '';
            if (group.IsClosed) {
                lockElement = '<i class="lni-' + (group.IsMember ? 'un' : '') + 'lock mr-1"></i>';
                lockHintElement = '<br><font color=' + (group.IsMember ? 'green' : 'red') + '>Сообщество закрыто и вы ' + (group.IsMember ? '' : 'не ') + 'являетесь его участником</font>';
                if (!group.IsMember) {
                    warningElement = 'bg-error';
                }
            }

            // Заполняем карточку сообщества
            let groupCard = $(`
                <div class="col-auto px-1 py-1">
                    <div class="card ${warningElement} group-card">
                        <div class="card-img-overlay px-1 py-1">
                            <a class="btn btn-danger float-right px-2 py-2" id="delete-button"><i class="lni-close text-white"></i></a>
                        </div>
                        <a href="${groupExtraInfo.URL}" target="_blank">
                            <img class="card-img-top group" src="${group.PhotoURL}">
                        </a>
                        <div class="card-img-overlay medium-info">
                            <span class="small-info-box group-results" id="efficiency">${groupExtraInfo.Efficiency}</span>
                        </div>
                        <div class="card-body py-0 px-2">
                            <p class="card-text my-0 text-truncate" id="group-name">${group.Name}</p>
                        </div>
                        <div class="card-footer pt-0 px-2 pb-p-1">
                            <div class="float-left" id="last-activity"><small class="text-muted">${lockElement}<i class="lni-pulse mr-1"></i><span class="activity-counter">${isoTimeToLocalDeltaAsString(group.LastActivity)}</span></small></div>
                            <div class="float-right" id="last-scanned"><small class="text-muted"><i class="lni-reload pr-p-4"></i><span class="activity-counter">${isoTimeToLocalDeltaAsString(group.LastScanned)}</span></small></div>
                        </div>
                    </div>
                </div>
            `).appendTo($('#groups-closed-' + group.IsClosed));

            // Тултипы
            groupCard.find('#last-activity').popover({
                template: getPopoverTemplateWithClass("no-weight-limit"),
                trigger: 'hover',
                placement: 'top',
                html: true,
                delay: { "show": 450, "hide": 100 },
                content: 'Как давно было найдено что-нибудь полезное в этом сообществе' + lockHintElement
            });

            // Оработчики событий
            groupCard.find('#delete-button').click(function() {
                bootbox.confirm({
                    title: "Удалить?",
                    message: "Вы действительно хотите навсегда удалить сообщество \"" + group.Name + "\"?",
                    backdrop: true,
                    callback: function(result) {
                        if (result) {
                            // Отправляем запрос на удаление
                            $.post("delete_group",
                            {
                                id: group.Id
                            },
                            function(data, status) {
                                if (status) {
                                    // Удаляем сообщество из UI
                                    updateBadgeRelative('#groups-header-closed-' + group.IsClosed, -1);
                                    groupCard.fadeOut();

                                    $.hulla.send("Сообщество \"" + group.Name + "\" удалено", "danger");
                                }
                                else {
                                    $.hulla.send("Не удалось удалить сообщество \"" + group.Name + "\"", "danger");
                                }
                            }).fail(function(result) {
                                $.hulla.send("Не удалось удалить сообщество \"" + group.Name + "\"", "danger");
                            });
                        }
                    }
                });
            });

            // Увеличиваем счётчики
            if (group.IsClosed) {
                ++closedGroupsCount;
            }
            else {
                ++openGroupsCount;
            }
        }

        // Общие тултипы на все элементы
        $('*[id=delete-button]').popover({
            trigger: 'hover',
            placement: 'bottom',
            delay: { "show": 450, "hide": 100 },
            content: 'Удалить сообщество навсегда'
        });

        $('*[id=efficiency]').popover({
            template: getPopoverTemplateWithClass("no-weight-limit"),
            trigger: 'hover',
            placement: 'top',
            delay: { "show": 450, "hide": 100 },
            content: 'Сколько пользователей было найдено из этого сообщества'
        });

        $('*[id=last-scanned]').popover({
            template: getPopoverTemplateWithClass("no-weight-limit"),
            trigger: 'hover',
            placement: 'top',
            delay: { "show": 450, "hide": 100 },
            content: 'Как давно было последнее сканирование сообщества'
        });

        // Заполняем баджи с количеством сообществ
        updateBadgeRelative('#groups-header-closed-true', closedGroupsCount);
        updateBadgeRelative('#groups-header-closed-false', openGroupsCount);

        finish_loading();

        let recordsCount = result.length;
        $.hulla.send(oneFewMany(recordsCount, "Загружено", "Загружены", "Загружено") + " " + oneFewMany(recordsCount, "сообщество", "сообщества", "сообществ", true), "success");

        // Показываем кнопку добавления нового сообщества если всё нормально загрузилось
        $('#add-group-button').show();

        // Не загружено ни одного сообщества? Показываем окно добавления сообществ
        if (recordsCount == 0) {
            showAddGroupDialog();
        }
    }).fail(function(result) {
        finish_loading();

        $.hulla.send("Ошибка при загрузке списка сообществ.<br>Главный модуль программы не запущен или в нём произошла внутренняя ошибка", "danger");
    });
}

function showAddGroupDialog() {
    bootbox.prompt({
        title: "Добавить сообщество",
        message: "Введите адрес сообщества или его короткое имя. Можно в виде списка из нескольких сообществ:",
        inputType: 'textarea',
        placeholder: "https://vk.com/club123456\nhttps://vk.com/public123456\nhttps://vk.com/apiclub\napiclub",
        backdrop: 'static',
        callback: function(groupNames) {
            if (groupNames) {
                // Отправляем запрос на добавление нового сообщества
                $.post("add_group",
                {
                    url: groupNames
                },
                function(data, status) {
                    if (status) {
                        $.hulla.send("Сообщество \"" + groupNames + "\" добавлено<br>Оно появится в списке после начальной обработки", "success");
                    }
                    else {
                        $.hulla.send("Не удалось добавить сообщество \"" + groupNames + "\"", "danger");
                    }
                }).fail(function(result) {
                    $.hulla.send("Не удалось добавить сообщество \"" + groupNames + "\"", "danger");
                });
            }
        }
    });
}