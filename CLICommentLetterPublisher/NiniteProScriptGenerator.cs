using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WebhostMySQLConnection;

namespace CLICommentLetterPublisher
{
    class NiniteProScriptGenerator
    {
        public static void Generate()
        {
            CSV csv = new CSV(new FileStream(@"U:\Applications\NinitePro\GroupApplicationNeeds.csv", FileMode.Open));

            Dictionary<String, List<String>> Lists = new Dictionary<string, List<string>>();

            foreach (String key in csv.AllKeys)
            {
                if (key.Equals("Software")) continue;
                Lists.Add(key, new List<string>());
            }

            foreach (Dictionary<String, String> row in csv.Data)
            {
                foreach (String key in row.Keys)
                {
                    if (key.Equals("Software")) continue;
                    if (row[key].Equals("x"))
                        Lists[key].Add(row["Software"]);
                }
            }

            StreamWriter UpdateScript = new StreamWriter(new FileStream(@"U:\Applications\NinitePro\Update.bat", FileMode.OpenOrCreate));
            UpdateScript.AutoFlush = true;
            foreach (String key in Lists.Keys)
            {
                if (key.Equals("(all domain)"))
                {
                    continue;
                }

                UpdateScript.WriteLine("del -F {0}_Result.txt", key);
                UpdateScript.Write(@".\NinitePro.exe /remote ad:dublinschool.org;(Name={0}*)", key);

                UpdateScript.Write(" /select ");
                foreach(String software in Lists["(all domain)"])
                {
                    UpdateScript.Write("{0} ", software.Replace("\"\"\"", "\""));
                }
                foreach (String software in Lists[key])
                {
                    UpdateScript.Write("{0} ", software.Replace("\"\"\"", "\""));
                }

                UpdateScript.WriteLine("/silent {0}_Result.txt", key.Equals("(all domain)") ? "All_Domain" : key);
            }
            UpdateScript.Flush();
            UpdateScript.Close();

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}
