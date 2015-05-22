//function SquareModel(options) {
//    var settings = options || {};
    
//    var result = {};
//    return result;
//}
var model= {
    viewModel:null
}

var controller = {
    init:function() {
        controller.getUserSquares();
    },
    getUserSquares: function() {
        $.get(rootPath + 'api/Square', function(r, s) {
            model.viewModel = ko.mapping.fromJS(r);
            ko.applyBindings(model.viewModel);
        });
    },
    onSquareClick: function (e) {
        for (var i = 0; i < model.viewModel.UserSquares().length; i++) {
            if (model.viewModel.UserSquares()[i].timer)
                clearInterval(model.viewModel.UserSquares()[i].timer);
        }

        e.SquareActivity.ActivityType(1);
        
        var usid = e.Id();
        var $obj = null;
        var data = ko.mapping.toJS(model.viewModel);
        $.post(rootPath + 'api/Square', data, function (r, s) {
            ko.mapping.fromJS(r, model.viewModel);
            for (var i = 0; i < model.viewModel.UserSquares().length; i++) {

                if (model.viewModel.UserSquares()[i].Id() == usid) {
                    $obj = model.viewModel.UserSquares()[i];
                    $obj.timer = setInterval(function () {
                        $obj.TimerValue($obj.TimerValue() + 1);
                    }, 1000);
                }
            }
        });
        
        //$.post(rootPath + 'api/Square', data, function(r, s) {
        //    ko.mapping.fromJS(r, model.viewModel);
        //});
        //var $obj = e;
  
    }
}

$(function() {
    controller.init();
});