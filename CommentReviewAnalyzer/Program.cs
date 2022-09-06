using CommentReviewAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommentReviewAnalyzer
{
    class Program
    {
        private readonly static Regex rgx = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        static void Main(string[] args)
        {
            var sourcePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var docsFolder = Path.Combine(sourcePath, "Documents");

            var txtFilesPath = GetFolderTextFiles(docsFolder);
            var comments = GetComments(txtFilesPath);

            if (comments.Count() > 0)
            {
                var report = GenerateReport(comments);

                if (report != null)
                {
                    PrintReportResult(report);
                }
            }
            else
            {
                Console.WriteLine($"No Comments Found!");
            }
        }

        private static List<string> GetFolderTextFiles(string docsFolder)
        {
            return Directory.EnumerateFiles(docsFolder, "*.txt").ToList();
        }

        private static List<string> GetComments(List<string> txtFilesPath)
        {
            var comments = new List<string>();

            foreach (string filePath in txtFilesPath)
            {
                var lines = File.ReadAllLines(filePath);

                if (lines.Count() > 0)
                {
                    comments.AddRange(lines);
                }

                var fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
                Console.WriteLine($"File processed: {fileName} - Comments: {lines.Count()}");
            }
            return comments;
        }

        private static Report GenerateReport(List<string> comments)
        {
            var report = new Report();

            report.Movers = comments.Where(x => x.ToLower().Contains("mover")).Count();
            report.Shakers = comments.Where(x => x.ToLower().Contains("shaker")).Count();
            report.Shorter = comments.Where(x => x.Length < 15).Count();
            report.Questions = comments.Where(x => x.Contains("?")).Count();
            report.Spam = comments.Count(x => rgx.IsMatch(x));

            return report;
        }

        private static void PrintReportResult(Report report)
        {
            Console.WriteLine($"\n");

            Console.WriteLine($"Shorter than 15 chars: {report.Shorter}");
            Console.WriteLine($"Movers: {report.Movers}");
            Console.WriteLine($"Shakers: {report.Shakers} \n");

            Console.WriteLine($"Spam: {report.Spam}");
            Console.WriteLine($"Questions: {report.Questions}");

            Console.ReadLine();
        }
    }
}
