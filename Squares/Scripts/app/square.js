﻿function lpad(originalstr, length, strToPad) {
    while (originalstr.length < length)
        originalstr = strToPad + originalstr;
    return originalstr;
}
function setTime(ms) {

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

var Square = function (options) {
    var settings = options || {};
    var result = {
        id: ko.observable(settings.Id),
        elapsed: ko.observable(settings.Elapsed),
        started: ko.observable(settings.Elapsed),
        startDate:ko.observable(settings.StartDate),
        timer:null,
        activityState: ko.observable(settings.ActivityState),
        name:ko.observable(settings.Name),
        value: null,
        displayValue: ko.observable(null),
        onSquareClick: function () {

            switch (result.activityState()) {
                case 0:
                    result.startDate(new Date().getTime());
                    result.startTimer();
                    break;
                case 1:
                    result.pauseTimer();
                    break;
                case 2:
                    result.startDate(new Date().getTime());
                    result.resumeTimer();
                    break;
                default:
                    break;
            }
            $.event.trigger({ type: 'squareClicked', args: result, time: new Date() });
        },
        onTick: function () {

            result.elapsed(result.started() + result.timer.lap());
            result.value = setTime(result.elapsed());
            result.getDisplayValue();
         

            
        },
        getDisplayValue: function () {
            var r = lpad(result.value.days.toString(), 2, '0') + ' - Days ' + lpad(result.value.hours.toString(), 2, '0') + ':' + lpad(result.value.minutes.toString(), 2, '0') + ':' + lpad(Math.floor(result.value.seconds).toString(), 2, '0');
            result.displayValue(r);
        },
        init: function () {
            result.value = setTime(result.elapsed());
            result.getDisplayValue();
            result.timer = new Tock({ interval: 100, callback: result.onTick });
   
        },
        startTimer:function() {
            result.timer.start();
            result.activityState(1);
        },
        pauseTimer: function () {
            result.timer.pause();
            result.activityState(2);
        },
        resumeTimer:function() {
            result.timer.start(result.timer.lap());
            result.activityState(1);
        },
        toData: function() {
            return{
                Elapsed: result.elapsed(),
                Id: result.id(),
                Name: result.name(),
                ActivityState: result.activityState(),
                StartDate:result.startDate()
            };
        }

    };
    result.init();
    return result;
}