# PSCleaner C# CmdLet
PSCleaner is a PowerShell Ccmdlet, designed to query and remove files and folders that have been created since a specified date and time.

PSCleaner is 50% faster at querying files just using the normal search.  It's 75% to 80% faster using the -Collapse switch.

The -Collapse switch will stop processing subdirectories that are greater than the date and time specified.  This allows the process to
continue to another directory without parsing all the nested files in that folder.

This is a great tool for detecting changes to your file system after an installation, or running configuration changes.
