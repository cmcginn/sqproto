var common =  {
    lpad:function(originalstr, length, strToPad) {
        while (originalstr.length < length)
            originalstr = strToPad + originalstr;
        return originalstr;
    },
    getDuration:function(ms) {
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
      
        var result = { days: days, hours: hours, minutes: minutes, seconds: seconds,milliseconds:ms };

        return result;
    },
    getMS:function(duration) {
      
        var d = 0;
        if (typeof duration.days == typeof(Function)) {
            d += Number(duration.days()) * 86400000;
            d += Number(duration.hours()) * 3600000;
            d += Number(duration.minutes()) * 60000;
            d += Number(duration.seconds()) * 1000;
        } else {
            d += Number(duration.days) * 86400000;
            d += Number(duration.hours) * 3600000;
            d += Number(duration.minutes) * 60000;
            d += Number(duration.seconds) * 1000;
        }
        return d;
        
    }
    
}
