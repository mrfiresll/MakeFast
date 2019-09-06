(function ($) {
    function buildThis(target) {
        var opts = $.data(target, 'mfbuttonedit').options;
        opts.buttonText = '...';
        opts._id = target.id;
        opts._name = target.name;
        opts.editable = false;        
        $(target).textbox($.extend({}, opts, {
            onChange: function (newValue, oldValue) {
                opts.newValue = newValue;
                opts.oldValue = oldValue;
                if (newValue && newValue != '')
                {
                    $(target).textbox('getIcon', 0).css('visibility', 'visible');
                }

                if (opts.onChange) {
                    opts.onChange.call(target, newValue, oldValue);
                }
            },
            icons: [{
                iconCls: 'icon-clear',
                handler: function (e) {
                    $(e.data.target).textbox('clear').textbox('textbox').focus();
                    if (opts.onClearButtonClick)
                    {
                        opts.onClearButtonClick.call(target);
                    }
                    $(this).css('visibility', 'hidden');
                }
            }]
        }));
        $(target).textbox('getIcon', 0).css('visibility', 'hidden');
        $(target).next("span").children("a").bind('click',
                function (e) {
                    opts.onButtonClick({ value: opts.newValue, text: opts.newText, id: opts._id, opts: opts });
                });
    }

    $.fn.mfbuttonedit = function (options, param) {
        if (typeof options == 'string') {
            var method = $.fn.mfbuttonedit.methods[options];
            if (method) {
                return method(this, param);
            } else {
                return this.textbox(options, param);
            }
        }

        options = options || {};
        return this.each(function () {
            var state = $.data(this, 'mfbuttonedit');
            if (state) {
                $.extend(state.options, options);
            } else {
                $.data(this, 'mfbuttonedit', {
                    options: $.extend({}, $.fn.mfbuttonedit.defaults, $.fn.mfbuttonedit.parseOptions(this), options)
                });
            }


            buildThis(this);            
        });
    };

    $.fn.mfbuttonedit.parseOptions = function (target) {
        return $.extend({}, $.fn.textbox.parseOptions(target), {
        });
    };

    $.fn.mfbuttonedit.methods = {
        options: function (jq) {
            var opts = $.data(jq[0], 'mfbuttonedit').options;
            return opts;
        },
        setText: function (jq, text) {
            return jq.each(function () {
                var opts = $.data(this, 'mfbuttonedit').options;
                opts.newText = text
                $(this).textbox('setText', text);
            });
        }
    };

    $.fn.mfbuttonedit.defaults = $.extend({}, $.fn.textbox.defaults, {
        _id: '',
        _name: '',

        newValue: null,newText:null,
        oldValue: null,
        textName: null,//显示值
        onButtonClick: null,
        onClearButtonClick: null
    });

    ////////////////////////////////
    $.parser.plugins.push('mfbuttonedit');
})(jQuery);