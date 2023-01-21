
var myCells = [];
var myhours = ['00:00', '01:00', '02:00', '03:00', '04:00', '05:00', '06:00', '07:00', '08:00', '09:00', '10:00', '11:00', '12:00', '13:00', '14:00', '15:00', '16:00', '17:00', '18:00', '19:00', '20:00', '21:00', '22:00', '23:00'];
var myShorthours = ['00', '01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23'];
var myminutes = ['00', '15', '30', '45'];
var myminutes10 = ['00', '10', '20', '30','40','50'];
var myminutes15 = ['00', '15', '30', '45'];
var myminutes20 = ['00', '20', '40'];
var myminutes30 = ['00', '30'];
//var myCells = "";
var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

flatpickr('#mnmfrom', { dateFormat: 'd-m-Y', minDate: 'today' });
flatpickr('#mnmto', { dateFormat: 'd-m-Y', minDate: 'today' });
flatpickr('#mnmDat', { dateFormat: 'YmdH:i:ss', enableTime: true, minDate: 'today' });

$(document).ready(function ()
{
    days = [];
    days.push($('#mnmSu').val());
    days.push($('#mnmMo').val());
    days.push($('#mnmTues').val());
    days.push($('#mnmWed').val());
    days.push($('#mnmTh').val());
    days.push($('#mnmFr').val());
    days.push($('#mnmSat').val());
});

function getRemainder(x,y)
{
    return x % y;
}

function drawTable(rows)
{
    return '<table style="verflow-y: auto;">' + rows + '</table>';
}

function drawDays(dayName,dayDate)
{
    dayRow = '<tr>' +
        '<td style="border:double;background: wheat;">' +
        dayName+'<br/>'+dayDate+
    '</td>' +
        '<td style="border:double">' +
        '<table class="table"><thead>' +
        drawHours(dayName,dayDate) +
        '</thead></table>' +
    '</td>' +
        '</tr>';
    return dayRow;
}

function drawHours(dayName,dayDate)
{
    var hourRow = '<tr>';
    for (var i = 0; i < 24; i+=2)
    {
        hourRow += '<th style="text-align:center;background:wheat;border-radius: 30px;">' +
           "(" + dayName +" "+ dayDate + ")  " + myhours[i] +
            '</th>';
        
        hourRow += '<th style="text-align:center;background:coral;border-radius: 30px;">' +
    "(" + dayName + " " + dayDate + ")  " + myhours[i + 1] +
    '</th>';
    }
    hourRow += '</tr>';

    hourRow += '<tr>';
    for (var i = 0; i < 24; i+=2) {
        hourRow += drawMinutes(dayDate, myhours[i],myShorthours[i], '#fff3ea');
        hourRow += drawMinutes(dayDate, myhours[i + 1],myShorthours[i+1], '#efa685');
    }
    hourRow += '</tr>';
    return hourRow;
}

function drawMinutes(dayDate, hour,sh,myColor) {

    var minuteRows = "<td>" +
                "<table><tbody style='background:" + myColor + ";box-shadow: 5px 7px 3px rgba(0, 0, 0, 0.3);'>" +
                "<tr>";
    for (var i = 0; i < myminutes.length; i++)
    {
        minuteRows += "<td style='text-align:center'>" +sh+":" +myminutes[i] + "</td>";
    }
    minuteRows += "</tr>";

    minuteRows += "<tr>";
    for (var i = 0; i < myminutes.length; i++)
    {
        var xox = dayDate + "_" + sh + "_" + myminutes[i];
        myCells.push(dayDate + "_" + sh + "_" + myminutes[i]);
        minuteRows += "<td id='" + dayDate + "_" + sh + "_" + myminutes[i] + "'>" + "<input type='button' class='form-control img-circle' style='background:#808080' onclick='addOrderrr(" +'"'+ xox+ '"'+")'/>" + "</td>";
            
        //myCells += dayDate + "-" + hour + "-" + i + "*";
    }

        
    minuteRows += "</tr>" +

                "<tbody></table>" +
        "</td>";

    return minuteRows;
}

function getDaysNumber()
{
    var sss=$('#mnmfrom').val();
    var startYear = $('#mnmfrom').val().substr(6, 9);
    var startMonth = $('#mnmfrom').val().substr(3, 2);
    var startDay = $('#mnmfrom').val().substr(0, 2);

    var endYear = $('#mnmto').val().substr(6, 9);
    var endMonth = $('#mnmto').val().substr(3, 2);
    var endDay = $('#mnmto').val().substr(0, 2);

    var startDatee = Date.parse($('#mnmfrom').val().substr(6, 9) + "-" + $('#mnmfrom').val().substr(3, 2) + "-" + $('#mnmfrom').val().substr(0, 2));
    var endDate = Date.parse($('#mnmto').val().substr(6, 9) + "-" + $('#mnmto').val().substr(3, 2) + "-" + $('#mnmto').val().substr(0, 2));

    var timeDiff = endDate - startDatee;
    daysDiff = Math.floor(timeDiff / (1000 * 60 * 60 * 24));
    return daysDiff;
}

var activeCells = [];
var previousIndex = -1;
var nextIndex = -1;

