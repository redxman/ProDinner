//by default jquery.validate 1.9 doesn't validate hidden inputs
if ($.validator) $.validator.setDefaults({
    ignore: []
});

awe.err = function (o, xhr, textStatus, errorThrown) {
    var msg = "unexpected error occured";
    if (xhr) {
        msg = xhr.responseText;
    }
    var btnHide = $('<button type="button"> hide </button>').click(function () {
        $(this).parent().remove();
    });

    var c = $('<div/>').html(msg).append(btnHide);

    if (o.p && o.p.isOpen) {
        o.p.d.prepend(c);
    } else if (o.f) {
        o.f.html(c);
    } else if (o.d) {
        o.d.after(c);
    } else $('body').prepend(c);
};

$(function () {
    $('#chTheme').change(changeTheme);

    $('#chLang').change(function (e) {
        $.post(_root + "Mui/Change", { l: $(this).val() }, function () {
            location.reload();
        });
    });

    //bind the window min-height to window size
    adjustLayout();
    $(window).on('resize', adjustLayout);
    
    wrapGrids();
    $(document).ajaxComplete(wrapGrids);
    
    if (_isMobileOrTablet) {
        awe.ff = function (o) {
            o.p.d.find(':tabbable').blur();//override jQueryUI dialog autofocus
        };
    }

    //parsing the unobtrusive attributes when we get content via ajax
    $(document).ajaxComplete(function () {
        $.validator.unobtrusive.parse(document);
        //make server-side generated validation errors look like the client side ones
        $('.field-validation-error').each(function () {
            if (!$(this).find('span').length) {
                var x = $(this).html();
                $(this).html('<span>' + x + '</span>');
            }
        });
    });

    // on ie hitting enter doesn't trigger change, 
    // all searchtxt inputs will trigger change on enter in all browsers
    $('.searchtxt').each(function () {
        $(this).data('myval', $(this).val());
    });
    $('.searchtxt').on('change', function (e) {
        if ($(this).val() != $(this).data('myval')) {
            $(this).data('myval', $(this).val());
        } else {
            e.stopImmediatePropagation();
        }
    }).on('keyup', function (e) {
        if (e.which == 13) {
            e.preventDefault();
            if ($(this).val() != $(this).data('myval')) {
                
                $(this).change();
            }
        }
    });
});

//wrap grids for horizontal scrolling on small screens
function wrapGrids() {
    $('.awe-grid:not([wrapped]), table.awe-ajaxlist:not([wrapped])').each(function () {
        var mw = 700;
        if ($(this).data('mw')) {
            mw = $(this).data('mw');
        }

        $(this).wrap('<div style="width:100%; overflow:auto;"></div>')
            .wrap('<div style="min-width:' + mw + 'px;padding-bottom:0px;"></div>')
            .attr('wrapped', 'wrapped');
    });
}

function changeTheme() {
    var v = $(this).val().split("|");
    var theme = v[0];
    var jqTheme = v[1];
    $('#jqStyle').attr('href', "http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.2/themes/" + jqTheme + "/jquery-ui.css");
    $('#aweStyle').attr('href', _root + "Content/themes/" + theme + "/AwesomeMvc.css");
    $('#demoStyle').attr('href', _root + "Content/themes/" + theme + "/Site.css");
    $.post(_root + "ChangeTheme/Change", { s: theme });
}

// o.Type is being set in Crudere.cs to the entity's type name (lower case)

//removes the item from the html
function del(o) {
    $('#' + o.Type + o.Id).fadeOut(300, function () { $(this).remove(); });
    if (o.Type == "dinner") refreshDinnersGrid();
}

//update the item in html
function edit(o) {
    $('#' + o.Type + o.Id).fadeOut(300, function () {
        $(this).after($.trim(o.Content)).remove();
        if (o.Type == "meal") adjustMeals();
    });

    if (o.Type == "dinner") refreshDinnersGrid();
}

//append the html for the created country
function createCountry(o) {
    $('#Countries').prepend(o.Content);
    $('#Countries .awe-li:first').hide().fadeIn();
}

//append the html for the created meal
function createMeal(o) {
    $('#Meals').parent().find('ul').prepend($.trim(o.Content));
}

//append the html for the created country (inside the lookup popup)
function lookupCreateCountry(o) {
    $('#CountryId-awepw .awe-srl').prepend(o.Content);
    $('#CountryId-awepw .awe-srl .awe-li:first').hide().fadeIn();
}

function createDinner(o) {
    $('#Dinners').prepend(o.Content);
    $('#Dinners li:first').hide().fadeIn();
}

