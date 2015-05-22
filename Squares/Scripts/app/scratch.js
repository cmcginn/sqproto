function lpad(originalstr, length, strToPad) {
    while (originalstr.length < length)
        originalstr = strToPad + originalstr;
    return originalstr;
}

function rpad(originalstr, length, strToPad) {
    while (originalstr.length < length)
        originalstr = originalstr + strToPad;
    return originalstr;
}

function getDateDisplay(ms) {
    var delta = Math.abs(ms) / 1000;

    // calculate (and subtract) whole days
    var days = Math.floor(delta / 86400);
    delta -= days * 86400;

    // calculate (and subtract) whole hours
    var hours = Math.floor(delta / 3600) % 24;
    delta -= hours * 3600;

    // calculate (and subtract) whole minutes
    var minutes = Math.floor(delta / 60) % 60;
    delta -= minutes * 60;

    // what's left is seconds
    var seconds = delta % 60;

    var result = { days: days, hours: hours, minutes: minutes, seconds: seconds };
    return result;
}
function dateDiff(date_now, date_future) {
    var delta = Math.abs(date_future - date_now) / 1000;

    // calculate (and subtract) whole days
    var days = Math.floor(delta / 86400);
    delta -= days * 86400;

    // calculate (and subtract) whole hours
    var hours = Math.floor(delta / 3600) % 24;
    delta -= hours * 3600;

    // calculate (and subtract) whole minutes
    var minutes = Math.floor(delta / 60) % 60;
    delta -= minutes * 60;

    // what's left is seconds
    var seconds = delta % 60;

    var result = { days: days, hours: hours, minutes: minutes, seconds: seconds };
    return result;
}

function parseDate(s) {

}
var x = new Date('2015-05-15T08:57:40.414Z');
var y = new Date('2015-05-18T08:56:55.614Z');
var v = y - x;
function onTime() {
    var z = v + timer.lap();
    var q = getDateDisplay(z);
    //
    var disp = lpad(q.days.toString(), 2, '0') + ' - Days ' + lpad(q.hours.toString(), 2, '0') + ':' + lpad(q.minutes.toString(), 2, '0') + ':' + lpad(Math.floor(q.seconds).toString(), 2, '0');
    $('#clock').val(disp);
};



var timer = new Tock({ interval: 10, callback: onTime });
timer.start();