function ActivityRecord(options) {
    var settings = options;
    var duration = common.getDuration(settings.Elapsed);
    var activityState = '';
    switch (settings.State) {
        case 0:
            activityState = 'None';
            break;
        case 1:
            activityState = 'Running';
            break;
        case 2:
            activityState = 'Paused';
            break;
        case 3:
            activityState = 'Stopped';
            break;
        default:
            break;
    }
    var result = {
        activityState: ko.observable(activityState),
        duration: {
            days: ko.observable(duration.days),
            hours: ko.observable(duration.hours),
            minutes: ko.observable(duration.minutes),
            seconds: ko.observable(duration.seconds),
            milliseconds: ko.observable(duration.milliseconds)

        },
        elapsed: ko.observable(settings.Elapsed),
        id: ko.observable(settings.Id),
        isDeleted:ko.observable(settings.isDeleted),
        started: ko.observable(settings.Started),
        ended: ko.observable(settings.Ended),
        startDate: ko.observable(new Date(settings.Started)),
        data: function () {
            return {
                Id: result.id(),
                Elapsed: common.getMS(result.duration),
                Started: result.startDate().getTime(),
                IsDeleted:result.isDeleted()
            }
        },
        init: function () { },
        onDeleteActivityItemClick: function() {
            result.isDeleted(true);
        }
    };
    result.visible = ko.computed(function() {
        return !result.isDeleted();
    }, this);
    result.init();
    return result;
}

function ReportItem(options) {
    var settings = options;
    var duration = common.getDuration(settings.TotalDuration);

    var activityRecords = [];
    for (var i = 0; i < settings.ActivityRecords.length; i++) {
        var ar = new ActivityRecord(settings.ActivityRecords[i]);
        activityRecords.push(ar);
    }
    var result = {
        activityRecords: (ko.observable(activityRecords)),
        canSave: ko.observable(settings.State != 1),
        duration: {
            days: ko.observable(duration.days),
            hours: ko.observable(duration.hours),
            minutes: ko.observable(duration.minutes),
            seconds: ko.observable(duration.seconds)
        },
        isDeleted: ko.observable(settings.isDeleted),
        id: ko.observable(settings.Id),
        name: ko.observable(settings.Name),
        started: ko.observable(settings.Started),
        state: ko.observable(settings.State),
        totalDuration: ko.observable(settings.TotalDuration),
        data: function () {
            var r = {
                Name: result.name(),
                Id: result.id(),
                TotalDuration: result.totalDuration(),
                Started: result.started(),
                State: result.state(),
                ActivityRecords: [],
                IsDeleted:result.isDeleted()
            };
            for (var i = 0; i < result.activityRecords().length; i++) {
                    r.ActivityRecords.push(result.activityRecords()[i].data());
            }
            return r;

        },
        init: function () { },
        onDeleteItemClick: function() {
           
            result.isDeleted(true);
            var d = result.data();
            $.post(rootPath + 'api/Report', d, function (r) {
                $.cookie('refresh',true);
            });
        },
        onSaveItemClick: function () {
            var d = result.data();
            $.post(rootPath + 'api/Report', d, function(r) {
                $.cookie('refresh', true);
            });

        },
    };
    result.totalDurationDisplay = ko.computed(function () {
        var t = 0;
        for (var i = 0; i < result.activityRecords().length ; i++) {
            if (!result.activityRecords()[i].isDeleted())
                t += common.getMS(result.activityRecords()[i].duration);
        }
        var d = common.getDuration(t);
        result.duration.days(d.days);
        result.duration.hours(d.hours);
        result.duration.minutes(d.minutes);
        result.duration.seconds(d.seconds);

        return result.duration.days() + ' Days ' + result.duration.hours() + ' Hours ' + result.duration.minutes() + ' Minutes ' + result.duration.seconds() + ' Seconds';
    }, this);
    result.visible = ko.computed(function () {
        return !result.isDeleted() && result.activityRecords().length > 0;
    }, this);
    result.init();
    return result;
}