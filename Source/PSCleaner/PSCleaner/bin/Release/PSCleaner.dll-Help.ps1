
### Get-CleanUpFiles command help
@{
	command = 'Get-CleanUpFiles'
	synopsis = 'Find Files and Folders that have been created since the specified time frame.'
	description = 'Find Files and Folders that have been created since the specified time frame.

 The timestamp is based on the CreationTime property'
	parameters = @{
		Collapse = 'Description: Collapse the results based on the Folder Path being created in the same time frame.
 Notes: If the Folder Path was created at in the same time frame as the files, then only the directory will flagged.
 Alias:
 ValidateSet:'
		CreationTime = 'Description: The time frame to search for files and folders that have been created.
 Notes:
 Alias:
 ValidateSet:'
		Depth = 'Description: The depth of the search.
 Notes: The default is -1, which means no limit.
 Alias:
 ValidateSet:'
		ExcludeFile = 'Description: A file with a list of paths to exclude from the search.
 Notes:
 Alias:
 ValidateSet:'
		ExcludeString = 'Description: A string with a list of paths to exclude from the search.
 Notes:	This is a single string with a list of paths separated by a comma.
 Alias:
 ValidateSet:'
		Path = 'Description: The path to start the search from.
 Notes: The default is the current directory.
 Alias:
 ValidateSet:'
		Recurse = 'Description: Recurse into subdirectories.
 Notes:
 Alias:
 ValidateSet:'
        Delete = 'Description: Automatically Delete the files and folders that are found in the search.
 Notes: The -Force switch has to be used as well to delete the files and folders.  This is to prevent accidental deletion.
 Alias:
 ValidateSet:'
        Force = 'Description: This is to acknowledge that the files and folders will be deleted.
 Notes:
 Alias:
 ValidateSet:'
	}
	inputs = @(
		@{
			type = ''
			description = ''
		}
	)
	outputs = @(
		@{
			type = 'FileScanner.IOObject'
			description = 'CreationDate : <DateTime>
 IsDirectory  : <Boolean>
 FullPath     : <String>
 OnDisk       : <Boolean>'
		}
	)
	notes = '
    • [Original Author]
    o Michael Arroyo
 • [Original Build Version]
	o 23.02.2201 [XX = Year (.) XX = Month (.) XX = Day XX = Build revision]
 • [Latest Author]
	o
 • [Latest Build Version]
	o
 • [Comments]
	o
 • [PowerShell Compatibility]
	o  2,3,4,5.x
 • [Forked Project]
	o
 • [Links]
	o
 • [Dependencies]
	o
'
	examples = @(
		@{
			code = {
				Get-CleanUpFiles -Path $env:TEMP
			}
			remarks = '
			Description: Search for files in the users temp direcotry that have been created in the last hour.
    Notes: The default time frame is 1 hour.'
			test = { . $args[0] }
		}
		@{
			code = {
				Get-CleanUpFiles -Path $env:TEMP -CreationTime (Get-Date).AddDays(-1)
			}
			remarks = '
			Description: Set the time frame to the past 24 hours.
    Notes:'
			test = { . $args[0] }
		}
		@{
			code = {
				Get-CleanUpFiles -Path 'C:\Temp' -CreationTime (Get-Date).AddDays(-1) -Depth 1 -Recurse -ExcludeString 'C:\Temp\Exclude2.txt,C:\Temp\Exclude3.txt'
			}
			remarks = '
			Description: Search a depth of 1, while excluding the files from being tracked.
	Notes:'
			test = { . $args[0] }
		}
		@{
			code = {
				Get-CleanUpFiles -Path 'C:\' -CreationTime (Get-Date).AddDays(-1) -Recurse -ExcludeFile '.\ExcludeList.txt' -Collapse
			}
			remarks = '
			Description: Search the C:\ Drive for all files created in the last 24 hours, while excluding paths from the ExcludeList.txt file.
	Notes: Use the Collapse parameter to only show the directory that was created in the same time frame as the files.'
			test = { . $args[0] }
		}
		@{
			code = {
				Get-CleanUpFiles -Path 'C:\' -CreationTime (Get-Date).AddDays(-5) -Recurse -ExcludeFile '.\ExcludeList.txt' -Collapse -Delete -Force
			}
			remarks = '
			Description: Automatically delete all files and folders that were created in the last 5 days.
	Notes:'
			test = { . $args[0] }
		}
	)
	links = @(
		@{ text = 'PSCleaner C# CmdLet'; URI = 'https://github.com/kgkzworld/PSCleaner/blob/main/README.md' }
	)
}
