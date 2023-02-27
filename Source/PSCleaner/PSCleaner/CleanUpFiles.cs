using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;  // Windows PowerShell assembly.
using System.Text.RegularExpressions;

namespace FileScanner
{
    /// <summary>
    ///
    /// Summary:
    ///     Find Files and Folders that have been created since the specified time frame.
    ///     
    ///     The timestamp is based on the CreationTime property
    ///
    /// Parameters:
    ///   Path:
    ///     The specified path.
    ///   
    ///   Depth:
    ///     The depth to recurse
    ///     Default = -1
    ///     
    ///   CreationTime:
    ///     The Creation Timestamp to use to query for files and folders that are newer
    ///     Default = 1 hour behind the current time
    ///     
    ///   Recurse
    ///     Allow for recursive file and folder scans
    ///     
    /// Returns:
    ///     #####
    ///
    /// Exceptions:
    ///     #####
    ///     
    /// Examples:
    /// 
    /// 
    /// Notes:
    /// </summary>

    // Declare the class as a PSCmdlet and specify the
    // appropriate verb and noun for the cmdlet name.
    // [Cmdlet(VerbsCommunications.Send, "Message")]
    [Cmdlet(VerbsCommon.Get, "CleanUpFiles")]
    public class CleanUpFilesCommand : PSCmdlet
    {
        [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string Path { get; set; }

        [Parameter]
        public int Depth { get; set; } = -1;

        [Parameter(HelpMessage = "File/Folder Creation TimeStamp")]
        [Alias("TimeStamp")]
        public DateTime CreationTime { get; set; } = DateTime.Now.AddHours(-1);

        [Parameter]
        public SwitchParameter Recurse { get; set; }

        [Parameter]
        public SwitchParameter Collapse { get; set; }

        [Parameter]
        public string ExcludeString { get; set; }

        [Parameter]
        public string ExcludeFile { get; set; }

        [Parameter]
        public SwitchParameter Delete { get; set; }

        [Parameter]
        public SwitchParameter Force { get; set; }

        // Override the ProcessRecord method to process
        // the supplied user name and write out a
        // greeting to the user by calling the WriteObject
        // method.

        protected override void ProcessRecord()
        {
            #region Set Process Variables
            if (Path == null)
            {
                var CurPath = this.SessionState.Path.CurrentFileSystemLocation;
                Path = $@"{CurPath}";
            } else
            {
                Path = $@"{Path}\";
            }

            if (Delete == true)
            {
                if (!Force == true)
                {
                    ConsoleWriter.WriteLine($"{{FC={ConsoleColor.Red}}}\nYou need to use the -Force switch parameter with the -Delete switch to Delete items.\n{{/FC}}");
                    return;
                }
            }

            Regex RegExString = new Regex($@"^\s\s\s");

            List<object> ReturnList = new List<object>();
            #endregion Set Process Variables

            #region Write Header
             WriteVerbose($"[{DateTime.Now}]..Scanning");
            #endregion Write Header

            #region Set Exclude RegEx String
            if (ExcludeFile != null || ExcludeString != null)
            {
                if (ExcludeString != null)
                {
                    RegExString = new Regex(Regex.Escape($@"{ExcludeString}").Replace(",", "|"));
                }
                else
                {
                    // The file to read
                    string filePath = $@"{ExcludeFile}";

                    // Read the file and append it to a string variable
                    string fileContents = File.ReadAllText(filePath);
                    string[] lines = fileContents.Split('\n');
                    string joinedLines = string.Join(",", lines);

                    RegExString = new Regex(Regex.Escape($@"{joinedLines}").Replace(",", "|"));
                }
            }
            #endregion Set Exclude RegEx String

            #region Set Directory Search Path
            DirectoryInfo dirInfo = new DirectoryInfo($@"{Path}");
            #endregion Set Directory Search Path

            try
            {
                #region File scan for the initial starting directory
                foreach (FileInfo fileInfo in dirInfo.GetFiles("*", SearchOption.TopDirectoryOnly))
                {
                    if (fileInfo.CreationTime >= CreationTime)
                    {
                        if (RegExString.IsMatch(fileInfo.FullName))
                        {
                            // Do Nothing
                        }
                        else
                        {
                            IOObject curfileorfolder = new IOObject
                            {
                                FullPath = fileInfo.FullName,
                                IsDirectory = false,
                                CreationDate = fileInfo.CreationTime,
                                OnDisk = true
                            };

                            try
                            {
                                if (Delete)
                                {
                                    File.Delete($@"{curfileorfolder.FullPath}");
                                    curfileorfolder.OnDisk = false;
                                }
                            }
                            catch
                            {
                                // Do Nothing
                            }
                            ReturnList.Add(curfileorfolder);
                        }
                    }
                }
            
                #endregion File scan for the initial starting directory

                #region Subdirectory scan for the initial starting directory
                foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    var curfileorfolder = new IOObject();

                    if (subDirInfo.CreationTime >= CreationTime)
                    {
                        if (RegExString.IsMatch(subDirInfo.FullName))
                        {
                            // Do Nothing
                        }
                        else
                        {
                            curfileorfolder.FullPath = subDirInfo.FullName;
                            curfileorfolder.IsDirectory = true;
                            curfileorfolder.CreationDate = subDirInfo.CreationTime;
                            curfileorfolder.OnDisk = true;

                            try
                            {
                                if (Delete)
                                {
                                    Directory.Delete($@"{curfileorfolder.FullPath}", true);
                                    curfileorfolder.OnDisk = false;
                                }
                            }
                            catch
                            {
                                // Do Nothing
                            }

                            ReturnList.Add(curfileorfolder);
                        }
                    }

                    if (curfileorfolder.FullPath != null && Collapse == true)
                    {
                        // Do Nothing
                    }
                    else
                    {
                        if (Recurse == true)
                        {
                            IterateSubDirectories(subDirInfo, 1);
                        }
                    }
                }
                #endregion Subdirectory scan for the initial starting directory

                #region Subdirectory File and Folder Scan Loop
                void IterateSubDirectories(DirectoryInfo dir, int level)
                {
                    #region sub directory depth check
                    if (level > Depth && Depth != -1)
                    {
                        return;
                    }
                    #endregion sub directory depth check

                    try
                    {
                        #region sub directory file scan
                        foreach (FileInfo fileInfo in dir.GetFiles("*", SearchOption.TopDirectoryOnly))
                        {
                            if (fileInfo.CreationTime >= CreationTime)
                            {
                                if (RegExString.IsMatch(fileInfo.FullName))
                                {
                                    // Do Nothing
                                }
                                else
                                {
                                    IOObject curfileorfolder = new IOObject
                                    {
                                        FullPath = fileInfo.FullName,
                                        IsDirectory = false,
                                        CreationDate = fileInfo.CreationTime,
                                        OnDisk = true
                                    };

                                    try
                                    {
                                        if (Delete)
                                        {
                                            File.Delete($@"{curfileorfolder.FullPath}");
                                            curfileorfolder.OnDisk = false;
                                        }
                                    }
                                    catch
                                    {
                                        // Do Nothing
                                    }

                                    ReturnList.Add(curfileorfolder);
                                }
                            }
                        }
                        #endregion sub directory file scan

                        #region sub directory folder scan
                        foreach (DirectoryInfo subDirInfo in dir.GetDirectories("*", SearchOption.TopDirectoryOnly))
                        {
                            var cursubfolder = new IOObject();

                            if (subDirInfo.CreationTime >= CreationTime)
                            {
                                if (RegExString.IsMatch(subDirInfo.FullName))
                                {
                                    // Do Nothing
                                }
                                else
                                {
                                    cursubfolder.FullPath = subDirInfo.FullName;
                                    cursubfolder.IsDirectory = true;
                                    cursubfolder.CreationDate = subDirInfo.CreationTime;
                                    cursubfolder.OnDisk = true;

                                    try
                                    {
                                        if (Delete)
                                        {
                                            Directory.Delete($@"{cursubfolder.FullPath}", true);
                                            cursubfolder.OnDisk = false;
                                        }
                                    }
                                    catch
                                    {
                                        // Do Nothing
                                    }

                                    ReturnList.Add(cursubfolder);
                                }
                            }

                            if (cursubfolder.FullPath != null && Collapse == true)
                            {
                                // Do Nothing
                            }
                            else
                            {
                                if (Recurse == true)
                                {
                                    IterateSubDirectories(subDirInfo, level + 1);
                                }
                            }
                        }
                        #endregion sub directory folder scan
                    }
                    catch (UnauthorizedAccessException uae)
                    {
                        WriteError(new ErrorRecord(uae, "AccessDenied", ErrorCategory.InvalidOperation, dirInfo.FullName));

                    }
                }
                #endregion Subdirectory File and Folder Scan Loop

                #region Write Pipeline Output
                //foreach (var i in ReturnList)
                //{
                //    WriteObject(i); // Prints items to the Pipeline for futher processing    
                //}

                WriteObject(ReturnList, true);
                #endregion Write Pipeline Output
            }
            catch (UnauthorizedAccessException uae)
            {
                WriteError(new ErrorRecord(uae, "AccessDenied", ErrorCategory.InvalidOperation, dirInfo.FullName));

            }

            #region Write Footer
            WriteVerbose($"[{DateTime.Now}]..Completed\n\n");

            WriteVerbose($"[Total Count         ]..{ReturnList.Count}\n\n");
            #endregion Write Footer
        }
    }
}