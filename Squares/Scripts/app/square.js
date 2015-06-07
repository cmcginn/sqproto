function Square(options) {
    var settings = options;
    //need to resume
    if (settings.StopWatch.State == 1) {
        settings.StopWatch.Time = Date.now() - settings.StopWatch.Started;
    }
    var t = common.getDuration(settings.StopWatch.Time);

    var result = {
        canHide: ko.observable(false),
        canReset: ko.observable(false),
        duration: {
            days: ko.observable(t.days),
            hours: ko.observable(t.hours),
            minutes: ko.observable(t.minutes),
            seconds: ko.observable(t.seconds)
        },
        visible:ko.observable(!settings.IsHidden),
        id: ko.observable(settings.Id),
        name: ko.observable(settings.Name),
        stopWatch: settings.StopWatch,
        timer: null,
        timerButtonDisplay: ko.observable(''),
        onDurationEdit: function () {
            var d = 0;
            d += Number(result.duration.days()) * 86400000;
            d += Number(result.duration.hours()) * 3600000;
            d += Number(result.duration.minutes()) * 60000;
            d += Number(result.duration.seconds()) * 1000;
            result.timer.modify(d);
            result.save();
        },
        onHideButtonClick: function() {
            result.visible(false);
            var d = result.data();
            $.post(rootPath + 'api/Square',d, function(r) {});
        },
        onRenameClick: function () {
            var d = result.data();
            $.post(rootPath + 'api/Square', d, function (r) { });
        },
        onTimerButtonClick: function () {
            switch (result.timer.state) {
                case 0:
                case 3:
                    result.startTimer();
                    result.save();
                    break;
                case 1:
                    result.pauseTimer();
                    result.save();
                    break;
                case 2:
                    result.startTimer();
                    result.save();
                    break;
                default:
                    break;
            }
            result.updateState();
            $.event.trigger({ type: 'TimerStateChanged', args: result, time: new Date() });
        },
        onTimerButtonResetClick: function () {
            result.timer.stop();
            var d = result.timer.data();

            $.post(rootPath + 'api/stopwatch', d, function (r) {
                //do timer stuffs
                result.timer.reset();
                result.stopWatch = r;
                result.timer.load(result.stopWatch);
                result.refreshDuration(common.getDuration(0));
                result.updateState(result.timer.state);
            });
        },
        onTimerTick: function () {
            if (result.timer) {
                var d = common.getDuration(result.timer.lap());
                result.refreshDuration(d);
            }

        },

        data: function () {
            return {
                StopWatch: result.timer.data(),
                Name: result.name(),
                Id: result.id(),
                IsHidden:!result.visible()
            };
        },

        init: function () {
            var timer = new Tock({ interval: 1000, callback: result.onTimerTick });
            timer.load(settings.StopWatch);
            result.timer = timer;
            result.updateState();
        },
        pauseTimer: function () {
            result.timer.pause();
        },
        refreshDuration: function (d) {
            result.duration.days(d.days);
            result.duration.hours(d.hours);
            result.duration.minutes(d.minutes);
            result.duration.seconds(d.seconds);
        },
        save: function () {
            var d = result.timer.data();
            $.post(rootPath + 'api/stopwatch', d, function (r, s) { });
        },
        startTimer: function () {
            result.timer.start();
        },
        updateState: function () {
            switch (result.timer.state) {
                case 0:
                    result.timerButtonDisplay('Start');
                    result.canReset(false);
                    result.canHide(true);
                    break;
                case 1:
                    result.timerButtonDisplay('Pause');
                    result.canReset(false);
                    result.canHide(false);
                    break;
                case 2:
                    result.timerButtonDisplay('Resume');
                    result.canReset(true);
                    result.canHide(true);
                    break;
                case 3:
                    result.timerButtonDisplay('Start');
                    result.canReset(false);
                    result.canHide(true);
                    break;
                default:
                    break;

            }
        }

    };

    result.init();
    return result;
}