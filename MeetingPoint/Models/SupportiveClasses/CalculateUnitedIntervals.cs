using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingPoint.Models.SupportiveClasses
{
    public static class CalculateUnitedIntervals
    {

        public static List<UnitedIntervals> CalculateIntervals(List<Interval> intervals, Calendar calendar) 
        {
            List<UnitedIntervals> unitedIntervals = new List<UnitedIntervals>();

            var dateFrom = calendar.DateFrom; 
            var dateTo = calendar.DateTo;
          
            while (dateFrom.Date <= dateTo.Date)
            {
                for (int i = 10; i <= 23; i++)
                {
                    var temp = intervals.FindAll(j => j.DateBegin.Hour <= i && (j.DateEnd.Hour > i || j.DateEnd.Minute == 59) && j.DateBegin.Date == dateFrom.Date);
                    foreach (var item in temp)
                    {
                        DateTime dateBegin;
                        DateTime dateEnd;
                        dateBegin = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, i, dateFrom.Minute, dateFrom.Second);

                        if (item.DateEnd.Minute == 59)
                        {
                            if (i == 23)
                            {
                                dateEnd = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, i, 59, 59);
                            }
                            else
                            {
                                dateEnd = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, i+1, dateFrom.Minute, dateFrom.Second);
                            }                          
                        }
                        else
                        {
                            dateEnd = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, i + 1, dateFrom.Minute, dateFrom.Second);
                        }

                        unitedIntervals.Add(new UnitedIntervals(Guid.NewGuid().ToString(), dateBegin, dateEnd, item.UserId, calendar.Id));
                    }
                }
                dateFrom = dateFrom.Date.AddDays(1);
            }
            return unitedIntervals;
        }


        public static (List<CalculatedInterval>, List<CalculatedIntervalUser>) DivideUnitedIntervalsInTables(List<UnitedIntervals> unitedIntervals)
        {
            List<CalculatedInterval> calculatedIntervals = (
                from ui in unitedIntervals
                group ui by new { ui.DateFrom, ui.DateTo, ui.CalendarId } 
                into newui
                select new CalculatedInterval{ Id = Guid.NewGuid().ToString(), CalendarId = newui.Key.CalendarId, DateFrom = newui.Key.DateFrom, DateTo = newui.Key.DateTo }).ToList();

            List<CalculatedIntervalUser> calculatedIntervalUsers = new List<CalculatedIntervalUser>();

            foreach (var uInteval in unitedIntervals)
            {
                var hash = (uInteval.DateFrom.GetHashCode().ToString() + uInteval.DateTo.GetHashCode().ToString());

                var calcInt = calculatedIntervals.Find(ci => (ci.DateFrom.GetHashCode().ToString() + ci.DateTo.GetHashCode().ToString()) == hash);

                CalculatedIntervalUser calculatedIntervalUser = new CalculatedIntervalUser { CalculatedIntervalId = calcInt.Id, UserId = uInteval.UserId };

                calculatedIntervalUsers.Add(calculatedIntervalUser);
            }

            return (calculatedIntervals, calculatedIntervalUsers);
        }





    }
}
