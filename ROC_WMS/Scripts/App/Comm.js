function DateTimeFormatter(value, row) {
    if (value == undefined) {
        return "";
    }
    /*json格式时间转js时间格式*/
    value = value.substr(1, value.length - 2);
    var obj = eval('(' + "{Date: new " + value + "}" + ')');
    var dateValue = obj["Date"];
    if (dateValue.getFullYear() < 1900) {
        return "";
    }
    return dateValue.toLocaleString();
}

//在String对象中扩展一个toDate方法，可以根据要求完善
String.prototype.toDate = function () {
    var dateMilliseconds;
    if (isNaN(this)) {
        //使用正则表达式将日期属性中的非数字（\D）删除
        dateMilliseconds = this.replace(/\D/igm, "");
    } else {
        dateMilliseconds = this;
    }
    //实例化一个新的日期格式，使用1970 年 1 月 1 日至今的毫秒数为参数
    return new Date(parseInt(dateMilliseconds));
};

//EasyUI DateTimeBox控制中转换函数  将JSON格式的DateTime字符串转成JS的时间对象
function DatetimeParser(s) {
    if (s == undefined) {
        return new Date();
    }
    else {
        return s.toDate();
    }
}

function BoolFormat(value, row) {
    if (value == undefined) {
        return unchecked;
    }
    return value == true ? checked : unchecked;
}

function FormatDecimal(value, row) {
    if (value == undefined) {
        return "";
    }
    return value.toFixed(2);
}



//判断字符是否为空的方法
function isEmpty(obj) {
    if (typeof obj == "undefined" || obj == null || obj == "") {
        return true;
    } else {
        return false;
    }
}

//通过iframe 打开一个新的窗体
function OpenIFame(title, url,dialogClose) {
    if (!isEmpty(url)) {
        $('#dialog').dialog(
        {
            title: title,
            width: 600,
            height: 400,
            closed: false,
            cache: false,
            modal: true,
            onClose: dialogClose,
            content: CreateIFame(url)
        });
    }
}

function CreateIFame(src) {
    var iframe = "<iframe width='100%' height='98%' scrolling='no' frameborder='0' src='" + src + "'></iframe>";
    return iframe;
}

//弹出框及系统消息框
function ShowMsg(title, msg, isAlert) {
    if (isAlert !== undefined && isAlert) {
        $.messager.alert(title, msg);
    }
    else {
        $.messager.show({
            title: title,
            msg: msg,
            showType: 'show'
        });
    }
}

//确认框
function ShowConfirm(title, msg, callback) {
    $.messager.confirm(title, msg, function (r) {
        if (r) {
            if (jQuery.isFunction(callback)) {
                callback.call();
            }
        }
    });
}
//进度框
function ShowProcess(isShow, title, msg) {
    if (!isShow) {
        $.messager.progress('close');
        return;
    }
    var win = $.messager.progress({
        title: title,
        msg: msg
    });
}

//过滤树数据
function TreeConvert(rows) {
    function exists(rows, parentCode) {
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].Code == parentCode) return true;
        }
        return false;
    }

    var nodes = [];
    // get the top level nodes
    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];
        if (!exists(rows, row.ParentCode)) {
            nodes.push({
                id: row.Code,
                text: row.Name,
                attributes: {
                    Id:row.Id
                }
            });
        }
    }

    var toDo = [];
    for (var i = 0; i < nodes.length; i++) {
        toDo.push(nodes[i]);
    }
    while (toDo.length) {
        var node = toDo.shift();    // the parent node
        // get the children nodes
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row.ParentCode == node.id) {
                var child = {
                    id: row.Code,
                    text: row.Name,
                    attributes: {
                        Id: row.Id
                    }
                };
                if (node.children) {
                    node.children.push(child);
                } else {
                    node.children = [child];
                }
                toDo.push(child);
            }
        }
    }
    return nodes;
}