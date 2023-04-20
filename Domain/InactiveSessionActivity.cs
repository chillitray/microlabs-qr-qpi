namespace Domain
{
    public enum InactiveSessionStatusOptions
    {
        LOGGEDOUT=1,
        EXPIRED=2
    } 

    public class InactiveSessionActivity : SessionActivity
    {
        public InactiveSessionStatusOptions status { get; set; }
        public DateTime expired_at { get; set; }   
    }
}


// status	enum (  LoggedOut=>1, Expired=>2   )
// expired_at	Datetime