function passchanged(o) {
    $("<div> password for " + o.Login + " was successfuly changed </div>").dialog();
}

function createUser(o) {
    $('#Users').parent().find('tbody').prepend(o.Content);
}

function refreshChefGrid() {
    $('#ChefGrid').data('api').load();
}

function refreshDinnersGrid() {
    if ($('#DinnersGrid').length) {
        $('#DinnersGrid').data('api').load();
    }
}

function loadDinnersGrid(r) {
    $('#gridContainer').html(r);
    $('.showGrid').hide();
}

// used for the grid to generate the delete button, will show a restore button if the item is deleted
function dinnersDelFormat(o) {
    var format = "<form class='" + (o.IsDeleted ? "DinnerRestore" : "DinnerConfirm") + "' method='post'>"
        + "<input type='hidden' name='Id' value='" + o.Id + "'/>"
        + "<button type='submit' class='awe-btn'>";

    if (o.IsDeleted) {
        format += "re";
    } else {
        format += "<span class='ico-del'></span>";
    }

    format += "</button>";
    format += "</form>";
    return format;
}

// adjusts the layout of the meal items
function adjustMeals() {
    if ($.support.cors)
        $(".notcool").hide();
    else
        $(".cool").hide();

    var w = $('#Meals').width();
    var mw = 450;
    if (w < mw) mw = w - 20;
    var count = Math.floor(w / mw);
    var rest = w % mw;
    var nw = mw + (rest / count) - 10;
    $('.meal').css('width', nw + 'px');
    $('.comments').css('width', $('.comments:first').parent().width() - $('.comments:first').prev().width() - 20);
}

function adjustLayout() {
    $("#main").css("min-height", ($(window).height() - 120) + "px");
}

function setjQueryValidateDateFormat(format) {
    //setting the correct date format for jquery.validate
    jQuery.validator.addMethod(
        'date',
        function (value, element, params) {
            if (this.optional(element)) {
                return true;
            };
            var result = false;
            try {
                $.datepicker.parseDate(format, value);
                result = true;
            } catch (err) {
                result = false;
            }
            return result;
        },
        ''
    );
}

var _gaq = _gaq || [];
_gaq.push(['_setAccount', 'UA-27119754-1']);
_gaq.push(['_setDomainName', 'aspnetawesome.com']);
_gaq.push(['_trackPageview']);

(function () {
    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
})();

storg = function () {
    if (localStorage) return localStorage;
    var list = {};
    return {
        setItem: function (key, value) {
            list[key] = value;
        },
        getItem: function (key) {
            return list[key];
        }
    };
}();

$(function () {
    var dog = Dog();
    dog.load();
    
    function setText() {
        if (dog.state() != "hidden") {
            $('#chDog').html('hide dog');
        } else {
            $('#chDog').html('show dog');
        }
    }
    
    setText();

    $('#chDog').click(function () {
        dog.state() != "hidden" ? dog.hide() : dog.show();
        setText();
    });
});

function Dog() {
    var state = "visible";
    var h;

    function load() {
        var s = storg.getItem("dogstate");
        if (s) state = s;
        changeStateTo(state);
        $('#doggy').click(function () { changeStateTo(state == "visible" ? "visiblenotip" : "visible"); });
        $('#tip').click(function () {
            clearTimeout(h);
            tell();
        });
        
        $('#doggy').draggable({
            drag: function () {
                var dl = parseFloat($('#doggy').css('left'));
                var dt = parseFloat($('#doggy').css('top'));
                $('#tip').css('left', (dl - 115) + "px").css('top', (dt - 100) + 'px');
            }
        });
    }
    
    function changeStateTo(ns) {
        states[ns]();
        state = ns;
        storg.setItem("dogstate", state);
    }
    
    var times = 0;
    function tell() {
        if (state == "visible")
            $.post(_root + "doggy/tell", { c: _controller, a: _action },
            function (d) {
                $('#tipcontent').html(d.o);
                times++;
                if (times < 10)
                    h = setTimeout(tell, Math.random() * 8000 + 5000);
            });
    }

    var states = {
        visible: function () {
            $('#doggy,#tip').show();
            h = setTimeout(tell, 5000);
        },
        visiblenotip: function () {
            $('#doggy').show();
            $('#tip').hide();
            clearTimeout(h);
        },
        hidden: function () {
            $('#doggy,#tip').hide();
            clearTimeout(h);
        }
    };

    return {
        load: load,
        hide: function () {
            changeStateTo("hidden");
        },
        show: function () {
            changeStateTo("visible");
        },
        state: function () { return state; }
    };
}