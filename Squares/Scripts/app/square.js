function Square(options) {
    var postPath = rootPath + 'api/TimerAction';

    function toTimerActionModel(state) {

        return {
            Time: new Date().getTime(),
            Elapsed: result.timer.lap(),
            ActivityState: state,
            Id: result.activityId(),
            ParentId: result.id(),
            Modified: false
        };
    }

    var settings = options || {};
    var result = {
        id: ko.observable(settings.Id),
        activityId: ko.observable(settings.UserSquareActivityId),
        elapsed: ko.observable(settings.Elapsed),
        duration: ko.observable({
            days: ko.observable(0),
            hours: ko.observable(0),
            minutes: ko.observable(0),
            seconds: ko.observable(0),
            milliseconds: ko.observable(0)
        }),
        name: ko.observable(settings.Name),
        state: ko.observable(settings.ActivityState),
        timer: null,
        canReset: ko.observable(false),
        startTimer: function () {
            result.timer.start(result.elapsed());
            $.post(postPath, toTimerActionModel(1, 0), function (r) {
                result.activityId(r.Id);
                result.state(r.ActivityState);
            });

        },
        pauseTimer: function () {
            result.timer.pause();
            $.post(postPath, toTimerActionModel(2, 0), function (r) {
                result.activityId(r.Id);
                result.state(r.ActivityState);
            });
        },
        resumeTimer: function () {
            if (result.timer.pause_time == 0) {
                result.timer.start(result.elapsed());
            } else
                result.timer.pause();
            $.post(postPath, toTimerActionModel(1, 0), function (r) {
                result.activityId(r.Id);
                result.state(r.ActivityState);
            });
        },
        stopTimer: function () {

            result.timer.stop();
            $.post(postPath, toTimerActionModel(3, 0), function (r) {
                result.activityId(r.Id);
                result.state(r.ActivityState);
                var d = common.getDuration(0);
                result.duration().days(d.days);
                result.duration().hours(d.hours);
                result.duration().minutes(d.minutes);
                result.duration().seconds(d.seconds);
                result.duration().milliseconds(d.milliseconds);
                result.elapsed(0);
            });

        },
        toData: function () {
            return {
                UserSquareActivityId: result.activityId(),
                Id: result.id(),
                ActivityState: result.state(),
                Elapsed: result.timer.lap(),
                Time: new Date().getTime(),
                Name: result.name()
            };
        },
        init: function () {
            var ms = result.elapsed();
            var d = common.getDuration(ms);
            result.duration().days(d.days);
            result.duration().hours(d.hours);
            result.duration().minutes(d.minutes);
            result.duration().seconds(d.seconds);
            result.duration().milliseconds(d.milliseconds);
            result.timer = new Tock({ interval: 1000, callback: result.onTick });
            if (result.state() == 1)
                result.timer.start(result.elapsed());
        },
        onDurationEdit: function () {
            if (result.state() != 0)
                result.timer.stop();
            var d = 0;
            d += Number(result.duration().days()) * 86400000;
            d += Number(result.duration().hours()) * 3600000;
            d += Number(result.duration().minutes()) * 60000;
            d += Number(result.duration().seconds()) * 1000;

            result.elapsed(d);
            var data = toTimerActionModel(result.state(), 0);
            data.Elapsed = d;
            data.Modified = true;
            $.post(postPath, data, function (r) {
                if (result.state() != 0);
                result.timer.start(result.elapsed());
            });


        },
        onRenameClick: function () {
            $.post(rootPath + 'Home/Rename', result.toData(), function (r, s) { });
        },
        onTick: function (e) {
            var ms = result.timer.lap();
            var d = common.getDuration(ms);
            result.duration().days(d.days);
            result.duration().hours(d.hours);
            result.duration().minutes(d.minutes);
            result.duration().seconds(d.seconds);
            result.duration().milliseconds(d.milliseconds);

        },

        onTimerButtonClick: function () {
            switch (result.state()) {
                case 0:
                    result.startTimer();
                    break;
                case 1:
                    result.pauseTimer();
                    break;
                case 2:
                    result.resumeTimer();
                    break;
                case 3:
                    result.startTimer();
                    break;
                default:
                    break;
            }
        },
        onTimerButtonResetClick: function () {
            result.stopTimer();
        },

    };

    result.timerButtonDisplay = ko.computed(function () {
        var r = '';
        switch (result.state()) {
            case 0:
                r = 'Start';
                result.canReset(false);
                break;
            case 1:
                r = 'Pause';
                result.canReset(false);
                break;
            case 2:
                r = 'Resume';
                result.canReset(true);
                break;
            case 3:
                r = 'Start';
                result.canReset(false);
                break;
            default:
                break;

        }
        return r;
    }, this);
    result.init();
    return result;
}