﻿using FuzzyMatch.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DuoVia.FuzzyStrings;

namespace FuzzyMatch.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            var aa = "Malik rizwan".FuzzyEquals("m.Rizwan");
            var a= "Malik rizwan".FuzzyMatch("mr. Rizwan");
            var a1 = "Malik rizwan".FuzzyMatch("mr. Rizwan");
            var a2 = "Malik rizwan".FuzzyMatch("m Rizwan");
            var a3 = "Malik rizwan".FuzzyMatch("Rizwan");
            var a4 = "Malik rizwan".FuzzyMatch("Malik");
            var a5 = "Malik rizwan".FuzzyMatch("rizwan");
            var a6 = "Malik rizwan".FuzzyMatch("malik");
            var a7 = "Malik rizwan".FuzzyMatch("Rizwan Malik");
            var a8 = "Malik rizwan".FuzzyMatch("rizwan malik");
            var a9 = "Malik rizwan".FuzzyMatch("MalikRizwan");
            var a10 = "Malik rizwan".FuzzyMatch("Malik Rizwan");
            return View(new VModel());
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase FileUpload1, HttpPostedFileBase FileUpload2)
        {
            if (FileUpload1 != null && FileUpload1.ContentLength > 0 && FileUpload2 != null && FileUpload2.ContentLength > 0)
            {
                List<string> headerFile1 = new List<string>();
                List<string> headerFile2 = new List<string>();
                List<string[]> dataFile1 = new List<string[]>();
                List<string[]> dataFile2 = new List<string[]>();
                // there's a file that needs our attention
                if (!(Directory.Exists(HttpContext.Server.MapPath("~/CSVFiles"))))
                {
                    Directory.CreateDirectory(HttpContext.Server.MapPath("~/CSVFiles"));
                }
                string file1 = Path.Combine(HttpContext.Server.MapPath("~/CSVFiles"), "file1.csv");
                FileUpload1.SaveAs(file1);
                string file2 = Path.Combine(HttpContext.Server.MapPath("~/CSVFiles"), "file2.csv");
                FileUpload2.SaveAs(file2);
                using (var reader = new StreamReader(FileUpload1.InputStream))
                {
                    List<string> lines = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        lines.Add(line);
                    }
                    dataFile1 = (from line in lines
                                 select line.Split(',')).ToList();
                    if (dataFile1.Count > 0)
                    {
                        headerFile1 = dataFile1.FirstOrDefault().ToList();
                        dataFile1.RemoveAt(0);
                    }

                }

                using (var reader = new StreamReader(FileUpload2.InputStream))
                {
                    List<string> lines = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        lines.Add(line);
                    }
                    dataFile2 = (from line in lines
                                 select line.Split(',')).ToList();
                    if (dataFile2.Count > 0)
                    {
                        headerFile2 = dataFile2.FirstOrDefault().ToList();
                        dataFile2.RemoveAt(0);
                    }

                }
                var model = new VModel();
                model.data1 = dataFile1;
                model.data2 = dataFile2;
                model.header1 = headerFile1;
                model.header2 = headerFile2;
                return View(model);
            }
            return View(new VModel());
        }
        [HttpPost]
        public ActionResult JoinData(FormCollection form)
        {
            string ddl1 = form["ddl1"];
            string ddl2 = form["ddl2"];
            string ddljointype = form["ddljointype"];
            var model = new VModel();
            if (!string.IsNullOrEmpty(ddljointype) && !string.IsNullOrEmpty(ddl1) && !string.IsNullOrEmpty(ddl2))
            {
                List<string> headerFile1 = new List<string>();
                List<string> headerFile2 = new List<string>();
                List<string[]> dataFile1 = new List<string[]>();
                List<string[]> dataFile2 = new List<string[]>();

                string file1 = Path.Combine(HttpContext.Server.MapPath("~/CSVFiles"), "file1.csv");
                string file2 = Path.Combine(HttpContext.Server.MapPath("~/CSVFiles"), "file2.csv");
                using (var reader = new StreamReader(file1))
                {
                    List<string> lines = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        lines.Add(line);
                    }
                    dataFile1 = (from line in lines
                                 select line.Split(',')).ToList();
                    if (dataFile1.Count > 0)
                    {
                        headerFile1 = dataFile1.FirstOrDefault().ToList();
                        dataFile1.RemoveAt(0);
                    }

                }

                using (var reader = new StreamReader(file2))
                {
                    List<string> lines = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        lines.Add(line);
                    }
                    dataFile2 = (from line in lines
                                 select line.Split(',')).ToList();
                    if (dataFile2.Count > 0)
                    {
                        headerFile2 = dataFile2.FirstOrDefault().ToList();
                        dataFile2.RemoveAt(0);
                    }
                }
                
                switch (ddljointype)
                {
                    case "Inner":
                        var index1 = headerFile1.FindIndex(s => s.Equals(ddl1));
                        var index2 = headerFile2.FindIndex(s => s.Equals(ddl2));
                        var matchingUsers = (from d1 in dataFile1
                                             join d2 in dataFile2 on d1.ElementAt(index1) equals d2.ElementAt(index2)
                                             select d1
                                             ).ToList();
                        model.dataResult = matchingUsers;
                        model.headerResult = headerFile1;
                        break;
                    case "Left":
                        index1 = headerFile1.FindIndex(s => s.Equals(ddl1));
                        index2 = headerFile2.FindIndex(s => s.Equals(ddl2));
                        matchingUsers = (from d1 in dataFile1
                                         join d2 in dataFile1 on d1.ElementAt(index1) equals d2.ElementAt(index2)
                                         into gj
                                         from mlist in gj.DefaultIfEmpty()
                                         select d1
                                             ).ToList();
                        model.dataResult = matchingUsers;
                        model.headerResult = headerFile1;
                        break;
                    case "Right":
                        index1 = headerFile1.FindIndex(s => s.Equals(ddl1));
                        index2 = headerFile2.FindIndex(s => s.Equals(ddl2));
                        matchingUsers = (from d2 in dataFile2
                                         join d1 in dataFile1 on d2.ElementAt(index2) equals d1.ElementAt(index1)
                                         into gj
                                         from mlist in gj.DefaultIfEmpty()
                                         select d2
                                             ).ToList();
                        model.dataResult = matchingUsers;
                        model.headerResult = headerFile2;
                        break;
                    case "Fuzzy":
                        //bool isEqual = "".FuzzyEquals("");
                        //double coefficient = input.FuzzyMatch(name);
                        index1 = headerFile1.FindIndex(s => s.Equals(ddl1));
                        index2 = headerFile2.FindIndex(s => s.Equals(ddl2));
                        matchingUsers = new List<string[]>();
                        foreach (var item in dataFile1)
                        {
                            if (dataFile2.Where(w => w.ElementAt(index1).FuzzyMatch(item.ElementAt(index2)) > 0.3).Count()>0)
                                matchingUsers.Add(item);
                        }
                        model.dataResult = matchingUsers;
                        model.headerResult = headerFile1;
                        break;
                    default:
                        //do a different thing
                        break;
                }
            }
            
            
            return View("Index", model);
        }
    }
}
