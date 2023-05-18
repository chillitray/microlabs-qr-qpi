namespace Domain
{
    public enum InactiveSessionStatusOptions
    {
        LOGGEDOUT=1,
        EXPIRED=2,
        AUTO_LOGGED_OUT = 3
    } 

    public class InactiveSessionActivity : SessionActivity
    {
        public InactiveSessionStatusOptions status { get; set; }
        public DateTime expired_at { get; set; }  = DateTime.Now;
    }
}


// status	enum (  LoggedOut=>1, Expired=>2, AutoLoggedOut=>3   )
// expired_at	Datetime