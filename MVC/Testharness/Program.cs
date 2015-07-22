using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Testharness {
    class Program {

        private static void Connect(int user) {
            long total = 0;
            long average = 0; 


            for (int i = 0; i < 1000; i++) {

                try {

                    var sw = new Stopwatch();

                    sw.Start();

                    var hwr = WebRequest.Create("http://localhost:53624/");

                    var response = hwr.GetResponse();

                    // Get the stream associated with the response.
                    Stream receiveStream = response.GetResponseStream();

                    // Pipes the stream to a higher level stream reader with the required encoding format. 
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);


                    var responseString = readStream.ReadToEnd();

                    var result = responseString.Contains("Naked Objects Group Ltd") ? "GOOD" : "BAD";

                    sw.Stop();
           
                    total += sw.ElapsedMilliseconds; 
                    average =   total/i; 

                    Debug.WriteLine("user {0} attempt {1} was {2} took {3} ms", user, i, result, sw.ElapsedMilliseconds);

                    //Debug.WriteLine("user {0} average {1} ms", user, average);
                }
                catch (Exception e) {
                    Debug.WriteLine("Exception {0}", e.Message);
                }

            }
        }


        static void Main(string[] args) {
            var tasks = Enumerable.Range(1, 10).Select(i => new Task(() => Connect(i))).ToArray();

            Array.ForEach(tasks, t => t.Start());

            Task.WaitAll(tasks);

        }
    }
}
