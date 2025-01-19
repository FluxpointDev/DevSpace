using System;



//!Classes directly related to the minecraft server.
namespace LibMCRcon.RCon
{
    //!Track time passing using computer time.
    public class TimeCheck
    {
        DateTime dT;
        
        /// <summary>
        /// Create a TimeCheck object setting to a default of 5 seconds into the future from the time created.
        /// </summary>
        public TimeCheck()
        {
            dT = DateTime.Now.AddMilliseconds(5000);
        }
        /// <summary>
        /// Create a TimeCheck object set to X milliseconds into the future from the time created.
        /// </summary>
        /// <param name="Milliseconds">Number of milliseconds to add.</param>
        public TimeCheck(Int32 Milliseconds)
        {
            dT = DateTime.Now.AddMilliseconds((double)Milliseconds);

        }

        /// <summary>
        /// Checks to see if the current time is passed the stored checkpoint and returns true if passed.
        /// </summary>
        public bool Expired
        {
            get
            {
                return (DateTime.Now > dT) ? true : false;
            }
        }

        /// <summary>
        /// Reset the time point by adding X milliseconds to the current time from calling this function.
        /// </summary>
        /// <param name="Milliseconds">Number of milliseconds to add.</param>
        public void Reset(Int32 Milliseconds)
        {
            dT = DateTime.Now.AddMilliseconds((double)Milliseconds);
        }

        public DateTime CheckDT { get { return dT; } set { dT = value; } }

        public TimeSpan TimeLeft { get { return DateTime.Now - dT; } }
        public TimeSpan TimeLapse { get { return dT - DateTime.Now; } }

    }

}
