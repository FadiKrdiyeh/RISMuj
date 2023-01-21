using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIS
{
    public enum ReqPatientVals
    {
        //patient
        //firstname ,
        middlename= 1,
      //  lastname,
        gendre,
        mothername,
        birthdate,
        age,
        mobilephone,
        landphone,
        currentaddress,
        residentaddress,
        workphone,
        workaddress,
        nearestperson,
        nearestpersonphone,
        birthplace,
        nationalidnumber,
        nationality,
        worktype,
        notes,
        martialstatus,
        insertdate,

        //order
        Doctor,
        DocumnetId,
        rowsPerPage=200,
    }
}