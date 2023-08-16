using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchScanner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TaskVoid().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }

        public static async Task TaskVoid()
        {

            if (!File.Exists("DotaHIT.exe"))
            {
                Console.WriteLine("Please move this file to DotaHit.exe location!!");
                return;
            }
            Console.WriteLine("Drag and drop (or enter) path to scan:");
            string directory = Console.ReadLine().Replace("\"", "");
            if (Directory.Exists(directory))
            {
                var filelist = Directory.GetFiles(directory, "*.w3g");
                if (filelist.Length == 0)
                {
                    Console.WriteLine("No replay files found!");
                    return;
                }
                Console.WriteLine("Replay files count: " + filelist.Length);
                Console.WriteLine("Please enter threads count:");
                var concurrentTasks = int.Parse(Console.ReadLine());
                var semaphore = new System.Threading.SemaphoreSlim(concurrentTasks, concurrentTasks);
                var tasks = new List<System.Threading.Tasks.Task>();
                foreach (var file in filelist)
                {
                    // Blocks execution until another task can be started.
                    // This function also accepts a timeout in milliseconds
                    await semaphore.WaitAsync();

                    // Start a task
                    var task = System.Threading.Tasks.Task.Run( () =>
                    {
                        // Execute long running code
                        try
                        {
                            try
                            {
                                Console.WriteLine("Start scan file:" + file);
                            }
                            catch
                            {

                            }
                            var process = Process.Start("DotaHit.exe", "\"" + file + "\" scan");
                            process.WaitForExit(60000);
                        }
                        catch
                        {
                           
                        }
                        finally
                        {
                            // Signal that the task is completed 
                            // so another task can start
                            semaphore.Release();
                        }
                    });

                    tasks.Add(task);
                }
            }
            else
            {
                Console.WriteLine("Bad directory!");
            }
        }

    }
}
