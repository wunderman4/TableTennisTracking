using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTennisTracker.Services
{
    public class Logging
    {
        public string fileNameLocation;

        public Logging(string fileNameLocation)
        {
            this.fileNameLocation = fileNameLocation;
        }

        public void logIt(string data)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(this.fileNameLocation, true))
            {
                file.WriteLine(data);
            }
        }
    }
}
