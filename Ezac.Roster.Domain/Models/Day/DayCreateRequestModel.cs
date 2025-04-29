namespace Ezac.Roster.Infrastructure.Models.Day
{
    public class DayCreateRequestModel
    {
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public int CalendarId { get; set; }
    }
}