﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MCForge
{
    static class ProfanityFilter
    {
        private static Dictionary<string, string> RegexReduce;
        private static IEnumerable<string> BadWords;
        public static void Init()
        {
            // Initializes the reduction dictionary and word list
            RegexReduce = new Dictionary<string, string>();
            RegexReduce.Add("a", "[@]");
            RegexReduce.Add("b", "i3|l3");
            RegexReduce.Add("c", "[(]");
            RegexReduce.Add("e", "[3]");
            RegexReduce.Add("f", "ph");
            RegexReduce.Add("g", "[6]");
            // Because Is and Ls are similar, the swear list will contain a lowercase I instead of Ls.
            // For example, the word "asshole" would be saved as "asshoie".
            RegexReduce.Add("i", "[l!1]");
            RegexReduce.Add("o", "[0]");
            RegexReduce.Add("q", "[9]");
            RegexReduce.Add("s", "[$5]");
            RegexReduce.Add("w", "vv");
            RegexReduce.Add("z", "[2]");

            // Load/create the badwords.txt file and import them into the BadWords list
            if (!File.Exists("text/badwords.txt"))
            {
                // No file exists yet, so let's create one
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("# This file contains a list of bad words to remove via the profanity filter");
                sb.AppendLine("# Each bad word should be on a new line all by itself");
                File.WriteAllText("text/badwords.txt", sb.ToString());
            }

            // OK the file should exist now
            var tempBadWords = File.ReadAllLines("text/badwords.txt").Where(s => s.StartsWith("#") == false);

            // Run the badwords through the reducer to ensure things like Ls become Is and everything is lowercase
            // Also remove lines starting with a "#" since they are comments
            BadWords = from s in tempBadWords where !s.StartsWith("#") select Reduce(s.ToLower());
        }

        public static string Parse(string text)
        {

            var result = new List<string>();
            var originalWords = text.Split(' ');
            var reducedWords = Reduce(text).Split(' ');
            for(var i=0; i < originalWords.Length; i++)
            {
                if (BadWords.Contains(reducedWords[i].ToLower()))
                {
                    // A reduced word matched a bad word from our file!
                    result.Add(new String('*', originalWords[i].Length));
                }
                else
                {
                    result.Add(originalWords[i]);
                }
            }

            return String.Join(" ", result.ToArray());
        }


        private static string Reduce(string text)
        {
            text = text.ToLower();
            foreach (var pattern in RegexReduce)
            {
                text = Regex.Replace(text, pattern.Value, pattern.Key/*, RegexOptions.IgnoreCase*/);
            }
            return text;
        }
    }
}