function drawCalender()
{
    $('#mnmAlert').hide();
    $('#mnmAlert1').hide();
    $('#mnmNext').hide();
    cancelFastPreview();
    myCells = [];
    activeCells = [];
    var myStep = parseFloat($('#calStep').val());
    myminutes=[];
    if (myStep == "15")
    {
        myminutes = myminutes15;
    }
    else
    {
        if (myStep == "30")
        {
            myminutes = myminutes30;
        }
        else {
            if (myStep == "10") {
                myminutes = myminutes10;
            }
            else
            {
                if (myStep == "20") {
                    myminutes = myminutes20;
                }
            }
        }
    }

    $('#scheduleTable').empty();

    if ($('#mnmfrom').val() == "" || $('#mnmto').val() == "")
    {
        $('#mnmAlert').show();
        return;
    }
    var today = new Date($('#mnmfrom').val().substr(6, 9) + "-" + $('#mnmfrom').val().substr(3, 2) + "-" + $('#mnmfrom').val().substr(0, 2));
    var startDay = today.getDay() + 1;
    var today1 = new Date($('#mnmto').val().substr(6, 9) + "-" + $('#mnmto').val().substr(3, 2) + "-" + $('#mnmto').val().substr(0, 2));
    if (today > today1) {
        $('#mnmAlert1').show();

        return;
    }

    var daysToShow = getDaysNumber()+1;
    var myWeek=[];
    var rows = '';

    for (var i = 0; i < daysToShow; i++) {
        myWeek.push(getRemainder(startDay + i, 7));
    }

    for (var i = 0; i < myWeek.length; i++) {
        rows += drawDays(days[myWeek[i]], addDays(today, i ).toISOString().slice(0,10));

    }
    var tbl = drawTable(rows);
    $('#scheduleTable').append(tbl);
    $('#scheduleTable').hide();
    $('#mnmCalenderContaines').show();
    $('#loadingDiv').show();
    $.ajax
(
{
    type: "POST",
    dataType: "json",
    url: "/Schedule/pyramidSearch/",
    data: { dates: myCells, myShorthours: myShorthours, myminutes: myminutes, mod: $('#mmnmModality').val() },
    success: function(d)
    {
        for (var i = 0; i < myCells.length;i++)
        {
            var myTd = document.getElementById(myCells[i]);
            if (d.data[i].orderId!="-1")
            {
                var x = 0;
                $('#' + myCells[i]).empty();
                //myTd.empty();
                var newBtn = "<input type='button' class='form-control img-circle' style='background:red;border-radius: 19px;' onmouseout='cancelFastPreview()' onmouseover='fastPreview(" + d.data[i].orderId + ")' ondblclick='showOrder(" + d.data[i].orderId + ")'/>";
                $('#' + myCells[i]).append(newBtn);
                //nextIndex = 0;
                //activeCells.push('#' + myCells[i]);
                //myTd.setAttribute('style', 'background:red');
                //alert("MNM");
            }
            //else
            //{
            //    myTd.setAttribute('style', 'background:green');
            //}
        }
        //debugger;
        //previousIndex = activeCells.length;

        $('#scheduleTable').show();
        
        $('#mnmNext').show();

        $('#loadingDiv').hide();
    }
});
        

}

function addDays(date, days) {
    var result = new Date(date);
    result.setDate(result.getDate() + days);
    return result;
}
    
function addOrderrr(val)
{
    debugger;
    var car = val.replace("_", " ");
    var car = car.replace("_", " ");
}

function hideMe()
{
    $('#myModal').modal('hide');

}

function showOrder(id)
{
    $('#mnmEdit').prop('disabled', true);

    $.ajax
        (
        {
            type: "POST",
            dataType: "json",
            url: "/Schedule/previewOrder/",
            data: { id: id },
            success: function (d) {
                $('#mnmPat').empty();
                $('#mnmDat').empty();
                $('#mnmDatOld').empty();
                //$('#mnmOrderId').empty();

                $('#mnmPat').append(d.data[0].patientName);
                $('#mnmDatOld').append(d.data[0].orderDate);
                //$('#mnmDat').attr('value', d.data[0].orderDate);

                $('#mnmOrderId').attr('value', (d.data[0].id));
                var options = { "backdrop": "static", keyboard: true }
                $('#myModal').modal(options);
                $('#myModal').modal('show');
                //alert('patient: ' + d.data[0].patientName + '\n' + d.data[0].modalityName + '\n' + d.data[0].orderDate);

            }
        });
}

function deleteOrder() {
    $.ajax
        (
        {
            type: "POST",
            dataType: "json",
            url: "/Schedule/deleteOrder/",
            data: { id: $('#mnmOrderId').val() },
            success: function (d) {
                //alert(d);
            }
        });
}

function enableEdit() {
    $('#mnmEdit').prop('disabled', false);

}

function editOrder_()
{
    $.ajax
        (
        {
            type: "POST",
            dataType: "json",
            url: "/Schedule/editOrder/",
            data: { id: $('#mnmOrderId').val(), orderDate: $('#mnmDat').val() },
            success: function (d) {
                //alert(d);
            }
        });
}

function fastPreview(id)
{

    $.ajax
        (
        {
            type: "POST",
            dataType: "json",
            url: "/Schedule/previewOrder/",
            data: { id: id },
            success: function (d) {
                $('#mnmFastPat').empty();
                $('#mnmFastDatOld').empty();

                $('#mnmFastPat').append(d.data[0].patientName);
                $('#mnmFastDatOld').append(d.data[0].orderDate);
                //$('#fastPreview').show();

            }
        });
}

function cancelFastPreview()
{
    $('#mnmFastPat').empty();
    $('#mnmFastPat').append(".")
    //$('#fastPreview').hide();

}

//function mnmNextClick()
//{
//    var acLength = activeCells.length;
//    nextIndex += 1;
//    if(nextIndex==acLength)
//    {
//        nextIndex = 0;
//        previousIndex = acLength - 1;
//    }
//    else
//    {
//        previousIndex = nextIndex - 1;
//    }
//    $(activeCells[nextIndex]).focus();
//}

//function mnmPreviousClick()
//{

//}