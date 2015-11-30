using Microsoft.VisualStudio.Shell.Interop;
using GitWrapper;
using System;

namespace GitStash
{
    public class OutputWindowLogger : IGitStashOutputLogger
    {
        private IVsOutputWindowPane output;

        public OutputWindowLogger(IVsOutputWindowPane output)
        {
            this.output = output;
        }

        public void WriteLine(string text)
        {
            output.Activate();
            output.OutputStringThreadSafe(text + Environment.NewLine);
        }
    }
}
