using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubtitleDownloader;
using SubtitleDownloader.Core;
using SubtitleDownloader.Implementations;
using SubtitleDownloader.Implementations.Subscene;
using SubtitleDownloader.Util;
using System.IO;
using SubtitleDownloader.Implementations.SubtitleSource;

namespace SubDl
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Count() >= 1)
            {
                string videoPath = args[0];
                FileInfo videoFileInfo;

                try
                {
                    videoFileInfo = new FileInfo(videoPath);

                    ISubtitleDownloader loader = new SubsceneDownloader2();
                    SearchQuery queryFolderName = new SearchQuery(videoFileInfo.Directory.Name);
                    List<Subtitle> subs = loader.SearchSubtitles(queryFolderName);

                    SearchQuery queryFileName = new SearchQuery(videoFileInfo.Name);
                    subs.AddRange(loader.SearchSubtitles(queryFileName));

                    int subNum = 0;
                    foreach (var sub in subs)
                    {
                        //Console.WriteLine("\n{0} File Name: {1}\nLanguage: {2}\nId: {3}", subNum, sub.FileName, sub.LanguageCode, sub.Id);
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("{0} - File Name:", subNum);
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(sub.FileName);
                        subNum++;
                    }

                    Console.WriteLine("Enter id to download..\n");
                    string idStr = Console.ReadLine();


                    int id = int.Parse(idStr);

                    if (id >= 0 && id < subs.Count)
                    {
                        var fileInfos = loader.SaveSubtitle(subs[id]);

                        foreach (var fileInfo in fileInfos)
                        {
                            Console.WriteLine(fileInfo.FullName);
                            string subFileName = videoFileInfo.Directory.FullName + "\\" + videoFileInfo.Name + fileInfo.Extension;
                            Console.WriteLine(subFileName);
                            File.Move(fileInfo.FullName, subFileName);
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Abandon ship, please save this log for commander: \n{0}\n{1}", e.Message, e.TargetSite);
                    Console.Read();
                    System.Environment.Exit(2);
                    
                }
            }
            else
            {
                Console.WriteLine("Too few arguments");
                Console.Read();
                Environment.Exit(1);
            }
        }
    }
}
