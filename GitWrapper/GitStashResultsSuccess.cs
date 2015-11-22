using System;

namespace GitWrapper
{
    internal class GitStashResultsSuccess : IGitStashResults
    {
        public bool Success
        {
            get
            {
                return true; ;
            }
        }
    }

    internal class GitStashResultsFailure : IGitStashResults
    {
        public bool Success
        {
            get
            {
                return false; ;
            }
        }
    }
}