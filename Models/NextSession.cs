using System;
using System.Collections.Generic;
using System.Web;

public sealed class NextSession
{
    public const int January = 1; // month limit for winter registrations
    public const int August = 8;  // month limit for automn registrations
    private static readonly NextSession instance = new NextSession();
    public static NextSession Instance => instance;

    // Cette correction permet d'avoir un réglage de la prochaine session scolaire différent pour chacun des usagers en ligne.
    public static DateTime CurrentDate
    {
        get
        {
            if (HttpContext.Current.Session != null)
            {
                if (HttpContext.Current.Session["CurrentDate"] == null)
                    HttpContext.Current.Session["CurrentDate"] = DateTime.Now;
                return (DateTime)HttpContext.Current.Session["CurrentDate"];
            }
            return DateTime.Now;
        }
        set
        {
            HttpContext.Current.Session["CurrentDate"] = value;
        }
    }

    static public List<int> ValidSessions
    {
        get
        {
            List<int> result = new List<int>();
            if (CurrentDate.Month > January && CurrentDate.Month <= August)
            { result.Add(1); result.Add(3); result.Add(5); }
            else
            { result.Add(2); result.Add(4); result.Add(6); }
            return result;
        }
    }
    static public int Year
    {
        get
        {
            int value = CurrentDate.Year;
            if (CurrentDate.Month > August && CurrentDate.Month <= 12) value++;
            return value;
        }
    }
    static public string ShortCaption =>
        (ValidSessions.Contains(1) ? "Automne " : "Hiver ") + Year;
    static public string Caption => "Session " + ShortCaption;
}
}