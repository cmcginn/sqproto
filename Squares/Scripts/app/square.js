function lpad(originalstr, length, strToPad) {
    while (originalstr.length < length)
        originalstr = strToPad + originalstr;
    return originalstr;
}
function getDuration(ms) {

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
    var seconds = Math.floor(delta % 60);

    var result = { days: days, hours: hours, minutes: minutes, seconds: seconds, milliseconds:ms };

    return result;
}
function toMS(d) {
    var ms = 0;
    ms += d.days * 86400000;
    ms += d.hours * 3600000;
    ms += d.minutes * 60000;
    ms += d.seconds * 1000;
    return ms;
}
var Square = function (options) {
    var settings = options || {};

    var result = {
        id: ko.observable(settings.Id),
        elapsed: ko.observable(settings.Elapsed),
        started: ko.observable(settings.Elapsed),
        startDate: ko.observable(settings.StartDate),
        duration: ko.observable({ days: settings.Duration.Days, hours: settings.Duration.Hours, minutes: settings.Duration.Minutes, seconds: settings.Duration.Seconds, milliseconds: settings.Duration.Milliseconds }),
        days: ko.observable(settings.Duration.Days),
        hours: ko.observable(settings.Duration.Hours),
        minutes: ko.observable(settings.Duration.Minutes),
        seconds: ko.observable(settings.Duration.Seconds),
        milliseconds: ko.observable(Number(settings.Duration.Milliseconds)),
        enabled:ko.observable(true),
        timer: null,
        activityState: ko.observable(settings.ActivityState),
        availableState:ko.observable(''),
        name: ko.observable(settings.Name),
        setAvailableState:function () {
            var r = 'Start';
            switch (result.activityState()) {
                case 0:
                case 2:
                    r = 'Start';
                    break;
                case 1:
                    r = 'Pause';
                    break;
                default:
                    break;
            }
            result.availableState(r);
        },
        value: null,
        canReset: ko.observable(false),
        canHide: ko.observable(false),
        visible: ko.observable(settings.Visible),
        onHideClick: function() {
            $.event.trigger({ type: 'hideClicked', args: result, time: new Date() });
        },
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
            result.canHide(result.activityState() != 1);
            result.setAvailableState();
            $.event.trigger({ type: 'squareClicked', args: result, time: new Date() });
        },
        onTimeChanged: function () {
            var duration = { days: result.days(), hours: result.hours(), minutes: result.minutes(), seconds: result.seconds() };
            var ms = toMS(duration);
            result.duration(getDuration(ms));
        },
        onRenameClick: function() {
            $.event.trigger({ type: 'renameClicked', args: result, time: new Date() });
        },
        onTick: function () {
            
            result.elapsed(result.timer.lap());
            var duration = getDuration(result.duration().milliseconds + result.timer.lap());
            result.days(duration.days);
            result.hours(duration.hours);
            result.minutes(duration.minutes);
            result.seconds(duration.seconds);
            result.milliseconds(duration.milliseconds);
           
        },
        init: function () {
           
            result.timer = new Tock({ interval: 1000, callback: result.onTick });
            result.canReset(result.activityState() > 1);
            result.canHide(result.activityState() != 1);
            result.setAvailableState();
            
        },
        startTimer:function() {
            result.timer.start();
            result.activityState(1);
            result.enabled(false);
        },
        pauseTimer: function () {
            result.timer.pause();
            result.activityState(2);
            result.enabled(true);
        },
        resumeTimer:function() {
            result.timer.start(result.timer.lap());
            result.activityState(1);
            result.enabled(false);
        },
        stopTimer: function() {
            result.timer.stop();
            result.elapsed(0);
            result.activityState(0);
            result.duration(getDuration(0));
            result.enabled(true);

        },
        toData: function() {
            return{
                Elapsed: result.elapsed(),
                Id: result.id(),
                Name: result.name(),
                ActivityState: result.activityState(),
                StartDate: result.startDate(),
                Duration:result.duration()
            };
        }

    };
   
    result.init();
    return result;
}