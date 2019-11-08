// Возвращает слово в зависимости от числа
// 21 год
// 22 года
// 27 лет
function oneFewMany(number, one, few, many)
{
    number = Math.abs(number);
    var mod100 = number % 100;

    var result = many;
    if (!(mod100 >= 11 && mod100 <= 19))
    {
        switch (number % 10)
        {
            case 1:
                result = one;
                break;

            case 2:
            case 3:
            case 4:
                result = few;
                break;
        }
    }

    return result;
}

function isoTimeToLocalDeltaAsString(arg) {
    if (arg === undefined) {
        return 'никогда';
    }

    var isoTime = Date.parse(arg);
    if (isoTime < 0)
    {
        return 'никогда';
    }

    var delta = parseInt((new Date().getTime() - isoTime) / 1000 / 60);
    if (delta < 60) {
        if (delta == 0) {
            return 'только что';
        }

        return delta + ' ' + oneFewMany(delta, 'минута', 'минуты', 'минут');
    }

    delta = parseInt(delta / 60)
    if (delta < 24) {
        return delta + ' ' + oneFewMany(delta, 'час', 'часа', 'часов');
    }

    delta = parseInt(delta / 24);
    return delta + ' ' + oneFewMany(delta, 'день', 'дня', 'дней');
}