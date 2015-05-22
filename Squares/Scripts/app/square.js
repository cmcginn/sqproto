function lpad(originalstr, length, strToPad) {
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
        startDate: ko.observable(settings.StartDate),
        runningTime:ko.observable(settings.RunningTime),
        timer:null,
        activityState: ko.observable(settings.ActivityState),
        name:ko.observable(settings.Name),
        value: null,
        displayValue: ko.observable(null),
        canReset:ko.observable(false),
        onResetClick:function() {
            $.event.trigger({ type: 'resetClicked', args: result, time: new Date() });
        },
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
            //can reset if started or paused
            result.canReset(result.activityState() > 1);
            $.event.trigger({ type: 'squareClicked', args: result, time: new Date() });
        },
        onTick: function () {

            result.elapsed(result.timer.lap());
            result.value = setTime(result.runningTime() + result.elapsed());
            result.getDisplayValue();
         

            
        },
        getDisplayValue: function () {
            var r = lpad(result.value.days.toString(), 2, '0') + ' - Days ' + lpad(result.value.hours.toString(), 2, '0') + ':' + lpad(result.value.minutes.toString(), 2, '0') + ':' + lpad(Math.floor(result.value.seconds).toString(), 2, '0');
            result.displayValue(r);
        },
        init: function () {
            result.value = setTime(result.runningTime());
            result.getDisplayValue();
            result.timer = new Tock({ interval: 100, callback: result.onTick });
            result.canReset(result.activityState() > 1);
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
        stopTimer: function() {
            result.timer.stop();
            result.elapsed(0);
            result.value = setTime(result.elapsed());
            result.activityState(0);
            result.runningTime(0);
            result.getDisplayValue();

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