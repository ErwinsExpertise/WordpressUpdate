/* @(#) Program.cs
 * @Author: Erwin Evans
 * @Purpose: Does shit
 * 
 * Version 1.0.0
 * 
 * (c) Erwin Evans 2018, All rights reserved.
 */

using System;
using System.Threading.Tasks;

namespace WordpressUpdate
{

    
class Program
    {
        static void Main(string[] args)
        {

            //Updates
            Console.WriteLine("Checking for updates on GPL Vault... ");
            string url = "https://www.gplvault.com/product/affiliatewp-signup-referrals-add-on/";
            string version = Worker.getVersion(url);
            Console.WriteLine("The current version(s) are: " + Environment.NewLine + version);

            //Downloads
            string[] download = Worker.getDownload().Split('/');
            string link = string.Join(Environment.NewLine, download);
            Console.WriteLine("The current downloads are: " + Environment.NewLine + link);

            //Auth
            Console.WriteLine(Environment.NewLine + "Please enter your username: ");
            string username = Console.ReadLine();
            Console.WriteLine(Environment.NewLine + "Please enter your password: ");
            string password = Console.ReadLine();

            //Download updates
            Task.WaitAll(Worker.getUpdateAsync(username, password));

            //Just a pause for terminal
            Console.ReadLine();
        }//end main


    }//end program
}
