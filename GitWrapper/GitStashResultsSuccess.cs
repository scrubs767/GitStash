using System;

namespace GitWrapper
{
    internal class GitStashResultsSuccess : GitStashResults, IGitStashResults
    {
        public GitStashResultsSuccess(string msg)
        :    base(true, msg)
        {

        }
    }

    internal class GitStashResultsFailure : GitStashResults, IGitStashResults
    {
        public GitStashResultsFailure(string msg)
        :    base(false, msg)
        {

        }
    }
}