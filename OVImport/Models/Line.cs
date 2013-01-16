using System;
using System.Collections.Generic;

namespace OVImport.Models
{
    public class Line
    {
        public long id;
        public string date;
        public string station;
        public string action;
        public long cardid;
        public long userid;

        public Line(IList<string> csvFields)
        {
            id = Convert.ToInt64(csvFields[0]);
            date = csvFields[1];
            station = csvFields[2];
            action = csvFields[3];
            cardid = Convert.ToInt64(csvFields[4]);
            userid = Convert.ToInt64(csvFields[5]);
        }

        public override string ToString()
        {
            return string.Format("id = {0}, date = {1}, station = {2}, action = {3}, cardid = {4}, userid = {5}", id, date, station, action, cardid, userid);
        }
    }
}
